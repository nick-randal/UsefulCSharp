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
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	public sealed partial class ScriptParserTests
	{
		private class ParserBuilder
		{
			private readonly List<Tuple<string, Func<string, IScriptBlock>>> _rules;
			private Func<string, string, IScriptBlock> _fallbackRule;

			public ParserBuilder()
			{
				_rules = new List<Tuple<string, Func<string, IScriptBlock>>>();
			}

			public ParserBuilder WithRule(string keyword, Func<string, IScriptBlock> blockRule)
			{
				_rules.Add(new Tuple<string, Func<string, IScriptBlock>>(keyword, blockRule));
				return this;
			}

			// ReSharper disable once UnusedMethodReturnValue.Local
			public ParserBuilder WithFallbackRule(Func<string, string, IScriptBlock> fallbackRule)
			{
				_fallbackRule = fallbackRule;
				return this;
			}

			private ScriptParser Build()
			{
				var parser = new ScriptParser();

				_rules.ForEach(r => parser.AddRule(r.Item1, r.Item2));
				if (_fallbackRule != null)
					parser.SetFallbackRule(_fallbackRule);

				return parser;
			}

			public static implicit operator ScriptParser(ParserBuilder builder)
			{
				return builder.Build();
			}
		}
	}
}