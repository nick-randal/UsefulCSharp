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
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GwtUnit.XUnit.Tests;

public sealed class XUnitTestBaseTests : XUnitTestBase<XUnitTestBaseTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldRepeatAction_WhenRepeatIncrementing()
	{
		When(Repeat(Incrementing, 10));

		Then.Repetitions.Should().Be(10);
		Then.FirstName.Should().Be("Bob");
		Then.FirstNameDefined.Should().BeFalse();
	}

	[Fact, PositiveTest]
	public void ShouldRepeatActionAtLeastOnce_WhenRepeatIncrementing_GivenInvalidRepetitions()
	{
		When(Repeat(Incrementing, -1234));

		Then.Repetitions.Should().Be(1);
	}

	[Fact, PositiveTest]
	public void ShouldAwaitAsynchronousFunction_WhenTestingAsyncMethod()
	{
		When(Await(Processing));

		Then.DelayedValue.Should().Be(4567);
	}

	[Fact, NegativeTest]
	public void ShouldHaveActionAvailableThroughThenLastActionProperty_WhenExpectingAnException()
	{
		WhenLastActionDeferred(Exploding);

#pragma warning disable CS0618 // Type or member is obsolete
		ThenLastAction.Should().NotBeNull().And.BeSameAs(DeferredAction);
		ThenLastAction.Should().Throw<Exception>("Did I do that?");
#pragma warning restore CS0618 // Type or member is obsolete
	}

	[Fact, NegativeTest]
	public void ShouldHaveDeferredAction_WhenExpectingAnException()
	{
		When(Defer(Exploding));

#pragma warning disable CS0618 // Type or member is obsolete
		DeferredAction.Should().NotBeNull().And.BeSameAs(ThenLastAction);
#pragma warning restore CS0618 // Type or member is obsolete
		DeferredAction.Should().Throw<Exception>("Did I do that?");
	}

	[Fact, NegativeTest]
	public void ShouldHaveGivensDefined_GivenValues()
	{
		Given.FirstName = "Bob";
		Given.LastName = "Jones";

		When(Creating);

		GivensDefined("FirstName", "LastName").Should().BeTrue();
		Then.FirstName.Should().Be("Bob");
		Then.FirstNameDefined.Should().BeTrue();
	}

	[Fact]
	public void ShouldNotThrow_WhenCreating_GivenRegisteredServiceIsOnlyIAsyncDisposable()
	{
		When(Creating);

		var disposable = Require<OnlyAsyncDisposable>();
		disposable.Should().NotBeNull();
		disposable.Disposed.Should().BeFalse();
	}

	public override async ValueTask DisposeAsync()
	{
		var disposable = Require<OnlyAsyncDisposable>();
		await base.DisposeAsync();
		disposable.Disposed.Should().BeTrue();
	}

	protected override void Creating()
	{
		Then.FirstNameDefined = GivensDefined("FirstName");
		Then.FirstName = GivenOrDefault<string>("FirstName", "Bob");

		Services.AddScoped<OnlyAsyncDisposable>();

		Build();
	}

	private void Incrementing()
	{
		Then.Repetitions++;
	}

	private async Task Processing()
	{
		await Task.Delay(1000);

		Then.DelayedValue = 4567;
	}

	private static void Exploding()
	{
		throw new Exception("Did I do that?");
	}

	public sealed class Thens
	{
		public int Repetitions;
		public int DelayedValue;
		public string FirstName = null!;
		public bool FirstNameDefined;
	}
}