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
using Randal.Sql.Deployer.Scripts;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ScriptSettingsTests : UnitTestBase<ScriptSettingsThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveDefaultValuesWhenCreating()
		{
			When(Creating);

			Then.Settings.Timeout.Should().Be(30);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveAssignedValuesWhenCreatingGivenValues()
		{
			Given.Timeout = 357;

			When(Creating);

			Then.Settings.Timeout.Should().Be(357);
		}

		protected override void Creating()
		{
			Then.Settings = GivensDefined("Timeout") ? new ScriptSettings(Given.Timeout) : new ScriptSettings();
		}
	}

	public sealed class ScriptSettingsThens
	{
		public ScriptSettings Settings;
	}
}