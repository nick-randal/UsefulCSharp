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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class SourceScriptTests : UnitTestBase<SourceScriptThens>
	{
		protected override void OnSetup()
		{
			Given.Name = "UnitTest";
			Given.BlockList = new List<IScriptBlock>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveNameWithoutExtensionWhenCreating()
		{
			Given.Name = "NameWithExtension.Sql";

			When(Creating);

			Then.Script.Name.Should().Be("NameWithExtension");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveZeroScriptBlocksAfterInstantiationGivenEmptyList()
		{
			When(Creating);

			Then.Script.ScriptBlocks.Should().BeEmpty();
			Then.Script.Name.Should().Be("UnitTest");
			Then.Script.IsValid.Should().BeFalse();
			Then.Configuration.Should().BeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveAllValidBlocksWhenValidatingSourceScriptGivenValidBlocks()
		{
			Given.BlockList.AddRange(
				new List<IScriptBlock>
				{
					new IgnoreScriptBlock("use Test1"),
					new CatalogBlock("Test1, Test2"),
					new OptionsBlock("{ timeout: 45, useTransaction: false}"),
					new SqlCommandBlock("pre", "select 1", SqlScriptPhase.Pre),
					new SqlCommandBlock("main", "select 2", SqlScriptPhase.Main),
					new SqlCommandBlock("post", "select 3", SqlScriptPhase.Post)
				});

			When(Validating, GettingCatalogPatterns, GettingConfiguration);

			Then.Script.ScriptBlocks.Should().HaveCount(6);
			Then.Messages.Should().HaveCount(0);
			Then.Script.IsValid.Should().BeTrue();
			Then.CatalogPatterns.Should().HaveCount(2);
			Then.Configuration.Should().NotBeNull();
			Then.Configuration.Settings.Timeout.Should().Be(45);
			Then.Script.HasSqlScriptPhase(SqlScriptPhase.Pre).Should().BeTrue();
			Then.Script.HasSqlScriptPhase(SqlScriptPhase.Main).Should().BeTrue();
			Then.Script.HasSqlScriptPhase(SqlScriptPhase.Post).Should().BeTrue();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveDefaultConfigurationWhenValidatingGivenNoConfigurationBlock()
		{
			Given.BlockList.AddRange(new List<IScriptBlock> {new CatalogBlock("Companies, Employees")});

			When(Validating, GettingConfiguration);

			Then.Script.ScriptBlocks.Should().HaveCount(2);
			Then.Messages.Should().HaveCount(0);
			Then.Script.IsValid.Should().BeTrue();
			Then.Configuration.Should().NotBeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldIndicateInvalidWhenValidatingGivenSqlBlocksWithNoCatalogBlock()
		{
			Given.BlockList.AddRange(
				new List<IScriptBlock>
				{
					new SqlCommandBlock("pre", "select 1", SqlScriptPhase.Pre)
				});

			When(Validating);

			Then.Script.IsValid.Should().BeFalse();
			Then.Messages.Should().HaveCount(1);
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnNullWhenGettingCatalogPatternsGivenNoCatalogBlock()
		{
			When(Validating, GettingCatalogPatterns);

			Then.CatalogPatterns.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnListOfNeedsWhenGettingNeedsGivenValidNeedBlock()
		{
			Given.BlockList.Add(new NeedBlock("A, B"));

			When(Validating, GettingNeeds);

			Then.Needs.Should().HaveCount(2);
			Then.Needs[0].Should().Be("A");
			Then.Needs[1].Should().Be("B");
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnNullWhenGettingNeedsGivenNoNeeds()
		{
			When(Validating, GettingNeeds);

			Then.Needs.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldIndicateTrueWhenCheckingForPhase()
		{
			Given.BlockList.Add(new SqlCommandBlock("main", "select 1", SqlScriptPhase.Main));
			Given.CheckForPhase = SqlScriptPhase.Main;

			When(Validating, CheckingForPhase);

			Then.HasPhase.Should().BeTrue();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowExceptionWhenCreatingGivenNullName()
		{
			Given.Name = null;

			WhenLastActionDeferred(Creating);

			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnTextWhenRequestingSqlScriptPhase()
		{
			Given.BlockList.Add(new SqlCommandBlock("main", "select 1", SqlScriptPhase.Main));
			Given.RequestedPhase = SqlScriptPhase.Main;

			When(Validating, RequestingPhase);

			Then.Text.Should().Be("select 1");
		}

		[TestMethod, PositiveTest]
		public void ShouldIndicateTrueWhenCheckingIfExecutedGivenPreviouslyRequestedBlock()
		{
			Given.BlockList.Add(new SqlCommandBlock("main", "select 1", SqlScriptPhase.Main));
			Given.RequestedPhase = SqlScriptPhase.Main;

			When(Validating, RequestingPhase, CheckingIfExecuted);

			Then.WasExecuted.Should().BeTrue();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowExceptionWhenRequestingPhaseGivenPhaseDoesNotExist()
		{
			Given.RequestedPhase = SqlScriptPhase.Main;

			WhenLastActionDeferred(Validating, RequestingPhase);

			ThenLastAction.ShouldThrow<InvalidOperationException>();
		}

		private void CheckingIfExecuted()
		{
			Then.WasExecuted = Then.Script.HasPhaseExecuted(Given.RequestedPhase);
		}

		private void RequestingPhase()
		{
			Then.Text = Then.Script.RequestSqlScriptPhase(Given.RequestedPhase);
		}

		private void CheckingForPhase()
		{
			Then.HasPhase = Then.Script.HasSqlScriptPhase(Given.CheckForPhase);
		}

		private void GettingNeeds()
		{
			Then.Needs = Then.Script.GetNeeds();
		}

		private void GettingCatalogPatterns()
		{
			Then.CatalogPatterns = Then.Script.GetCatalogPatterns();
		}

		private void GettingConfiguration()
		{
			Then.Configuration = Then.Script.GetConfiguration();
		}

		private void Validating()
		{
			var messages = new List<string>();
			Then.Script.Validate(messages);
			Then.Messages = messages;
		}

		protected override void Creating()
		{
			Then.Script = new SourceScript(Given.Name, Given.BlockList);
		}
	}

	public sealed class SourceScriptThens
	{
		public SourceScript Script;
		public OptionsBlock Configuration;
		public IReadOnlyList<string> CatalogPatterns, Messages, Needs;
		public bool HasPhase, WasExecuted;
		public string Text;
	}
}