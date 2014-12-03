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
using Sprache;
using Pattern = System.Tuple<System.Text.RegularExpressions.Regex, Randal.Sql.Deployer.Scripts.ScriptCheck, string>;

namespace Randal.Sql.Deployer.Scripts
{
	[Flags]
	public enum ScriptCheck
	{
		Passed = 0,
		Warning = 1,
		Failed = 2,
		Fatal = 4
	}

	public interface IScriptCheckerConsumer
	{
		ScriptCheck Validate(string input, out IEnumerable<string> messages);
	}

	public interface IScriptChecker : IScriptCheckerConsumer
	{
		void AddValidationPattern(string pattern, ScriptCheck shouldIssue);
	}

	public class ScriptChecker : IScriptChecker
	{
		public ScriptChecker()
		{
			_patterns = new List<Pattern>();
		}

		public void AddValidationPattern(string pattern, ScriptCheck shouldIssue)
		{
			_patterns.Add(
				new Pattern(new Regex(pattern, StandardOptions), shouldIssue, pattern)
			);
		}

		public ScriptCheck Validate(string input, out IEnumerable<string> messages)
		{
			var tempMessages = new List<string>();

			var sanitizedInput = SanitizeCode(input, tempMessages);
			if (sanitizedInput == null)
			{
				messages = tempMessages;
				return ScriptCheck.Fatal;
			}

			var validationState = EvaluatePatterns(input, sanitizedInput, tempMessages);

			messages = tempMessages;
			return validationState;
		}

		private ScriptCheck EvaluatePatterns(string orignalInput, string sanitizedInput, ICollection<string> tempMessages)
		{
			var validationState = ScriptCheck.Passed;
			
			foreach (var filter in _patterns)
			{
				var matches = filter.Item1.Matches(sanitizedInput);

				if (matches.Count == 0)
					continue;

				var lastIndex = 0;
				validationState |= filter.Item2;

				foreach (Match match in matches)
				{
					string text;
					int line;
					lastIndex = FindTextLocation(orignalInput, lastIndex, match.Value, out line, out text);
					tempMessages.Add(string.Format("{0}: Line {1}, found \"{2}\".", filter.Item2, line, text));
				}
			}

			return validationState;
		}

		private static int FindTextLocation(string orignalInput, int lastIndex, string match, out int line, out string text)
		{
			if (lastIndex < 0)
				lastIndex = 0;

			lastIndex = orignalInput.IndexOf(match, lastIndex, StringComparison.InvariantCulture);
			if (lastIndex == -1)
			{
				text = "<failed to find text>";
				line = -1;
				return -1;
			}

			line = orignalInput.Substring(0, lastIndex).Count(c => c == '\n') + 1;

			text = new string(
				orignalInput
					.Skip(lastIndex - 10)
					.Take(lastIndex + 10)
					.Select(c => c == '\r' || c == '\n' ? ' ' : c)
					.ToArray()
			);

			return lastIndex + 1;
		}

		private static string SanitizeCode(string input, ICollection<string> messages)
		{
			try
			{
				return string.Join(string.Empty, Sql.Parse(input).Where(s => s != string.Empty));
			}
			catch (ParseException pe)
			{
				Exception ex = pe;

				do
				{
					messages.Add(ex.Message);
					ex = ex.InnerException;
				} while (ex is ParseException);

				return null;
			}
		}

		private readonly List<Pattern> _patterns;

		private const RegexOptions StandardOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline;

		private static readonly Parser<char> 
			SimpleWhitespace = Parse.Chars(' ', '\t').Named("Space or Tab"),
			Newlines = Parse.Chars('\r', '\n').Named("CR or NL");

		private static readonly Parser<IEnumerable<char>> 
			SlCommentHead = Parse.Char('-').Repeat(2).Named("Single line comment start"),
			MlCommentHead = Parse.String("/*").Named("Multi-line comment start"),
			MlCommentTail = Parse.String("*/").Named("Multi-line comment end"),
			LineEnd = Newlines.Many().Named("End of line"),
			LineTerminator = Parse.Return("").End()
				.Or(LineEnd.End())
				.Or(LineEnd)
				.Named("LineTerminator")
		;

		private static readonly Parser<string> SingleLineComment =
			(
				from leadingWs in SimpleWhitespace.Many()
				from head in SlCommentHead.Text()
				from comment in Parse.AnyChar.Except(Newlines).Many().Text()
				from tail in LineTerminator
				select string.Empty
			)
			.Named("Single Line Comment");

		private static readonly Parser<string> MultiLineComment =
			(
				from leadingWs in SimpleWhitespace.Many()
				from head in MlCommentHead
				from comment in Parse.AnyChar.Except(MlCommentTail).Many().Text()
				from tail in MlCommentTail
				select string.Empty
			)
			.Named("Multi-line Comment");

		private static readonly Parser<string>
			Code = Parse.AnyChar.Except(SlCommentHead.Or(MlCommentHead)).Many().Text().Named("Code"),
			Comments = SingleLineComment.XOr(MultiLineComment).Named("Comments");

		private static readonly Parser<IEnumerable<string>> Sql = Comments.XOr(Code).XMany().End().Named("T-SQL");
	}
}
