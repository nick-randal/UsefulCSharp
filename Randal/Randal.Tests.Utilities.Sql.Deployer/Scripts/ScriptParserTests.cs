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
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed partial class ScriptParserTests : UnitTestBase<ScriptParserThens>
	{
		protected override void OnSetup()
		{
			GivenParser = new ParserBuilder();
		}

		[TestMethod, PositiveTest]
		public void ShouldAcceptParseBlocks_WhenCreating_GivenValidRules()
		{
			GivenParser.WithRule("pre", text => new SqlCommandBlock("pre", text, SqlScriptPhase.Pre));

			When(Creating);

			Then.Parser.RegisteredKeywords().Should().HaveCount(1);
			Then.Parser.RegisteredKeywords().First().Should().Be("pre");
		}

		[TestMethod, PositiveTest]
		public void ShouldCreateSourceScript_WhenParsing_GivenValidText()
		{
			GivenParser
				.WithRule("pre", text => new SqlCommandBlock("pre", text, SqlScriptPhase.Pre))
				.WithRule("main", text => new SqlCommandBlock("main", text, SqlScriptPhase.Main))
				.WithRule("post", text => new SqlCommandBlock("post", text, SqlScriptPhase.Post));

			Given.Text = "--:: pre\nselect 1\n--:: main\nselect 2\n--:: post\nselect 3\nGO";

			When(Parsing);

			Then.SourceScript.ScriptBlocks.Should().HaveCount(3);
			Then.SourceScript.ScriptBlocks[2].Keyword.Should().Be("post");
			Then.SourceScript.ScriptBlocks[2].Text.Should().Be("select 3\nGO");
			Then.SourceScript.ScriptBlocks[2].As<ISqlCommandBlock>().Phase.Should().Be(SqlScriptPhase.Post);
		}

		[TestMethod, PositiveTest]
		public void ShouldProcessFallbackRule_WhenProcessingUnknownKeywords()
		{
			GivenParser.WithFallbackRule((kw, text) => new UnexpectedBlock(kw, text));
			Given.Text = "--:: unknown\nselect 1\nGO\n";

			When(Parsing);

			Then.SourceScript.ScriptBlocks.Should().HaveCount(1);

			var thenFirstBlock = Then.SourceScript.ScriptBlocks.First();
			thenFirstBlock.Keyword.Should().Be("unknown");
			thenFirstBlock.IsValid.Should().BeFalse();
			thenFirstBlock.Text.Should().Be("select 1\nGO");
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenProcessingUnexpectedBlock()
		{
			Given.Text = "--:: unknown\nselect 1\nGO\n";

			WhenLastActionDeferred(Parsing);

			ThenLastAction.ShouldThrow<InvalidOperationException>();
		}

		protected override void Creating()
		{
			Then.Parser = GivenParser;
		}

		private void Parsing()
		{
			Then.SourceScript = Then.Parser.Parse("UnitTest", Given.Text);
		}

		private ParserBuilder GivenParser { get; set; }
	}

	public sealed class ScriptParserThens
	{
		public SourceScript SourceScript;
		public ScriptParser Parser;
	}
}