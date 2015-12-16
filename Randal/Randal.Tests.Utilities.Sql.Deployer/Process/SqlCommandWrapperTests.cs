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
using System.Data.SqlClient;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Process;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlCommandWrapperTests : UnitTestBase<SqlCommandWrapperThens>
	{
		protected override void OnTeardown()
		{
			Then.Wrapper.Dispose();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstance_WhenCreating()
		{
			Given.Database = "master";
			When(Creating);
			Then.Wrapper.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapper>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowException_WhenExecuting_GivenNoSqlServer()
		{
			Given.Database = "master";
			WhenLastActionDeferred(ExecutingCommand);
			ThenLastAction.ShouldThrow<InvalidOperationException>().WithMessage("Invalid operation. The connection is closed.");
		}

		private void ExecutingCommand()
		{
			Then.Wrapper.Execute(Given.Database);
		}

		protected override void Creating()
		{
			Then.Wrapper = new SqlCommandWrapper(new SqlConnection(), string.Empty);
		}
	}

	public sealed class SqlCommandWrapperThens
	{
		public SqlCommandWrapper Wrapper;
	}
}