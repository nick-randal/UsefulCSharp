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

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Randal.Core.Testing.Factory.Tests
{
	/// <summary>
	/// Created by nrandal on 9/10/2015 10:24:57 AM
	/// </summary>
	[TestClass]
	public sealed class MemberHashcodeValueFactoryTests : UnitTestBase<MemberHashcodeValueFactoryTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingStringValue()
		{
			When(GettingStringValue);

			Then.StringValue.Should().Be("FirstName1");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveTenthValue_WhenIncrementingAndGettingStringValue()
		{
			When(Repeat(Incrementing, 9), GettingStringValue);

			Then.StringValue.Should().Be("FirstName10");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveFirstValue_WhenIncrementingResettingAndGettingStringValue()
		{
			When(Repeat(Incrementing, 9), Resetting, GettingStringValue);

			Then.StringValue.Should().Be("FirstName1");
		}

		protected override void Creating()
		{
			Then.Target = new IncrementByMemberValueFactory(new DateTime(2016, 5, 17));
		}

		private void Incrementing()
		{
			Then.Target.Increment();
		}

		private void Resetting()
		{
			Then.Target.Reset();
		}

		private void GettingStringValue()
		{
			Then.StringValue = Then.Target.GetString(Given.FieldName ?? "FirstName");
		}

		public sealed class Thens
		{
			public IncrementByMemberValueFactory Target;
			public string StringValue;
		}
	}
}