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
using System.Text.RegularExpressions;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Sql.Deployer.Scripts
{
	public sealed class ScriptParser : IScriptParser
	{
		public ScriptParser()
		{
			_keywordRuleLookup = new Dictionary<string, Func<string, IScriptBlock>>(StringComparer.InvariantCultureIgnoreCase);
		}

		public void AddRule(string keyword, Func<string, IScriptBlock> blockRule)
		{
			_keywordRuleLookup.Add(keyword, blockRule);
		}

		public void SetFallbackRule(Func<string, string, IScriptBlock> fallbackRule)
		{
			_fallbackRule = fallbackRule;
		}

		public bool HasFallbackRule
		{
			get { return _fallbackRule != null; }
		}

		public SourceScript Parse(string name, string text)
		{
			var blockMatches = ScriptBlockRegex.Matches(text);
			var blocks = new List<IScriptBlock>();

			foreach (Match match in blockMatches)
			{
				var keyword = match.Groups[MatchGroupKeyword].Value;
				var blockText = match.Groups[MatchGroupText].Value;
				Func<string, IScriptBlock> rule;

				if (_keywordRuleLookup.TryGetValue(keyword, out rule))
					blocks.Add(rule(blockText));
				else if (_fallbackRule != null)
					blocks.Add(_fallbackRule(keyword, blockText));
				else
					throw new InvalidOperationException("Unexpected block keyword '" + keyword + "' found and no fallback rule set.");
			}

			return new SourceScript(name, blocks);
		}

		public IReadOnlyList<string> RegisteredKeywords()
		{
			return _keywordRuleLookup.Keys.ToList().AsReadOnly();
		}

		private readonly Dictionary<string, Func<string, IScriptBlock>> _keywordRuleLookup;
		private Func<string, string, IScriptBlock> _fallbackRule;

		private const string
			MatchGroupKeyword = "keyword",
			MatchGroupText = "text"
			;

		private static readonly Regex ScriptBlockRegex = new Regex(@"--::[\s\t]*(?<keyword>[\w\d]+)(?<text>.+?)(?=--::|$)",
			RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase |
			RegexOptions.ExplicitCapture | RegexOptions.Singleline);
	}
}