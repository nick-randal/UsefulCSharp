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

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Configuration;

namespace Randal.Tests.Sql.Deployer.Configuration
{
	[TestClass]
	public sealed class FilterConfigTests : UnitTestBase<FilterConfigThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveEmptyFilter_WhenCreating()
		{
			When(Creating);

			Then.Target.Should().NotBeNull().And.BeAssignableTo<IValidationFilterConfig>();
			Then.Target.HaltOn.Should().BeNull();
			Then.Target.WarnOn.Should().BeNull();
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveFilters_WhenCreating_GivenFilters()
		{
			Given.WarnFilters = new[] { " testA " };
			Given.HaltFilters = new[] { " testB " };

			When(Creating);

			Then.Target.WarnOn.Should().HaveCount(1);
			Then.Target.WarnOn.First().Should().Be(" testA ");
			Then.Target.HaltOn.Should().HaveCount(1);
			Then.Target.HaltOn.First().Should().Be(" testB ");
		}

		protected override void Creating()
		{
			Then.Target = new ValidationFilterConfig();

			if (Given.WarnFilters != null)
				Then.Target.WarnOn = Given.WarnFilters;

			if (Given.HaltFilters != null)
				Then.Target.HaltOn = Given.HaltFilters;
		}
	}

	public sealed class FilterConfigThens
	{
		public ValidationFilterConfig Target;
	}
}
