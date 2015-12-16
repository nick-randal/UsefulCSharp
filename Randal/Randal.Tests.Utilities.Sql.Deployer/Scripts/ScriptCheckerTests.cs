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

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ScriptCheckerTests : UnitTestBase<ScriptCheckThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveValidChecker_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull().And.BeAssignableTo<IScriptChecker>();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidState_WhenValidating_GivenSafeScript()
		{
			Given.Script = TestScripts.SafeScript;

			When(Validating);

			Then.Validation.Should().Be(ScriptCheck.Passed);
			Then.Messages.Should().BeEmpty();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveInvalidState_WhenValidating_GivenSuspectScript()
		{
			Given.Script = TestScripts.UnsafeScript;

			When(Validating);

			Then.Validation.Should().Be(ScriptCheck.Failed | ScriptCheck.Warning);
			Then.Messages.Should().HaveCount(5);
			Then.Messages.ElementAt(0).Should().Contain("Warning: Line 2");
			Then.Messages.ElementAt(1).Should().Contain("Warning: Line 4");
			Then.Messages.ElementAt(2).Should().Contain("Failed: Line 1");
			Then.Messages.ElementAt(3).Should().Contain("Failed: Line 3");
			Then.Messages.ElementAt(4).Should().Contain("Failed: Line 5");
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveInvalidScript_WhenValidating_GivenBrokenScript()
		{
			Given.Script = TestScripts.BrokenScript;

			When(Validating);

			Then.Validation.Should().Be(ScriptCheck.Fatal);
			Then.Messages.Should().HaveCount(1);
			Then.Messages.ElementAt(0).Should().Contain("expected Multi-line comment end");
		}

		protected override void Creating()
		{
			Then.Target = new ScriptChecker();

			if (Given.WarnFilters == null)
				Given.WarnFilters = new[] { @"(?<=[\s^.\[]*)exec\s*\(" };

			foreach(var filter in Given.WarnFilters)
				Then.Target.AddValidationPattern(filter, ScriptCheck.Warning);
			

			if (Given.FatalFilters == null)
				Given.FatalFilters = new[] { @"drop\s+\w", @"truncate\s+table" };

			foreach (var filter in Given.FatalFilters)
				Then.Target.AddValidationPattern(filter, ScriptCheck.Failed);
		}

		private void Validating()
		{
			var messages = new List<string>();
			Then.Validation = Then.Target.Validate(Given.Script, messages);
			Then.Messages = messages;
		}
	}

	public sealed class ScriptCheckThens
	{
		public ScriptChecker Target;
		public IEnumerable<string> Messages;
		public ScriptCheck Validation;
	}
}
