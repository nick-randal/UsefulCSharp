// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
using Randal.Logging;
using Randal.Sql.Deployer.Process;
using Randal.Sql.Deployer.Scripts;
using Randal.Tests.Sql.Deployer.Scripts;
using Rhino.Mocks;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass]
	public sealed class ScriptDeployerTests : BaseUnitTest<SqlDeployerThens>
	{
		protected override void OnSetup()
		{
			Given.Project = MockRepository.GenerateMock<IProject>();

			var manager = MockRepository.GenerateMock<ISqlConnectionManager>();
			manager.Stub(x => x.CreateCommand(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything))
				.Return(MockRepository.GenerateMock<ISqlCommandWrapper>());
			manager.Stub(x => x.DatabaseNames).Return(new[] {"master", "model", "Research", "Data"});

			Given.ConnectionManager = manager;
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveScriptDeployerWhenCreating()
		{
			When(Creating);

			Then.Deployer.Should().NotBeNull().And.BeAssignableTo<IScriptDeployer>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowExceptionWhenCreatingInstanceGivenNullProject()
		{
			Given.Project = null;
			ThrowsExceptionWhen(Creating);
			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowExceptionWhenCreatingInstanceGivenNullConnectionManager()
		{
			Given.ConnectionManager = null;
			ThrowsExceptionWhen(Creating);
			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod, PositiveTest]
		public void ShouldCallExecuteWhenDeployingScripts()
		{
			Given.Project = (Project) new ProjectBuilder()
				.WithConfiguration("Test", "01.01.01.01")
				.WithScript(
					(SourceScript) new ScriptBuilder("A")
						.WithCatalogs("master")
						.WithMainBlock("Select 1")
				);

			When(Deploying);

			Then.Manager.AssertWasCalled(x => x.CreateCommand(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything));
		}

		protected override void Creating()
		{
			Then.Deployer = new ScriptDeployer(Given.Project, Given.ConnectionManager, new StringLogger());
			Then.Manager = Given.ConnectionManager;
		}

		private void Deploying()
		{
			Then.Deployer.DeployScripts();
		}
	}

	public sealed class SqlDeployerThens
	{
		public ScriptDeployer Deployer;
		public ISqlConnectionManager Manager;
	}
}