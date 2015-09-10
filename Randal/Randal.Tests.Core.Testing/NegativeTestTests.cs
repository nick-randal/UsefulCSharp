// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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

namespace Randal.Tests.Core.Testing
{
	[TestClass]
	public sealed class NegativeTestTests : BaseUnitTest<NegativeTestThens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveAttributeWithCorrectTraits_WhenCreating()
		{
			When(Creating);

			Then
				.Attribute.Should().NotBeNull()
				.And
				.BeAssignableTo<TestCategoryBaseAttribute>();
			Then.Attribute.TestCategories.Should().HaveCount(1);
			Then.Attribute.TestCategories[0].Should().Be("Negative");

		}

		protected override void Creating()
		{
			Then.Attribute = new NegativeTest();
		}
	}

	public sealed class NegativeTestThens
	{
		public NegativeTest Attribute;
	}
}