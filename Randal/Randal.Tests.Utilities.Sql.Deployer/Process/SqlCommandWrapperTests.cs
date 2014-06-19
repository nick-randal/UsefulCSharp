using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using System.Data.SqlClient;
using Randal.Utilities.Sql.Deployer.Process;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlCommandWrapperTests : BaseUnitTest<SqlCommandWrapperThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestCleanup]
		public void Teardown()
		{
			Then.Wrapper.Dispose();
		}

		[TestMethod]
		public void ShouldHaveWrapperWhenCreating()
		{
			Given.Database = "master";
			When(Creating);
			Then.Wrapper.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapper>();
		}

		[TestMethod, ExpectedException(typeof (InvalidOperationException))]
		public void ShouldThrowSqlExceptionWhenExecutingGivenNoSqlServer()
		{
			Given.Database = "master";
			When(Creating, ExecutingCommand);
		}

		private void ExecutingCommand()
		{
			
			Then.Wrapper.Execute(Given.Database);
		}

		private void Creating()
		{
			Then.Wrapper = new SqlCommandWrapper(new SqlConnection(), string.Empty);
		}
	}

	public sealed class SqlCommandWrapperThens
	{
		public SqlCommandWrapper Wrapper;
	}
}
