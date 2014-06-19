using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Configuration;
using Randal.Utilities.Sql.Deployer.IO;
using Randal.Utilities.Sql.Deployer.Scripts;
using System.Collections.Generic;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass, DeploymentItem("TestFiles", "TestFiles")]
	public sealed class ProjectTests : BaseUnitTest<ProjectTestsThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();

			var loader = new ProjectLoader(@".\TestFiles\ProjectA");
			loader.Load();
			Given.Configuration = loader.Configuration;
			Given.Scripts = loader.AllScripts;
		}

		[TestMethod]
		public void ShouldHaveConfigurationAndScriptsWhenCreating()
		{
			When(Creating);

			Then.Project.Should().NotBeNull();
			Then.Project.AllScripts.Should().HaveCount(3);
			Then.Project.PriorityScripts.Should().HaveCount(1);
			Then.Project["ScriptA"].Should().NotBeNull();
			Then.Project["ScriptB"].Should().NotBeNull();
			Then.Project["ScriptC"].Should().NotBeNull();
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullConfiguration()
		{
			Given.Configuration = null;
			When(Creating);
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullScripts()
		{
			Given.Scripts = null;
			When(Creating);
		}

		[TestMethod, ExpectedException(typeof (InvalidOperationException))]
		public void ShouldThrowExceptionWhenCreatingGivenMissingScriptForPriorityList()
		{
			Given.Configuration = new ProjectConfig("Test", "14.06.08.01", new[] { "ScriptA" });
			Given.Scripts = new List<SourceScript>();

			When(Creating);
		}

		private void Creating()
		{
			Then.Project = new Project(Given.Configuration, Given.Scripts);
		}
	}

	public sealed class ProjectTestsThens
	{
		public Project Project;
	}
}