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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Core.IO.Logging;
using Randal.Utilities.Sql.Deployer.Process;
using Randal.Utilities.Sql.Deployer.Scripts;
using Rhino.Mocks;

namespace Randal.Tests.Utilities.Sql.Deployer.Process
{
	[TestClass]
	public sealed class ScriptDeployerTests : BaseUnitTest<SqlDeployerThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
			Given.Project = MockRepository.GenerateMock<IProject>();
			Given.ConnectionManager = MockRepository.GenerateMock<ISqlConnectionManager>();
		}

		[TestMethod]
		public void ShouldHaveScriptDeployerWhenCreating()
		{
			When(Creating);

			Then.Deployer.Should().NotBeNull().And.BeAssignableTo<IScriptDeployer>();
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingInstanceGivenNullProject()
		{
			Given.Project = null;
			When(Creating);	
		}

		[TestMethod, ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingInstanceGivenNullConnectionManager()
		{
			Given.ConnectionManager = null;
			When(Creating);
		}

		private void Creating()
		{
			Then.Deployer = new ScriptDeployer(Given.Project, Given.ConnectionManager, new StringLogger());
		}
	}

	public sealed class SqlDeployerThens
	{
		public ScriptDeployer Deployer;
	}
}