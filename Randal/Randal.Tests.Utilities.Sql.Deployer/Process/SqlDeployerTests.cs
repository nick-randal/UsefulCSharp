using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Core.IO.Logging;
using Randal.Utilities.Sql.Deployer.Process;
using Rhino.Mocks;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlDeployerTests : BaseUnitTest<SqlDeployerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveScriptDeployerWhenCreating()
		{
			When(Creating);

			Then.Deployer.Should().NotBeNull().And.BeAssignableTo<IScriptDeployer>();
		}

		private void Creating()
		{
			var connectionManager = MockRepository.GenerateMock<ISqlConnectionManager>();
			Then.Deployer = new ScriptDeployer(connectionManager, new StringLogger());
		}
	}

	public sealed class SqlDeployerThens
	{
		public ScriptDeployer Deployer;
	}
}