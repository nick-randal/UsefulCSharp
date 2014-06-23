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
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ScriptSettingsTests : BaseUnitTest<ScriptSettingsThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveDefaultValuesWhenCreating()
		{
			When(Creating);

			Then.Settings.Timeout.Should().Be(30);
		}

		[TestMethod]
		public void ShouldHaveAssignedValuesWhenCreatingGivenValues()
		{
			Given.Timeout = 357;

			When(Creating);

			Then.Settings.Timeout.Should().Be(357);
		}

		private void Creating()
		{
			if(Given.TestForMember("Timeout"))
				Then.Settings = new ScriptSettings(Given.Timeout);
			else
				Then.Settings = new ScriptSettings();
		}
	}

	public sealed class ScriptSettingsThens
	{
		public ScriptSettings Settings;
	}
}
