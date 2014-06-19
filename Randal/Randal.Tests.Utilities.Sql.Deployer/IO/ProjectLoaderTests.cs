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