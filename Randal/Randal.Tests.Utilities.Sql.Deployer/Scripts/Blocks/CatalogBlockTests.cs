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
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class CatalogBlockTests : UnitTestBase<CatalogBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveInvalidBlock_WhenCreating()
		{
			Given.Text = " master ";

			When(Creating);

			Then.Target.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Target.IsValid.Should().BeFalse();
			Then.Target.Keyword.Should().Be("catalog");
			Then.Target.Text.Should().Be("master");
			Then.Target.CatalogPatterns.Should().HaveCount(0);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidBlock_WhenParsing_GivenValidInput()
		{
			Given.Text = "master, DB123, X_%";

			When(Parsing);

			Then.Target.IsValid.Should().BeTrue();
			Then.Messages.Should().HaveCount(0);
			Then.Target.CatalogPatterns.Should().HaveCount(3);
			Then.Target.CatalogPatterns[0].Should().Be("master");
			Then.Target.CatalogPatterns[1].Should().Be("DB123");
			Then.Target.CatalogPatterns[2].Should().Be(@"X_%");
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveErrorMessages_WhenParsing_GivenInvalidInput()
		{
			Given.Text = "t-d?, P&E, t;";

			When(Parsing);

			Then.Target.IsValid.Should().BeFalse();
			Then.Target.CatalogPatterns.Should().HaveCount(0);
			Then.Messages.Should().HaveCount(3);
		}

		[TestMethod, PositiveTest]
		public void ShouldIgnoreAnythingOnNextLine_WhenParsing_GivenMultipleLines()
		{
			Given.Text = @"master
-- this should be ignored";

			When(Parsing);

			Then.Target.IsValid.Should().BeTrue();
			Then.Target.CatalogPatterns.Should().HaveCount(1);
			Then.Target.CatalogPatterns.First().Should().Be("master");
		}

		private void Parsing()
		{
			Then.Messages = Then.Target.Parse();
		}

		protected override void Creating()
		{
			Then.Target = new CatalogBlock(Given.Text);
		}
	}

	public sealed class CatalogBlockThens
	{
		public CatalogBlock Target;
		public IReadOnlyList<string> Messages;
	}
}