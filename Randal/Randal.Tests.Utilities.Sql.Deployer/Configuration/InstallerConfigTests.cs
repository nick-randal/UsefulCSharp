using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Configuration;
using Newtonsoft.Json.Linq;

namespace Randal.Tests.Utilities.Sql.Deployer.Configuration
{
	[TestClass]
	public sealed class InstallerConfigTests : BaseUnitTest<InstallerConfigThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveDefaultConfigWhenCreating()
		{
			Given.Json = "{}";

			When(Creating);

			Then.Object.Should().NotBeNull();
			Then.Object.PriorityScripts.Should().HaveCount(0);
			Then.Object.Project.Should().Be("Unknown");
			Then.Object.Version.Should().Be("01.01.01.01");
		}

		[TestMethod]
		public void ShouldHaveValidValuesWhenCreatingGivenValueJson()
		{
			Given.Json = "{ Version: '14.06.04.01', Project: 'UnitTest', PriorityScripts: [ 'A' ] }";

			When(Creating);

			Then.Object.PriorityScripts.Should().HaveCount(1);
			Then.Object.PriorityScripts.FirstOrDefault().Should().Be("A");
			Then.Object.Version.Should().Be("14.06.04.01");
			Then.Object.Project.Should().Be("UnitTest");
		}

		private void Creating()
		{
			var jsonObject = JObject.Parse(Given.Json);
			Then.Object = new ProjectConfig(jsonObject);
		}
	}

	public sealed class InstallerConfigThens
	{
		public IProjectConfig Object;
	}
}
