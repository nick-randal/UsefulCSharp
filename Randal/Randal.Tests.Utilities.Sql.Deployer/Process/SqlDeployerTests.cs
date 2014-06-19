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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Core.IO.Logging;
using Randal.Utilities.Sql.Deployer.Process;
using Rhino.Mocks;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class SqlDeployerTests : BaseUnitTest<SqlDeployerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveScriptDeployerWhenCreating()
		{
			When(Creating);

			Then.Deployer.Should().NotBeNull().And.BeAssignableTo<IScriptDeployer>();
		}

		private void Creating()
		{
			var connectionManager = MockRepository.GenerateMock<ISqlConnectionManager>();
			Then.Deployer = new ScriptDeployer(connectionManager, new StringLogger());
		}
	}

	public sealed class SqlDeployerThens
	{
		public ScriptDeployer Deployer;
	}
}