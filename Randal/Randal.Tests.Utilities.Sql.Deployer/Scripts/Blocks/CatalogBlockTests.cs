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
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts.Blocks
{
	[TestClass]
	public sealed class CatalogBlockTests : BaseUnitTest<CatalogBlockThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveInvalidBlockWhenCreating()
		{
			Given.Text = " master ";

			When(Creating);

			Then.Object.Should().NotBeNull().And.BeAssignableTo<IScriptBlock>();
			Then.Object.IsValid.Should().BeFalse();
			Then.Object.Keyword.Should().Be("catalog");
			Then.Object.Text.Should().Be("master");
			Then.Object.CatalogPatterns.Should().HaveCount(0);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveValidBlockWhenParsingGivenValidInput()
		{
			Given.Text = "master, DB123, X_%";

			When(Parsing);

			Then.Object.IsValid.Should().BeTrue();
			Then.Messages.Should().HaveCount(0);
			Then.Object.CatalogPatterns.Should().HaveCount(3);
			Then.Object.CatalogPatterns[0].Should().Be("master");
			Then.Object.CatalogPatterns[1].Should().Be("DB123");
			Then.Object.CatalogPatterns[2].Should().Be(@"X_%");
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveErrorMessagesWhenParsingGivenInvalidInput()
		{
			Given.Text = "t-d?, P&E, t;";

			When(Parsing);

			Then.Object.IsValid.Should().BeFalse();
			Then.Object.CatalogPatterns.Should().HaveCount(0);
			Then.Messages.Should().HaveCount(3);
		}

		private void Parsing()
		{
			Then.Messages = Then.Object.Parse();
		}

		protected override void Creating()
		{
			Then.Object = new CatalogBlock(Given.Text);
		}
	}

	public sealed class CatalogBlockThens
	{
		public CatalogBlock Object;
		public IReadOnlyList<string> Messages;
	}
}