using System.Collections.Generic;
using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ServerWrapperIntegrationTests : BaseUnitTest<ServerWrapperThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidWrapperWhenCreatingInstance()
		{
			Given.Name = ".";
			When(Creating);
			Then.Server.Should().NotBeNull().And.BeAssignableTo<IServer>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveListWhenGettingDatabases()
		{
			Given.Name = ".";
			When(GettingDatabases);
			Then.Databases.Should().NotBeEmpty();
		}

		private void GettingDatabases()
		{
			Then.Databases = Then.Server.GetDatabases();
		}

		protected override void Creating()
		{
			Then.Server = new ServerWrapper(Given.Name);
		}
	}

	public sealed class ServerWrapperThens
	{
		public ServerWrapper Server;
		public IEnumerable<Database> Databases;
	}
}
