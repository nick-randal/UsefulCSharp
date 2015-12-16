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

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Process;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlConnectionManagerTests : UnitTestBase<ScriptDeployerThens>
	{
		protected override void OnTeardown()
		{
			Then.Manager.Dispose();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidConnectionmanager_WhenCreating()
		{
			When(Creating);
			Then.Manager.Server.Should().BeEmpty();
			Then.Manager.DatabaseNames.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveCommandWrapper_WhenCreatingCommand()
		{
			Given.CommandText = "Select 1";
			When(CreatingCommand);
			Then.Command.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapper>();
		}

		[TestMethod, PositiveTest]
		public void ShouldRollbackTransaction_WhenExecutingAndRollingBack()
		{
			Given.Server = ".";
			Given.Database = "master";
			Given.CommandText = "Select 1";

			When(OpenningConnection, BeginningTransaction, CreatingCommand, ExecutingCommand, RollingBack);

			// Then no exceptions are thrown
		}

		[TestMethod, PositiveTest]
		public void ShouldCommitTransaction_WhenExecutingAndCommitting()
		{
			Given.Server = ".";
			Given.Database = "master";
			Given.CommandText = "Select 1";

			When(OpenningConnection, BeginningTransaction, CreatingCommand, ExecutingCommand, Committing);

			// Then no exceptions are thrown
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenBeginningTransaction_GivenNoOpenConnection()
		{
			WhenLastActionDeferred(BeginningTransaction);
			ThenLastAction.ShouldThrow<InvalidOperationException>();
		}

		[TestMethod, PositiveTest]
		public void ShouldTakeNoAction_WhenCommittingTransaction_GivenNoTransactionWasStarted()
		{
			Given.Server = ".";
			Given.Database = "master";

			When(OpenningConnection, Committing);

			// Then no exceptions are thrown
		}

		[TestMethod, PositiveTest]
		public void ShouldTakeNoAction_WhenRollingBackTransaction_GivenNoTransactionWasStarted()
		{
			Given.Server = ".";
			Given.Database = "master";

			When(OpenningConnection, RollingBack);

			// Then no exceptions are thrown
		}

		protected override void Creating()
		{
			Then.Manager = new SqlConnectionManager("select 'master'", Then.CommandFactory);
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