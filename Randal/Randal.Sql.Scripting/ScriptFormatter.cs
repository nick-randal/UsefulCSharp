// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
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
using Microsoft.SqlServer.Management.Smo;
using Randal.Core.Strings;
using Const = Randal.Sql.Scripting.ScriptFormatterConstants;

namespace Randal.Sql.Scripting
{
	public interface IScriptFormatter
	{
		string Format(ScriptableObject scriptableObject);
		string Format(StoredProcedure sproc);
		string Format(UserDefinedFunction sproc);
		string Format(View sproc);
		string Format(Table sproc);
	}

	public sealed class ScriptFormatter : IScriptFormatter
	{
		public string Format(ScriptableObject scriptableObject)
		{
			if (scriptableObject.IsSproc)
				return Format(scriptableObject.Sproc);

			if (scriptableObject.IsUdf)
				return Format(scriptableObject.Udf);

			if (scriptableObject.IsView)
				return Format(scriptableObject.View);

			if (scriptableObject.IsTable)
				return Format(scriptableObject.Table);

			throw new NotSupportedException("No support has been defined for type '" + scriptableObject.SchemaObject.GetType().FullName + "'.");
		}

		public string Format(Table table)
		{
			var server = table.CreateWrapper();
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Const.Catalog, table.Parent.Name },
				{ Const.Schema, table.Schema },
				{ Const.Name, table.Name },
				{ Const.NameEscaped, table.Name.EscapeName() },
				{ Const.Pre, table.FormatPreSection() },
				{ Const.Main, table.FormatMainSection() },
				{ Const.Version, 1.ToVersionToday() },
				{ Const.Needs, GetNeeds(server, table, DatabaseObjectTypes.UserDefinedFunction) ?? string.Empty }
			};

			return Const.Script.Table.Format().With(values);
		}

		public string Format(View view)
		{
			var server = view.CreateWrapper();
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Const.Catalog, view.Parent.Name },
				{ Const.Schema, view.Schema },
				{ Const.Name, view.Name },
				{ Const.NameEscaped, view.Name.EscapeName() },
				{ Const.Body, view.TextBody },
				{ Const.Header, view.ScriptHeader(true) },
				{ Const.Needs, GetNeeds(server, view, DatabaseObjectTypes.UserDefinedFunction) ?? string.Empty }
			};

			return Const.Script.View.Format().With(values); 
		}

		public string Format(UserDefinedFunction udf)
		{
			var server = udf.CreateWrapper();
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Const.Catalog, udf.Parent.Name },
				{ Const.Schema, udf.Schema },
				{ Const.Name, udf.Name },
				{ Const.NameEscaped, udf.Name.EscapeName() },
				{ Const.Body, udf.TextBody },
				{ Const.Header, udf.ScriptHeader(true) },
				{ Const.Needs, GetNeeds(server, udf, DatabaseObjectTypes.StoredProcedure | DatabaseObjectTypes.UserDefinedFunction | DatabaseObjectTypes.View) ?? string.Empty }
			};

			switch (udf.FunctionType)
			{
				case UserDefinedFunctionType.Inline:
					values[Const.FuncType] = Const.FuncInline;
					break;
				case UserDefinedFunctionType.Table:
					values[Const.FuncType] = Const.FuncMulti;
					break;
				default:
					values[Const.FuncType] = Const.FuncScalar;
					break;
			}

			return Const.Script.UserDefinedFunction.Format().With(values); 
		}

		public string Format(StoredProcedure sproc)
		{
			var server = sproc.CreateWrapper();
			var values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
			{
				{ Const.Catalog, sproc.Parent.Name },
				{ Const.Schema, sproc.Schema },
				{ Const.Name, sproc.Name },
				{ Const.NameEscaped, sproc.Name.EscapeName() },
				{ Const.Body, sproc.TextBody.NormalizeSprocBody() },
				{ Const.Header, sproc.ScriptHeader(true) },
				{ Const.Needs, GetNeeds(server, sproc, DatabaseObjectTypes.StoredProcedure | DatabaseObjectTypes.UserDefinedFunction | DatabaseObjectTypes.View) ?? string.Empty }
			};

			values["parameters"] = string.Join(", ", sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = "));

			return Const.Script.Sproc.Format().With(values); 
		}

		private static string GetNeeds(IServer server, SqlSmoObject sqlSmoObject, DatabaseObjectTypes requestedDependencyTypes)
		{
			var dependencyTypeList = requestedDependencyTypes.GetDependencyTypeList();

			var dependencies = server.GetDependencies(sqlSmoObject)
				.Where(x => 
					x.IsRootNode == false && 
					dependencyTypeList.Contains(x.Urn.Type) && 
					sqlSmoObject.InSameDatabase(x)
				)
				.Select(x => x.Urn.GetAttribute("Schema") + '.' + x.Urn.GetNameForType(x.Urn.Type) )
				.ToList();

			if (dependencies.Count == 0)
				return string.Empty;

			return "--:: need " + string.Join(", ", dependencies) + Environment.NewLine;
		}
	}
}
