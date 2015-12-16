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

using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Helpers;

namespace Randal.Tests.Sql.Deployer.Helpers
{
	[TestClass]
	public sealed class CatalogPatternLookupTests : UnitTestBase<CatalogPatternLookupThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveEmptyLookup_WhenCreatingInstance()
		{
			When(Creating);

			Then.Lookup.Should().NotBeNull();
			Then.Lookup.Count.Should().Be(0);
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveRegex_WhenGettingPattern_GivenText()
		{
			Given.Text = "ma%er";

			When(Getting);

			Then.Pattern.Options.Should()
				.Be(RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			Then.Lookup.Count.Should().Be(1);
			Then.Pattern.ToString().Should().Be(@"^ma[._\w\d-]*er$");
		}

		private void Getting()
		{
			Then.Pattern = Then.Lookup[Given.Text];
		}

		protected override void Creating()
		{
			Then.Lookup = new CatalogPatternLookup();
		}
	}

	public sealed class CatalogPatternLookupThens
	{
		public CatalogPatternLookup Lookup;
		public Regex Pattern;
	}
}