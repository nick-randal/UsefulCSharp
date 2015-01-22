using System;
using System.Collections.Generic;

namespace Randal.Core.T4
{
	public static class CodeGeneratorExtensions
	{
		public static IReadOnlyList<string> ToCodeLines(this IReadOnlyList<DbCodeDefinition> codes)
		{
			var lines = new List<string>();

			try
			{
				foreach (var code in codes)
				{
					lines.Add(FormattedAttribute(code));
					lines.Add(FormattedDefinition(code));
					lines.Add(string.Empty);
				}

				if (lines.Count > 2)
				{
					lines.RemoveAt(lines.Count - 1);
					lines[lines.Count - 1] = lines[lines.Count - 1].TrimEnd(',');
				}
			}
			catch (Exception ex)
			{
				lines.Add(CommentBegin);
				lines.Add(ex.ToString());
				lines.Add(CommendEnd);
			}

			return lines.AsReadOnly();
		}

		private static string FormattedDefinition(DbCodeDefinition code)
		{
			return string.Format(EnumDefinition2Args, code.NameAsCSharpProperty, code.Code);
		}

		private static string FormattedAttribute(DbCodeDefinition code)
		{
			return string.Format(DisplayAttribute2Args, code.DisplayName, code.Description);
		}

		private const string 
			CommentBegin = "/*",
			CommendEnd = "*/",
			EnumDefinition2Args = "{0} = {1},",
			DisplayAttribute2Args = @"[Display(Name = ""{0}"", Description = ""{1}"")]"
		;
	}
}