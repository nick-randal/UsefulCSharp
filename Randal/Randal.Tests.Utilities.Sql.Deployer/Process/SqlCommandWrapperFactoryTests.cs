using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Process;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlCommandWrapperFactoryTests : BaseUnitTest<SqlCommandWrapperFactoryThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveValidFactoryWhenCreating()
		{
			When(Creating);

			Then.Factory.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapperFactory>();
		}

		[TestMethod]
		public void ShouldHaveSqlCommandWrapperWhenFactoryCreatesNewCommand()
		{
			Given.CommandText = "Select 1";

			When(Creating, CreatingNewCommand);

			Then.Command.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapper>();
		}

		private void CreatingNewCommand()
		{
			Then.Command = Then.Factory.CreateCommand(null, null, Given.CommandText);
		}

		private void Creating()
		{
			Then.Factory = new SqlCommandWrapperFactory();
		}
	}

	public sealed class SqlCommandWrapperFactoryThens
	{
		public SqlCommandWrapperFactory Factory;
		public ISqlCommandWrapper Command;
	}
}
