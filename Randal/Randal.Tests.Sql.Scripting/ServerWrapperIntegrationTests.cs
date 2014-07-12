using System;
using System.Collections;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Sql.Scripting;
using Randal.Core.Testing.UnitTest;
using System.Collections.Generic;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ServerWrapperIntegrationTests : BaseUnitTest<ServerWrapperThens>
	{
		[TestMethod]
		public void ShouldHaveValidWrapperWhenCreatingInstance()
		{
			Given.Name = ".";
			When(Creating);
			Then.Server.Should().NotBeNull().And.BeAssignableTo<IServer>();
		}

		[TestMethod]
		public void ShouldHaveListWhenGettingDatabases()
		{
			Given.Name = ".";
			When(Creating, GettingDatabases);
			Then.Databases.Should().NotBeEmpty();
		}

		private void GettingDatabases()
		{
			Then.Databases = Then.Server.GetDatabases();
		}

		private void Creating()
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
