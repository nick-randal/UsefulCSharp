/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Utilities.Sql.Deployer.IO;
using Randal.Utilities.Sql.Deployer.Scripts;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Utilities.Sql.Deployer.IO
{
	[TestClass, DeploymentItem("TestFiles", "TestFiles")]
	public sealed class ProjectLoaderTests : BaseUnitTest<ProjectLoaderThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
			
			Given.Parser = new ScriptParserFactory().CreateStandardParser();
		}

		[TestMethod]
		public void ShouldHaveValuesSetWhenCreatingGivenValidValues()
		{
			Given.ProjectPath = @"c:\some folder";
			When(Creating);
			Then.Object.ProjectPath.Should().Be(@"c:\some folder");
		}

		[TestMethod]
		public void ShouldIndicateInvalidPathWhenLoadProjectFromInvalidPath()
		{
			Given.ProjectPath = @"c:\some folder";
			When(Creating, Loading);
			Then.Messages.Should().HaveCount(1);
		}

		[TestMethod]
		public void ShouldIndicateMissingConfigurationWhenLoadProjectFromWrongFolder()
		{
			Given.ProjectPath = @".";
			When(Creating, Loading);
			Then.Messages.Should().HaveCount(1);
		}

		[TestMethod]
		public void ShouldHaveConfigurationAndSourceFilesWhenLoadingProjectGivenValidFiles()
		{
			Given.ProjectPath = @".\TestFiles\ProjectA";

			When(Creating, Loading);

			Then.Messages.Should().HaveCount(0, "because " + string.Join(", ", Then.Messages));
			Then.Object.Configuration.Should().NotBeNull();
			Then.Object.Configuration.Project.Should().Be("Conmigo");
			Then.Object.Configuration.Version.Should().Be("14.06.03.01");
			Then.Object.AllScripts.Should().HaveCount(3);
		}

		private void Creating()
		{
			Then.Object = new ProjectLoader(Given.ProjectPath, Given.Parser);
		}

		private void Loading()
		{
			Then.Messages = Then.Object.Load();
		}
	}

	public sealed class ProjectLoaderThens
	{
		public ProjectLoader Object;
		public IReadOnlyList<string> Messages;
	}
}