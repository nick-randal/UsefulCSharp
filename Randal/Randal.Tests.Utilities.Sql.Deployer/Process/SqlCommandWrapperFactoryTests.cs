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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Process;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlCommandWrapperFactoryTests : UnitTestBase<SqlCommandWrapperFactoryThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidFactoryWhenCreating()
		{
			When(Creating);

			Then.Factory.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapperFactory>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveSqlCommandWrapperWhenFactoryCreatesNewCommand()
		{
			Given.CommandText = "Select 1";

			When(CreatingNewCommand);

			Then.Command.Should().NotBeNull().And.BeAssignableTo<ISqlCommandWrapper>();
		}

		private void CreatingNewCommand()
		{
			Then.Command = Then.Factory.CreateCommand(null, null, Given.CommandText);
		}

		protected override void Creating()
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