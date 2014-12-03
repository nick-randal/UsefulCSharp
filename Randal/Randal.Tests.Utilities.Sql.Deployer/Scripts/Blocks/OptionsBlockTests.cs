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

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class OptionsBlockTests : BaseUnitTest<OptionsBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveTimeoutBlockWhenCreaatingInstance()
		{
			Given.Json = "{}";

			When(Creating);

			Then.Configuration.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Configuration.Text.Should().Be("{}");
			Then.Configuration.IsValid.Should().BeFalse();
			Then.Configuration.Keyword.Should().Be("options");
			Then.Configuration.Settings.Should().BeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValuesWhenParsingGivenValidJson()
		{
			Given.Json = "{ timeout: 256 }";

			When(Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(256);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveDefaultValuesWhenParsingGivenValidJsonWithoutValues()
		{
			Given.Json = "{ }";

			When(Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(30);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValuesWhenParsingGivenJsonWithoutEnclosingBraces()
		{
			Given.Json = "Timeout: 256";

			When(Parsing);

			Then.Settings.Should().NotBeNull();
			Then.Settings.Timeout.Should().Be(256);
		}

		protected override void Creating()
		{
			Then.Configuration = new OptionsBlock(Given.Json);
		}

		private void Parsing()
		{
			Then.Messages = Then.Configuration.Parse();
			Then.Settings = Then.Configuration.Settings;
		}
	}

	public sealed class OptionsBlockThens
	{
		public OptionsBlock Configuration;
		public IReadOnlyList<string> Messages;
		public ScriptSettings Settings;
	}
}