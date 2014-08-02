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
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Randal.Sql.Deployer.IO;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.IO
{
	[TestClass, DeploymentItem("TestFiles", "TestFiles")]
	public sealed class ProjectLoaderTests : BaseUnitTest<ProjectLoaderThens>
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

		protected override void OnTeardown()
		{
			Then.Logger.Dispose();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValuesSetWhenCreatingGivenValidValues()
		{
			Given.ProjectPath = @"c:\some folder";
			When(Creating);
			Then.Object.ProjectPath.Should().Be(@"c:\some folder");
		}

		[TestMethod, NegativeTest]
		public void ShouldIndicateInvalidPathWhenLoadProjectFromInvalidPath()
		{
			Given.ProjectPath = @"c:\some folder";
			When(Loading);
			Then.Has.Should().Be(Returned.Failure, "because the path is invalid");
		}

		[TestMethod, NegativeTest]
		public void ShouldIndicateMissingConfigurationWhenLoadProjectFromWrongFolder()
		{
			Given.ProjectPath = @".";
			When(Loading);
			Then.Has.Should().Be(Returned.Failure, "because the config file does not exist.");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveConfigurationAndSourceFilesWhenLoadingProjectGivenValidFiles()
		{
			Given.ProjectPath = @".\TestFiles\ProjectA";

			When(Loading);

			Then.Has.Should().Be(Returned.Success);
			Then.Object.Configuration.Should().NotBeNull();
			Then.Object.Configuration.Project.Should().Be("Conmigo");
			Then.Object.Configuration.Version.Should().Be("14.06.03.01");
			Then.Object.AllScripts.Should().HaveCount(3);
		}

		protected override void Creating()
		{
			Then.Logger = new StringLogger();
			Then.Object = new ProjectLoader(Given.ProjectPath, scriptParser: Given.Parser, logger: Then.Logger);
		}

		private void Loading()
		{
			Then.Has = Then.Object.Load();
		}
	}

	public sealed class ProjectLoaderThens
	{
		public ProjectLoader Object;
		public Returned Has;
		public StringLogger Logger;
	}
}