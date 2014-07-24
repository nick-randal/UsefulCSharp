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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Enums;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Randal.Sql.Deployer.IO;
using Randal.Sql.Deployer.Process;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass, DeploymentItem(Test.Paths.ProjectA, Test.Paths.ProjectA)]
	public sealed class ScriptDeployerIntegrationTests : BaseUnitTest<ScriptDeployerIntegrationThens>
	{
		protected override void OnSetup()
		{
			Then.Logger = new StringLogger();
			var manager = new SqlConnectionManager();
			manager.OpenConnection(".", "master");
			manager.BeginTransaction();

			Then.Manager = manager;
		}

		[TestMethod]
		public void ShouldReturnSuccessWhenCheckingCanUpgrade()
		{
			Given.Path = Test.Paths.ProjectA;

			When(CheckingCanUpgrade);

			Then.CanUpgrade.Should().BeTrue();
		}

		[TestMethod]
		public void ShouldReturnSuccessWhenDeployingScripts()
		{
			Given.Path = Test.Paths.ProjectA;

			When(Deploying);

			Then.DeployReturned.Should().Be(Returned.Success);
		}

		protected override void Creating()
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

	public sealed class ScriptDeployerIntegrationThens : IDisposable
	{
		public ISqlConnectionManager Manager;
		public StringLogger Logger;
		public ScriptDeployer Deployer;
		public Returned DeployReturned;
		public bool CanUpgrade;

		public void Dispose()
		{
			Manager.RollbackTransaction();
			Manager.Dispose();
			Logger.Dispose();
		}
	}
}