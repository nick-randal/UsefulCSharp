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
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Enums;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Randal.Sql.Deployer.IO;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.IO
{
	[TestClass, DeploymentItem("TestFiles", "TestFiles")]
	public sealed class ProjectLoaderTests : UnitTestBase<ProjectLoaderTests.Thens>
	{
		protected override void OnSetup()
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

			Given.Parser = parser;
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValuesSet_WhenCreating_GivenValidValues()
		{
			Given.ProjectPath = @"c:\some folder";
			When(Creating);
			Then.Target.ProjectPath.Should().Be(@"c:\some folder");
		}

		[TestMethod, NegativeTest]
		public void ShouldIndicateInvalidPath_WhenLoading_GivneInvalidPath()
		{
			Given.ProjectPath = @"c:\some folder";
			When(Loading);
			Then.Has.Should().Be(Returned.Failure, "because the path is invalid");
		}

		[TestMethod, NegativeTest]
		public void ShouldIndicateMissingConfiguration_WhenLoading_GivenWrongFolder()
		{
			Given.ProjectPath = @".";
			When(Loading);
			Then.Has.Should().Be(Returned.Failure, "because the config file does not exist.");
		}

		[TestMethod, PositiveTest]
		public void ShouldIndicateAmbiguousConfiguration_WhenLoading_GivenMultipleConfigs()
		{
			Given.ProjectPath = @".\TestFiles\ProjectB";

			When(Loading);

			Then.Has.Should().Be(Returned.Failure);
			Then.Has.Should().Be(Returned.Failure, "because there are multiple config files.");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveConfigurationAndSourceFiles_WhenLoading_GivenValidFiles()
		{
			Given.ProjectPath = @".\TestFiles\ProjectA";

			When(Loading);

			Then.Has.Should().Be(Returned.Success);
			Then.Target.Configuration.Should().NotBeNull();
			Then.Target.Configuration.Project.Should().Be("Conmigo");
			Then.Target.Configuration.Version.Should().Be("14.12.01.02");
			Then.Target.Configuration.PriorityScripts.Should().HaveCount(1);
			Then.Target.Configuration.Vars.Should().HaveCount(2);
			Then.Target.AllScripts.Should().HaveCount(3);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveConfiguration_WhenLoading_GivenValidConfiguration()
		{
			Given.ProjectPath = @".\TestFiles\ProjectC";

			When(Loading);

			Then.Has.Should().Be(Returned.Success);
			Then.Target.Configuration.Should().NotBeNull();
			Then.Target.Configuration.Project.Should().Be("Conmigo");
			Then.Target.Configuration.Version.Should().Be("14.12.01.02");
			Then.Target.Configuration.PriorityScripts.Should().HaveCount(2);
			Then.Target.Configuration.Vars.Should().HaveCount(2);
		}

		protected override void Creating()
		{
			Then.Logger = new Logger();
			Then.LogSink = new StringLogSink();
			Then.Logger.AddLogSink(Then.LogSink);
			Then.Target = new ProjectLoader(Given.ProjectPath, scriptParser: Given.Parser, logger: Then.Logger);
		}

		private void Loading()
		{
			Then.Has = Then.Target.Load();
		}

		public sealed class Thens : IDisposable
		{
			public ProjectLoader Target;
			public Returned Has;
			public Logger Logger;
			public StringLogSink LogSink;

			public void Dispose()
			{
				if (Logger != null)
					Logger.Dispose();
			}
		}
	}
}