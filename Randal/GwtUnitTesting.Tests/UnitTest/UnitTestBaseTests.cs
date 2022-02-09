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
using System.Threading.Tasks;
using FluentAssertions;
using GwtUnit.UnitTest;
using GwtUnit.XUnit;
using Xunit;

namespace GwtUnitTesting.Tests.UnitTest
{
	public sealed class UnitTestBaseTests : XUnitTestBase<UnitTestBaseTests.Thens>
	{
		[Fact, GwtUnit.UnitTest.PositiveTest]
		public void ShouldRepeatAction_WhenRepeatIncrementing()
		{
			When(Repeat(Incrementing, 10));

			Then.Target.Thens.Repetitions.Should().Be(10);
		}

		[Fact, GwtUnit.UnitTest.PositiveTest]
		public void ShouldRepeatActionAtLeastOnce_WhenRepeatIncrementing_GivenInvalidRepetitions()
		{
			When(Repeat(Incrementing, -1234));

			Then.Target.Thens.Repetitions.Should().Be(1);
		}

		[Fact, GwtUnit.UnitTest.PositiveTest]
		public void ShouldAwaitAsynchronousFunction_WhenTestingAsyncMethod()
		{
			When(Await(Processing));

			Then.Target.Thens.DelayedValue.Should().Be(4567);
		}

		[Fact, GwtUnit.UnitTest.NegativeTest]
		public void ShouldHaveActionActionAvailableThroughProperty_WhenExpectingAnException()
		{
			WhenLastActionDeferred(Exploding);

			ThenLastAction.Should().NotBeNull();
			ThenLastAction.Should().Throw<Exception>("Did I do that?");
		}

		[Fact, GwtUnit.UnitTest.NegativeTest]
		public void ShouldHaveGivensDefined_GivenValues()
		{
			Given.FirstName = "Bob";
			Given.LastName = "Jones";

			GivensDefined("FirstName", "LastName").Should().BeTrue();
		}

		protected override void Creating()
		{
			Then.Target = new SampleTests();
			Then.Target.Setup();
		}

		private void Incrementing()
		{
			Then.Target.Incrementing();
		}

		private async Task Processing()
		{
			await Then.Target.Processing();
		}

		private static void Exploding()
		{
			throw new Exception("Did I do that?");
		}

		public sealed class Thens
		{
			public SampleTests Target;
		}
	}

	public sealed class SampleTests : UnitTestBase<SampleTestsThens>
	{
		public void Incrementing()
		{
			Then.Repetitions++;
		}

		public async Task Processing()
		{
			await Task.Delay(1000);

			Then.DelayedValue = 4567;
		}

		public static void Exploding()
		{
			throw new Exception("Did I do that?");
		}

		protected override void Creating() { }

		public SampleTestsThens Thens => Then;
	}

	public sealed class SampleTestsThens
	{
		public int Repetitions;
		public int DelayedValue;
	}
}