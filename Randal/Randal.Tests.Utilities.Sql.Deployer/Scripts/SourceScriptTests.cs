using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class SourceScriptTests : BaseUnitTest<SourceScriptThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Given.Name = "UnitTest";
			Given.BlockList = new List<IScriptBlock>();
		}

		[TestMethod]
		public void ShouldHaveNameWithoutExtensionWhenCreating()
		{
			Given.Name = "NameWithExtension.Sql";

			When(Creating);

			Then.Script.Name.Should().Be("NameWithExtension");
		}

		[TestMethod]
		public void ShouldHaveZeroScriptBlocksAfterInstantiationGivenEmptyList()
		{
			When(Creating);

			Then.Script.ScriptBlocks.Should().BeEmpty();
			Then.Script.Name.Should().Be("UnitTest");
			Then.Script.IsValid.Should().BeFalse();
			Then.Configuration.Should().BeNull();
		}

		[TestMethod]
		public void ShouldHaveAllValidBlocksWhenValidatingSourceScriptGivenValidBlocks()
		{
			Given.BlockList.AddRange(
				new List<IScriptBlock>
				{
					new IgnoreScriptBlock("use Coupon"),
					new CatalogBlock("TCPLP, Coupon"),
					new ConfigurationBlock("{ timeout: 45, useTransaction: false}"),
					new SqlCommandBlock("pre", "select 1", SqlScriptPhase.Pre),
					new SqlCommandBlock("main", "select 2", SqlScriptPhase.Main),
					new SqlCommandBlock("post", "select 3", SqlScriptPhase.Post)
				});

			When(Creating, Validating, GettingCatalogPatterns, GettingConfiguration);

			Then.Script.ScriptBlocks.Should().HaveCount(6);
			Then.Messages.Should().HaveCount(0);
			Then.Script.IsValid.Should().BeTrue();
			Then.CatalogPatterns.Should().HaveCount(2);
			Then.Configuration.Should().NotBeNull();
			Then.Configuration.Settings.Timeout.Should().Be(45);
			Then.Configuration.Settings.UseTransaction.Should().BeFalse();
			Then.Script.HasSqlScriptPhase(SqlScriptPhase.Pre).Should().BeTrue();
			Then.Script.HasSqlScriptPhase(SqlScriptPhase.Main).Should().BeTrue();
			Then.Script.HasSqlScriptPhase(SqlScriptPhase.Post).Should().BeTrue();
		}

		[TestMethod]
		public void ShouldHaveDefaultConfigurationWhenValidatingGivenNoConfigurationBlock()
		{
			Given.BlockList.AddRange(new List<IScriptBlock> { new CatalogBlock("TCPLP, Coupon") });

			When(Creating, Validating, GettingConfiguration);

			Then.Script.ScriptBlocks.Should().HaveCount(2);
			Then.Messages.Should().HaveCount(0);
			Then.Script.IsValid.Should().BeTrue();
			Then.Configuration.Should().NotBeNull();
		}

		[TestMethod]
		public void ShouldIndicateInvalidWhenValidatingGivenSqlBlocksWithNoCatalogBlock()
		{
			Given.BlockList.AddRange(
				new List<IScriptBlock>
				{
					new SqlCommandBlock("pre", "select 1", SqlScriptPhase.Pre)
				});

			When(Creating, Validating);

			Then.Script.IsValid.Should().BeFalse();
			Then.Messages.Should().HaveCount(1);
		}

		[TestMethod]
		public void ShouldReturnNullWhenGettingCatalogPatternsGivenNoCatalogBlock()
		{
			When(Creating, Validating, GettingCatalogPatterns);

			Then.CatalogPatterns.Should().BeNull();
		}

		[TestMethod]
		public void ShouldReturnListOfNeedsWhenGettingNeedsGivenValidNeedBlock()
		{
			Given.BlockList.Add(new NeedBlock("A, B"));

			When(Creating, Validating, WhenGettingNeeds);

			Then.Needs.Should().HaveCount(2);
			Then.Needs[0].Should().Be("A.sql");
			Then.Needs[1].Should().Be("B.sql");
		}

		[TestMethod]
		public void ShouldReturnNullWhenGettingNeedsGivenNoNeeds()
		{
			When(Creating, Validating, WhenGettingNeeds);

			Then.Needs.Should().BeNull();
		}

		[TestMethod]
		public void ShouldIndicateTrueWhenCheckingForPhase()
		{
			Given.BlockList.Add(new SqlCommandBlock("main", "select 1", SqlScriptPhase.Main));
			Given.CheckForPhase = SqlScriptPhase.Main;

			When(Creating, Validating, CheckingForPhase);

			Then.HasPhase.Should().BeTrue();
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullName()
		{
			Given.Name = null;

			When(Creating);
		}

		[TestMethod]
		public void ShouldReturnTextWhenRequestingSqlScriptPhase()
		{
			Given.BlockList.Add(new SqlCommandBlock("main", "select 1", SqlScriptPhase.Main));
			Given.RequestedPhase = SqlScriptPhase.Main;

			When(Creating, Validating, RequestingPhase);

			Then.Text.Should().Be("select 1");
		}

		private void RequestingPhase()
		{
			Then.Text = Then.Script.RequestSqlScriptPhase(Given.RequestedPhase);
		}

		private void CheckingForPhase()
		{
			Then.HasPhase = Then.Script.HasSqlScriptPhase(Given.CheckForPhase);
		}

		private void WhenGettingNeeds()
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
			Then.Messages = Then.Script.Validate();	
		}

		private void Creating()
		{
			Then.Script = new SourceScript(Given.Name, Given.BlockList);
		}
	}

	public sealed class SourceScriptThens
	{
		public SourceScript Script;
		public ConfigurationBlock Configuration;
		public IReadOnlyList<string> CatalogPatterns, Messages, Needs;
		public bool HasPhase;
		public string Text;
	}
}