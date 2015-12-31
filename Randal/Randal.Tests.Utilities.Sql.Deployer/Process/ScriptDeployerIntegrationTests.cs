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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Enums;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.IO;
using Randal.Sql.Deployer.Process;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass, DeploymentItem(Test.Paths.ProjectA, Test.Paths.ProjectA)]
	public sealed class ScriptDeployerIntegrationTests : UnitTestBase<ScriptDeployerIntegrationThens>
	{
		protected override void OnSetup()
		{
			var config = new ScriptDeployerConfig
			{
				DatabaseLookup = "select 'model' [name]",
				ProjectsTableConfig = new ProjectsTableConfig
				{
					Database = "master", CreateTable = "print 'done'", Insert = "print 'done'",
					Read = "select '14.12.01.01' -- {0}"
				},
				ValidationFilterConfig = new ValidationFilterConfig()
			};

			Given.Config = config;

			Then.Logger = new Logger();
			Then.LogSink = new StringLogSink();
			Then.Logger.AddLogSink(Then.LogSink);
			var manager = new SqlConnectionManager(Given.Config.DatabaseLookup);
			manager.OpenConnection(".", "model");
			manager.BeginTransaction();

			Then.Manager = manager;
		}

		protected override void OnTeardown()
		{
			Then.Manager.RollbackTransaction();
			Then.Manager.Dispose();
			Then.Logger.Dispose();
		}

		[TestMethod, PositiveTest]
		public void ShouldReturnSuccessWhenCheckingCanUpgrade()
		{
			Given.Path = Test.Paths.ProjectA;

			When(CheckingCanUpgrade);

			Then.CanUpgrade.Should().BeTrue();
		}

		[TestMethod, PositiveTest]
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

			var loader = new ProjectLoader(Given.Path, parser, null, Then.Logger);
			loader.Load();
			var config = Given.Config ?? ScriptDeployerConfig.Default;
			
			var project = new Project(loader.Configuration, loader.AllScripts);

			Then.Deployer = new SqlServerDeployer(config, project, Then.Manager, Then.Logger);
		}

		private void CheckingCanUpgrade()
		{
			Then.CanUpgrade = Then.Deployer.CanProceed();
		}

		private void Deploying()
		{
			Then.DeployReturned = Then.Deployer.DeployScripts();
		}
	}

	public sealed class ScriptDeployerIntegrationThens
	{
		public ISqlConnectionManager Manager;
		public StringLogSink LogSink;
		public Logger Logger;
		public SqlServerDeployer Deployer;
		public Returned DeployReturned;
		public bool CanUpgrade;
	}
}