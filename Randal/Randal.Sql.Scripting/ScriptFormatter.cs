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

			throw new InvalidOperationException("Not supported.");
		}

		public string Format(Table table)
		{
			var values = new Dictionary<string, object>
			{
				{ "catalog", table.Parent.Name },
				{ "schema", table.Schema },
				{ "table", table.Name },
				{ "pre", FormatTablePreSection(table) },
				{ "main", FormatTableMainSection(table) },
				{ "version", 1.ToVersionToday() },
				{ "needs", GetNeeds(table, DatabaseObjectTypes.UserDefinedFunction) ?? string.Empty }
			};

			return TableScript.Format().With(values);
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
			var values = new Dictionary<string, object>
			{
				{ "catalog", view.Parent.Name },
				{ "schema", view.Schema },
				{ "view", view.Name },
				{ "body", view.TextBody },
				{ "header", view.ScriptHeader(true) },
				{ "needs", GetNeeds(view, DatabaseObjectTypes.UserDefinedFunction) ?? string.Empty }
			};

			return ViewScript.Format().With(values); 
		}

		public string Format(UserDefinedFunction udf)
		{
			var values = new Dictionary<string, object>
			{
				{ "catalog", udf.Parent.Name },
				{ "schema", udf.Schema },
				{ "udf", udf.Name },
				{ "body", udf.TextBody },
				{ "header", udf.ScriptHeader(true) },
				{ "needs", GetNeeds(udf, DatabaseObjectTypes.StoredProcedure | DatabaseObjectTypes.UserDefinedFunction | DatabaseObjectTypes.View) ?? string.Empty }
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

			return UserDefinedFunctionScript.Format().With(values); 
		}

		public string Format(StoredProcedure sproc)
		{
			var values = new Dictionary<string, object>
			{
				{ "catalog", sproc.Parent.Name },
				{ "schema", sproc.Schema },
				{ "sproc", sproc.Name },
				{ "body", NormalizeSprocBody(sproc.TextBody) },
				{ "header", sproc.ScriptHeader(true) },
				{ "needs", GetNeeds(sproc, DatabaseObjectTypes.StoredProcedure | DatabaseObjectTypes.UserDefinedFunction | DatabaseObjectTypes.View) ?? string.Empty }
			};

			values["parameters"] = string.Join(", ", sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = "));

			return SprocScript.Format().With(values); 
		}

		private static string NormalizeSprocBody(string body)
		{
			body = PatternLineEndings.Replace(body.Trim(), LineBreakTab);
			if (body.StartsWith("begin", StringComparison.CurrentCultureIgnoreCase) == false)
				body = "begin" + LineBreakTab + body + Environment.NewLine + "end";

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
				x.Urn.GetNameForType("Database"), 
				sqlSmoObject.Urn.GetNameForType("Database"), 
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

		private const string 
			SprocScript =
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
			UserDefinedFunctionScript =
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
			ViewScript =
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
			TableScript =
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
