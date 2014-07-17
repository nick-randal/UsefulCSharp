﻿// Useful C#
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
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.IO;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass, DeploymentItem("TestFiles", "TestFiles")]
	public sealed class ProjectTests : BaseUnitTest<ProjectTestsThens>
	{
		protected override void OnSetup()
		{
			var parser = new ScriptParser();

			parser.AddRule("catalog", txt => new CatalogBlock(txt));
			parser.AddRule("options", txt => new OptionsBlock(txt));
			parser.AddRule("need", txt => new NeedBlock(txt));
			parser.AddRule("ignore", txt => new IgnoreScriptBlock(txt));
			parser.AddRule("pre", txt => new SqlCommandBlock("pre", txt, SqlScriptPhase.Pre));
			parser.AddRule("main", txt => new SqlCommandBlock("main", txt, SqlScriptPhase.Main));
			parser.AddRule("post", txt => new SqlCommandBlock("post", txt, SqlScriptPhase.Post));

			parser.SetFallbackRule((kw, txt) => new UnexpectedBlock(kw, txt));

			var loader = new ProjectLoader(@".\TestFiles\ProjectA", parser);
			loader.Load();
			Given.Configuration = loader.Configuration;
			Given.Scripts = loader.AllScripts;
		}

		[TestMethod]
		public void ShouldHaveConfigurationAndScriptsWhenCreating()
		{
			When(Creating);

			Then.Project.Should().NotBeNull();
			Then.Project.NonPriorityScripts.Should().HaveCount(2);
			Then.Project.PriorityScripts.Should().HaveCount(1);
			Then.Project.TryGetScript("ScriptA").Should().NotBeNull();
			Then.Project.TryGetScript("ScriptB").Should().NotBeNull();
			Then.Project.TryGetScript("ScriptC").Should().NotBeNull();
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullConfiguration()
		{
			Given.Configuration = null;
			When(Creating);
		}

		[TestMethod, ExpectedException(typeof (ArgumentNullException))]
		public void ShouldThrowExceptionWhenCreatingGivenNullScripts()
		{
			Given.Scripts = null;
			When(Creating);
		}

		[TestMethod, ExpectedException(typeof (InvalidOperationException))]
		public void ShouldThrowExceptionWhenCreatingInstanceGivenMissingScriptForPriorityList()
		{
			Given.Configuration = new ProjectConfig("Test", "14.06.08.01", new[] {"ScriptA"});
			Given.Scripts = new List<SourceScript>();

			When(Creating);
		}

		protected override void Creating()
		{
			Then.Project = new Project(Given.Configuration, Given.Scripts);
		}
	}

	public sealed class ProjectTestsThens
	{
		public Project Project;
	}
}