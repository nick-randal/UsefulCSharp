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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	[TestClass]
	public sealed class ConfigurationBlockTests : BaseUnitTest<TimeoutBlockThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void ShouldHaveTimeoutBlockWhenCreaatingInstance()
		{
			Given.Json = "{}";

			When(Creating);

			Then.Configuration.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Configuration.Text.Should().Be("{}");
			Then.Configuration.IsValid.Should().BeFalse();
			Then.Configuration.Keyword.Should().Be("configuration");
			Then.Configuration.Settings.Should().BeNull();
		}

		[TestMethod]
		public void ShouldHaveValuesWhenParsingGivenValidJson()
		{
			Given.Json = "{ timeout: 256, usetransaction: false }";

			When(Creating, Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(256);
			Then.Settings.UseTransaction.Should().BeFalse();
		}

		[TestMethod]
		public void ShouldHaveDefaultValuesWhenParsingGivenValidJsonWithoutValues()
		{
			Given.Json = "{ }";

			When(Creating, Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(30);
			Then.Settings.UseTransaction.Should().BeTrue();
		}

		[TestMethod]
		public void ShouldHaveValuesWhenParsingGivenJsonWithoutEnclosingBraces()
		{
			Given.Json = "Timeout: 256, useTransaction: true";

			When(Creating, Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(256);
			Then.Settings.UseTransaction.Should().BeTrue();
		}

		private void Creating()
		{
			Then.Configuration = new ConfigurationBlock(Given.Json);
		}

		private void Parsing()
		{
			Then.Messages = Then.Configuration.Parse();
			Then.Settings = Then.Configuration.Settings;
		}
	}

	public sealed class TimeoutBlockThens
	{
		public ConfigurationBlock Configuration;
		public IReadOnlyList<string> Messages;
		public ScriptSettings Settings;
	}
}