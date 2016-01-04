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
using Const = Randal.Sql.Scripting.ScriptFormatterConstants;

namespace Randal.Sql.Scripting
{
	public static class ScriptFormatterExtensions
	{
		private static readonly ScriptingOptions
			PrePhaseOptions = new ScriptingOptions
			{
				DriDefaults = true,
				Indexes = true,
				FullTextIndexes = true
			},

			MainPhaseOptions = new ScriptingOptions
			{
				PrimaryObject = false,

				DriChecks = true,
				DriForeignKeys = true,
				Triggers = true
			};

		public static string FormatPreSection(this Table table)
		{
			var sectionText = string.Join(Const.DoubleLineBreak, table.EnumScript(PrePhaseOptions).Select(x => x.Trim()));
			sectionText = Const.Patterns.LineEndings.Replace(sectionText.Trim(), Const.LineBreakTab);

			return sectionText;
		}

		public static string FormatMainSection(this Table table)
		{
			var main = string.Join(Const.DoubleLineBreak, table.EnumScript(MainPhaseOptions).Select(x => x.Trim()));
			main = Const.Patterns.LineEndings.Replace(main.Trim(), Const.LineBreakTab);

			return main.ReplaceCreateTrigger();
		}

		private static string ReplaceCreateTrigger(this string text)
		{
			return Const.Patterns.CreateTrigger.Replace(text, match =>
			{
				if (match.Success == false)
					throw new FormatException("Unexpected Create Trigger format.");

				var tableMatch = Const.Patterns.SchemaName.Match(match.Groups["table"].Value);

				if (tableMatch.Success == false)
					throw new FormatException("Unexpected table name format in create trigger.");

				var triggerMatch = Const.Patterns.SchemaName.Match(match.Groups["name"].Value);

				if (triggerMatch.Success == false)
					throw new FormatException("Unexpected trigger name format in create trigger.");

				return string.Format("exec coreCreateTrigger '{0}', '{1}', '{2}', '{3}', '{4}'",
					tableMatch.Groups["name"].Value,
					tableMatch.Groups["schema"].Value,
					triggerMatch.Groups["name"].Value,
					match.Groups["type"].Value.Trim(),
					match.Groups["body"].Value.Trim().Replace("'", "''")
				);
			});
		}

		public static string EscapeName(this string name)
		{
			return Const.Patterns.UnescapedSingleQuotes.Replace(name, Const.UnescapedSingleQuotesEvaluator);
		}

		public static string NormalizeSprocBody(this string body)
		{
			body = Const.Patterns.LineEndings.Replace(body.Trim(), Const.LineBreakTab);
			if (body.StartsWith(Const.Begin, StringComparison.CurrentCultureIgnoreCase) == false)
				body = Const.Begin + Const.LineBreakTab + body + Environment.NewLine + Const.End;

			return body;
		}

		public static bool InSameDatabase(this SqlSmoObject sqlSmoObject, DependencyNode x)
		{
			return 0 == string.Compare(
				x.Urn.GetNameForType(Const.Database),
				sqlSmoObject.Urn.GetNameForType(Const.Database),
				StringComparison.InvariantCultureIgnoreCase);
		}

		public static IEnumerable<string> GetDependencyTypeList(this DatabaseObjectTypes requestedDependencyTypes)
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

		public static IServer CreateWrapper(this Table table) { return new ServerWrapper(table.Parent.Parent); }
		public static IServer CreateWrapper(this View view) { return new ServerWrapper(view.Parent.Parent); }
		public static IServer CreateWrapper(this StoredProcedure sproc) { return new ServerWrapper(sproc.Parent.Parent); }
		public static IServer CreateWrapper(this UserDefinedFunction udf) { return new ServerWrapper(udf.Parent.Parent); }
	}
}
