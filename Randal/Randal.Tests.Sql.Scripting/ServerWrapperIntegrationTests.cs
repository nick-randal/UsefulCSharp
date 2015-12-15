// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Scripting;
using Randal.Tests.Sql.Scripting.Support;

namespace Randal.Tests.Sql.Scripting
{
	[TestClass]
	public sealed class ServerWrapperIntegrationTests : UnitTestBase<ServerWrapperIntegrationTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidWrapper_WhenCreatingInstance()
		{
			When(Creating);
			Then.Server.Should().NotBeNull().And.BeAssignableTo<IServer>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveList_WhenGettingDatabases()
		{
			When(GettingDatabases);
			Then.Databases.Should().NotBeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveList_WhenGettingProcedures()
		{
			Given.Database = "Test_Randal_Sql";
			When(GettingDatabases, GettingProcedures);
			Then.Procedures.Should().NotBeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveList_WhenGettingViews()
		{
			Given.Database = "Test_Randal_Sql";
			When(GettingDatabases, GettingViews);
			Then.Views.Should().NotBeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveList_WhenGettingFunctions()
		{
			Given.Database = "Test_Randal_Sql";
			When(GettingDatabases, GettingFunctions);
			Then.Functions.Should().NotBeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveList_WhenGettingTables()
		{
			Given.Database = "msdb";
			When(GettingDatabases, GettingTables);
			Then.Tables.Should().BeEmpty();
		}

		private void GettingFunctions()
		{
			Then.Functions = Then.Server.GetUserDefinedFunctions(Then.Databases.First(x => x.Name == Given.Database));
		}

		private void GettingTables()
		{
			Then.Tables = Then.Server.GetTables(Then.Databases.First(x => x.Name == Given.Database));
		}

		private void GettingViews()
		{
			Then.Views = Then.Server.GetViews(Then.Databases.First(x => x.Name == Given.Database));
		}

		private void GettingProcedures()
		{
			Then.Procedures = Then.Server.GetStoredProcedures(Then.Databases.First(x => x.Name == Given.Database));
		}

		private void GettingDatabases()
		{
			Then.Databases = Then.Server.GetDatabases();
		}

		protected override void Creating()
		{
			Then.Server = new ServerWrapper(new Server("."));
		}

		protected override void OnSetup()
		{
			ServerSetup.Go();
		}

		public sealed class Thens
		{
			public ServerWrapper Server;
			public IEnumerable<Database> Databases;
			public IEnumerable<UserDefinedFunction> Functions;
			public IEnumerable<StoredProcedure> Procedures;
			public IEnumerable<Table> Tables;
			public IEnumerable<View> Views;
		}
	}
}