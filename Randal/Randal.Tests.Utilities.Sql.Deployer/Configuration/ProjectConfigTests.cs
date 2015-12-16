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
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Configuration;

namespace Randal.Tests.Sql.Deployer.Configuration
{
	[TestClass]
	public sealed class ProjectConfigTests : UnitTestBase<InstallerConfigThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveInvalidConfig_WhenCreating()
		{
			Given.Json = "{ Version: '', Project: '' }";

			When(Creating);

			Then.Target.Should().NotBeNull();
			Then.Target.PriorityScripts.Should().HaveCount(0);
			Then.Target.Vars.Should().HaveCount(0);
			Then.Target.Project.Should().Be("");
			Then.Target.Version.Should().Be("");
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveValidationMessages_WhenValidating_GivenInvalidJson()
		{
			Given.Json = "{ Version: '', Project: '', Vars: { 'mo$': '', 'a.b': '', 'az-45_': 'valid' } }";

			When(Validating);

			Then.Messages.Should().HaveCount(4, string.Join(Environment.NewLine, Then.Messages));
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidValues_WhenCreating_GivenValidJson()
		{
			Given.Json = "{ Version: '14.06.04.01', Project: 'UnitTest', PriorityScripts: [ 'A' ], Vars: {'A':'Apples','B':'Bears'} }";

			When(Creating, Validating);

			Then.Target.PriorityScripts.Should().HaveCount(1);
			Then.Target.PriorityScripts.FirstOrDefault().Should().Be("A");
			Then.Target.Version.Should().Be("14.06.04.01");
			Then.Target.Project.Should().Be("UnitTest");
			Then.Target.Vars.ShouldBeEquivalentTo(
				new Dictionary<string, string>
				{
					{ "A", "Apples" },
					{ "B", "Bears" }
				}
			);
			Then.Messages.Should().HaveCount(0);
		}

		protected override void Creating()
		{
			Then.Target = JsonConvert.DeserializeObject<ProjectConfigJson>(Given.Json);
		}

		private void Validating()
		{
			IList<string> messages;
			Then.Target.Validate(out messages);

			Then.Messages = messages;
		}
	}

	public sealed class InstallerConfigThens
	{
		public IProjectConfig Target;
		public IList<String> Messages;
	}
}