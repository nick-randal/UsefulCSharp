using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Process;
using Rhino.Mocks;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlConnectionManagerTests : BaseUnitTest<UnitTest1Thens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidConnectionmanagerWhenCreating()
		{
			When(Creating);
			Then.Manager.Server.Should().BeEmpty();
			Then.Manager.DatabaseNames.Should().BeEmpty();
		}

		[TestMethod]
		public void ShouldHaveCommandWrapperWhenCreatingCommand()
		{
			Given.CommandText = "Select 1";
			When(Creating, CreatingCommand);
			Then.Command.Should().NotBeNull();
		}

		private void CreatingCommand()
		{
			Then.Command = Then.Manager.CreateCommand(Given.CommandText);
		}

		private void Creating()
		{
			var commandFactory = MockRepository.GenerateMock<ISqlCommandWrapperFactory>();
			commandFactory.Stub(
					x => x.CreateCommand(Arg<SqlConnection>.Is.Anything, Arg<SqlTransaction>.Is.Anything, Arg<string>.Is.Anything, Arg<object[]>.Is.Anything)
				)
				.Return(MockRepository.GenerateMock<ISqlCommandWrapper>());
			Then.Manager = new SqlConnectionManager(commandFactory);
		}
	}

	public sealed class UnitTest1Thens
	{
		public SqlConnectionManager Manager;
		public ISqlCommandWrapper Command;
	}
}
