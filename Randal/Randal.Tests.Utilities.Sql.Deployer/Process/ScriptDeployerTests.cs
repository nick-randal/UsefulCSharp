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
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Logging;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Process;
using Randal.Sql.Deployer.Scripts;
using Randal.Tests.Sql.Deployer.Scripts;
using Rhino.Mocks;

namespace Randal.Tests.Sql.Deployer.Process
{
	[TestClass]
	public sealed class ScriptDeployerTests : UnitTestBase<ScriptDeployerTests.Thens>
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
			WhenLastActionDeferred(Creating);
			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod, NegativeTest]
		public void ShouldThrowExceptionWhenCreatingInstanceGivenNullConnectionManager()
		{
			Given.ConnectionManager = null;
			WhenLastActionDeferred(Creating);
			ThenLastAction.ShouldThrow<ArgumentNullException>();
		}

		[TestMethod, PositiveTest]
		public void ShouldCallExecuteWhenDeployingScripts()
		{
			var messages = new List<string>();
			Given.Project = (Project) new ProjectBuilder()
				.WithConfiguration("Test", "01.01.01.01")
				.WithScript(
					new ScriptBuilder("A")
						.WithCatalogs("master")
						.WithMainBlock("Select 1")
						.Build(messages)
				);

			When(Deploying);

			messages.Should().HaveCount(0);
			Then.Manager.AssertWasCalled(x => x.CreateCommand(Arg<string>.Is.Anything, Arg<object[]>.Is.Anything));
		}

		protected override void Creating()
		{
			var config = Given.Config ?? new ScriptDeployerConfig();
			Then.Deployer = new SqlServerDeployer(config, Given.Project, Given.ConnectionManager, new NullLogger());
			Then.Manager = Given.ConnectionManager;
		}

		private void Deploying()
		{
			Then.Deployer.DeployScripts();
		}

		public sealed class Thens
		{
			public SqlServerDeployer Deployer;
			public ISqlConnectionManager Manager;
		}
	}
}