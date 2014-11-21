// Useful C#
// Copyright (C) 2014 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Smo;
using Randal.Core.Strings;

namespace Randal.Sql.Scripting
{
	public interface IScriptFormatter
	{
		string Format(ScriptSchemaObjectBase schemaObject);
		string Format(StoredProcedure sproc);
		string Format(UserDefinedFunction sproc);
		string Format(View sproc);
		string Format(Table sproc);
	}

	public sealed class ScriptFormatter : IScriptFormatter
	{
		public ScriptFormatter(IServer server)
		{
			_server = server;
		}

		public string Format(ScriptSchemaObjectBase schemaObject)
		{
			var sproc = schemaObject as StoredProcedure;
			var udf = schemaObject as UserDefinedFunction;
			var view = schemaObject as View;
			var table = schemaObject as Table;

			if (sproc != null)
				return Format(sproc);
			
			if (udf != null)
				return Format(udf);
			
			if (view != null)
				return Format(view);

			if (table != null)
				return Format(table);

			throw new NotSupportedException("No support has been defined for type '" + schemaObject.GetType().FullName + "'.");
		}

		public string Format(Table table)
		{
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Str.Catalog, table.Parent.Name },
				{ Str.Schema, table.Schema },
				{ Str.Table, table.Name },
				{ Str.Pre, FormatTablePreSection(table) },
				{ Str.Main, FormatTableMainSection(table) },
				{ Str.Version, 1.ToVersionToday() },
				{ Str.Needs, GetNeeds(table, DatabaseObjectTypes.UserDefinedFunction) ?? string.Empty }
			};

			return Str.Script.Table.Format().With(values);
		}

		private static string FormatTableMainSection(Table table)
		{
			var preOptions = new ScriptingOptions
			{
				PrimaryObject = false, 
				DriForeignKeys = true, 
				DriChecks = true
			};

			var main = string.Join(DoubleLineBreak, table.EnumScript(preOptions).Select(x => x.Trim()));
			main = PatternLineEndings.Replace(main.Trim(), LineBreakTab);

			return main;
		}

		private static string FormatTablePreSection(Table table)
		{
			var preOptions = new ScriptingOptions
			{
				XmlIndexes = true,
				Indexes = true,
				ClusteredIndexes = true,
				DriPrimaryKey = true,
				DriIndexes = true,
				DriClustered = true
			};

			var sectionText = string.Join(DoubleLineBreak, table.EnumScript(preOptions).Select(x => x.Trim()));
			sectionText = PatternLineEndings.Replace(sectionText.Trim(), LineBreakTab);
			
			return sectionText;
		}

		public string Format(View view)
		{
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Str.Catalog, view.Parent.Name },
				{ Str.Schema, view.Schema },
				{ Str.View, view.Name },
				{ Str.Body, view.TextBody },
				{ Str.Header, view.ScriptHeader(true) },
				{ Str.Needs, GetNeeds(view, DatabaseObjectTypes.UserDefinedFunction) ?? string.Empty }
			};

			return Str.Script.View.Format().With(values); 
		}

		public string Format(UserDefinedFunction udf)
		{
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Str.Catalog, udf.Parent.Name },
				{ Str.Schema, udf.Schema },
				{ Str.Udf, udf.Name },
				{ Str.Body, udf.TextBody },
				{ Str.Header, udf.ScriptHeader(true) },
				{ Str.Needs, GetNeeds(udf, DatabaseObjectTypes.StoredProcedure | DatabaseObjectTypes.UserDefinedFunction | DatabaseObjectTypes.View) ?? string.Empty }
			};

			switch (udf.FunctionType)
			{
				case UserDefinedFunctionType.Inline:
					values["funcType"] = "inline";
					break;
				case UserDefinedFunctionType.Table:
					values["funcType"] = "multi";
					break;
				default:
					values["funcType"] = "scalar";
					break;
			}

			return Str.Script.UserDefinedFunction.Format().With(values); 
		}

		public string Format(StoredProcedure sproc)
		{
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Str.Catalog, sproc.Parent.Name },
				{ Str.Schema, sproc.Schema },
				{ Str.Sproc, sproc.Name },
				{ Str.Body, NormalizeSprocBody(sproc.TextBody) },
				{ Str.Header, sproc.ScriptHeader(true) },
				{ Str.Needs, GetNeeds(sproc, DatabaseObjectTypes.StoredProcedure | DatabaseObjectTypes.UserDefinedFunction | DatabaseObjectTypes.View) ?? string.Empty }
			};

			values["parameters"] = string.Join(", ", sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = "));

			return Str.Script.Sproc.Format().With(values); 
		}

		private static string NormalizeSprocBody(string body)
		{
			body = PatternLineEndings.Replace(body.Trim(), LineBreakTab);
			if (body.StartsWith(Str.Begin, StringComparison.CurrentCultureIgnoreCase) == false)
				body = Str.Begin + LineBreakTab + body + Environment.NewLine + Str.End;

			return body;
		}

		private string GetNeeds(SqlSmoObject sqlSmoObject, DatabaseObjectTypes requestedDependencyTypes)
		{
			var dependencyTypeList = GetDependencyTypeList(requestedDependencyTypes);

			var dependencies = _server.GetDependencies(sqlSmoObject)
				.Where(x => 
					x.IsRootNode == false && 
					dependencyTypeList.Contains(x.Urn.Type) && 
					InSameDatabase(sqlSmoObject, x)
				)
				.Select(x => x.Urn.GetNameForType(x.Urn.Type))
				.ToList();

			if (dependencies.Count == 0)
				return string.Empty;

			return "--:: need " + string.Join(", ", dependencies) + Environment.NewLine;
		}

		private static bool InSameDatabase(SqlSmoObject sqlSmoObject, DependencyNode x)
		{
			return 0 == string.Compare(
				x.Urn.GetNameForType(Str.Database), 
				sqlSmoObject.Urn.GetNameForType(Str.Database), 
				StringComparison.InvariantCultureIgnoreCase);
		}

		private static IEnumerable<string> GetDependencyTypeList(DatabaseObjectTypes requestedDependencyTypes)
		{
			if (requestedDependencyTypes.HasFlag(DatabaseObjectTypes.StoredProcedure))
				yield return "StoredProcedure";
			
			if (requestedDependencyTypes.HasFlag(DatabaseObjectTypes.UserDefinedFunction))
				yield return "UserDefinedFunction";
			
			if (requestedDependencyTypes.HasFlag(DatabaseObjectTypes.View))
				yield return "View";
			
			if (requestedDependencyTypes.HasFlag(DatabaseObjectTypes.Table))
				yield return "Table";
		}

		private readonly IServer _server;
		private static readonly string 
			DoubleLineBreak = Environment.NewLine + Environment.NewLine,
			LineBreakTab = Environment.NewLine + '\t'
		;
		private static readonly Regex PatternLineEndings = new Regex(@"[\t ]*\r?\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

		private static class Str
		{
			public const string
				Database = "Database",
				Catalog = "catalog",
				Schema = "schema",
				Version = "version",
				Body = "body",
				Header = "header",
				Needs = "needs",
				Table = "Table",
				View = "View",
				Udf = "Udf",
				Sproc = "Sproc",
				Pre = "pre", Main = "main",
				Begin = "begin", End = "end"
			;

			public static class Script
			{
				public const string
					Sproc =
						@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateProcedure '{sproc}', '{schema}'
GO

--:: main
{header}
{body}

/*
	exec [{schema}].[{sproc}] {parameters}
*/",
					UserDefinedFunction =
						@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateFunction '{udf}', '{schema}', '{funcType}'
GO

--:: main
{header}
{body}

/*
	select {schema}.{udf}()
*/",
					View =
						@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateView '{view}', '{schema}'
GO

--:: main
{header}
{body}

/*
	select top 100 * from {view}
*/",
					Table =
						@"
{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
if(dbo.coreTableExists('{table}', '{schema}') = 0) begin

	{pre}

end

--:: main
if(dbo.coreGetTableVersion('{table}') < '{version}') begin

	{main}

	exec coreSetTableVersion '{table}', '{version}'
end

/*
	select top 100 * from [{schema}].[{table}]
*/"
					;
			}
		}
	}
}
