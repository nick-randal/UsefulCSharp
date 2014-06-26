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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Enums;
using Randal.Logging;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.IO;
using Randal.Utilities.Sql.Deployer.Process;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass, DeploymentItem(Test.Paths.ProjectA, Test.Paths.ProjectA)]
	public sealed class ScriptDeployerIntegrationTests : BaseUnitTest<ScriptDeployerIntegrationThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			Then.Logger = new StringLogger();
			var manager = new SqlConnectionManager();
			manager.OpenConnection(".", "master");
			manager.BeginTransaction();

			Then.Manager = manager;
		}

		[TestCleanup]
		public void Teardown()
		{
			Then.Manager.RollbackTransaction();
			Then.Manager.Dispose();
			Then.Logger.Dispose();
		}

		[TestMethod]
		public void ShouldReturnSuccessWhenCheckingCanUpgrade()
		{
			Given.Path = Test.Paths.ProjectA;

			When(Creating, CheckingCanUpgrade);

			Then.CanUpgrade.Should().BeTrue();
		}

		[TestMethod]
		public void ShouldReturnSuccessWhenDeployingScripts()
		{
			Given.Path = Test.Paths.ProjectA;

			When(Creating, Deploying);

			Then.DeployReturned.Should().Be(Returned.Success);
		}

		private void Creating()
		{
			var parser = new ScriptParser();
			parser.AddRule("catalog", txt => new CatalogBlock(txt));
			parser.AddRule("options", txt => new OptionsBlock(txt));
			parser.AddRule("need", txt => new NeedBlock(txt));
			parser.AddRule("ignore", txt => new IgnoreScriptBlock(txt));
			parser.AddRule("pre", txt => new SqlCommandBlock("pre", txt, SqlScriptPhase.Pre));
			parser.AddRule("main", txt => new SqlCommandBlock("main", txt, SqlScriptPhase.Main));
			parser.AddRule("post", txt => new SqlCommandBlock("post", txt, SqlScriptPhase.Post));
			parser.SetFallbackRule((kw, txt) => new UnexpectedBlock(kw, txt));

			var loader = new ProjectLoader(Given.Path, parser, Then.Logger);
			loader.Load();
			var project = new Project(loader.Configuration, loader.AllScripts);

			Then.Deployer = new ScriptDeployer(project, Then.Manager, Then.Logger);
		}

		private void CheckingCanUpgrade()
		{
			Then.CanUpgrade = Then.Deployer.CanUpgrade();
		}

		private void Deploying()
		{
			Then.DeployReturned = Then.Deployer.DeployScripts();
		}
	}

	public sealed class ScriptDeployerIntegrationThens
	{
		public ISqlConnectionManager Manager;
		public StringLogger Logger;
		public ScriptDeployer Deployer;
		public Returned DeployReturned;
		public bool CanUpgrade;
	}
}