using System;
using System.Collections.Generic;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	public sealed partial class ScriptParserTests
	{
		class ParserBuilder
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
				if(_fallbackRule != null)
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