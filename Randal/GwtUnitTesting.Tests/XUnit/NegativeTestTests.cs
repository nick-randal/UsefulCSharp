// Useful C#
// Copyright (C) 2014-2019 Nicholas Randal
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

using System;
using FluentAssertions;
using GwtUnit.XUnit;
using Xunit;

namespace GwtUnitTesting.Tests.XUnit
{
	public sealed class NegativeTestTests : XUnitTestBase<NegativeTestThens>
	{
		[Fact, GwtUnit.XUnit.PositiveTest]
		public void ShouldHaveAttributeWithCorrectTraits_WhenCreating()
		{
			When(Creating);

			Then
				.Attribute.Should().NotBeNull()
				.And
				.BeAssignableTo<Attribute>();
		}

		protected override void Creating()
		{
			Then.Attribute = new NegativeTestAttribute();
		}
	}

	public sealed class NegativeTestThens
	{
		public NegativeTestAttribute Attribute;
	}
}