// Useful C#
// Copyright (C) 2014-2022 Nicholas Randal
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
using Xunit;

namespace GwtUnit.XUnit.Tests;

public sealed class NegativeTestTests : XUnitTestBase<NegativeTestThens>
{
	[Fact, PositiveTest]
	public void ShouldHaveAttributeWithCorrectTraits_WhenCreating()
	{
		When(Creating);

		Then
			.Target.Should().NotBeNull()
			.And
			.BeAssignableTo<Attribute>();
	}

	protected override void Creating()
	{
		Then.Target = new NegativeTestAttribute();
	}
}

public sealed class NegativeTestThens : Thens<NegativeTestAttribute>
{
}