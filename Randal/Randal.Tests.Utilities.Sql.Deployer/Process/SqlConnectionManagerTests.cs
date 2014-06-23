/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Process;
using Rhino.Mocks;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlConnectionManagerTests : BaseUnitTest<ScriptDeployerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
			Then.CommandFactory = null;
		}

		[TestCleanup]
		public void Teardown()
		{
			Then.Manager.Dispose();
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
			Then.Command.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapper>();
		}

		[TestMethod]
		public void ShouldRollbackTransactionWhenExecutingAndRollingBack()
		{
			Given.Server = ".";
			Given.Database = "master";
			Given.CommandText = "Select 1";

			When(Creating, OpenningConnection, BeginningTransaction, CreatingCommand, ExecutingCommand, RollingBack);
		}

		[TestMethod]
		public void ShouldCommitTransactionWhenExecutingAndCommitting()
		{
			Given.Server = ".";
			Given.Database = "master";
			Given.CommandText = "Select 1";

			When(Creating, OpenningConnection, BeginningTransaction, CreatingCommand, ExecutingCommand, Committing);
		}

		[TestMethod, ExpectedException(typeof(InvalidOperationException))]
		public void ShouldThrowExceptionWhenBeginningTransactionWithoutAnOpenConnection()
		{
			When(Creating, BeginningTransaction);
		}

		[TestMethod]
		public void ShouldTakeNoActionWhenCommittingTransactionAndNoTransactionWasStarted()
		{
			Given.Server = ".";
			Given.Database = "master";

			When(Creating, OpenningConnection, Committing);
		}

		[TestMethod]
		public void ShouldTakeNoActionWhenRollingBackTransactionAndNoTransactionWasStarted()
		{
			Given.Server = ".";
			Given.Database = "master";

			When(Creating, OpenningConnection, RollingBack);
		}

		private void Creating()
		{
			Then.Manager = new SqlConnectionManager(Then.CommandFactory);
		}

		private void CreatingCommand()
		{
			Then.Command = Then.Manager.CreateCommand(Given.CommandText);
		}

		private void ExecutingCommand()
		{
			Then.Command.Execute(Given.Database);
		}

		private void OpenningConnection()
		{
			Then.Manager.OpenConnection(Given.Server, Given.Database);
		}

		private void BeginningTransaction()
		{
			Then.Manager.BeginTransaction();
		}

		private void Committing()
		{
			Then.Manager.CommitTransaction();
		}

		private void RollingBack()
		{
			Then.Manager.RollbackTransaction();
		}
	}

	public sealed class ScriptDeployerThens
	{
		public SqlConnectionManager Manager;
		public ISqlCommandWrapper Command;
		public ISqlCommandWrapperFactory CommandFactory;
	}
}
