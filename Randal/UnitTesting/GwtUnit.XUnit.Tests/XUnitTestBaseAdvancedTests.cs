using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GwtUnit.XUnit.Tests;

public sealed class WhenBehaviorTests : XUnitTestBase<WhenBehaviorTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldCallCreating_WhenCalledWithNoActions()
	{
		When();

		Then.CreatingCalled.Should().BeTrue();
	}

	[Fact, PositiveTest]
	public void ShouldSkipCreating_WhenNotCreatingIsProvided()
	{
		When(NotCreating);

		Then.CreatingCalled.Should().BeFalse();
	}

	[Fact, PositiveTest]
	public void ShouldExecuteRemainingActions_WhenNotCreatingIsProvided()
	{
		When(NotCreating, RecordingAction);

		Then.CreatingCalled.Should().BeFalse();
		Then.ActionCalled.Should().BeTrue();
	}

	protected override void Creating()
	{
		Then.CreatingCalled = true;
	}

	private void RecordingAction() => Then.ActionCalled = true;

	public sealed class Thens
	{
		public bool CreatingCalled;
		public bool ActionCalled;
	}
}

public sealed class RepeatBehaviorTests : XUnitTestBase<RepeatBehaviorTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldRunExactlyOnce_WhenRepeatCountIsZero()
	{
		When(Repeat(Incrementing, 0));

		Then.Count.Should().Be(1);
	}

	[Fact, PositiveTest]
	public void ShouldRunExactlyOnce_WhenRepeatCountIsNegative()
	{
		When(Repeat(Incrementing, int.MinValue));

		Then.Count.Should().Be(1);
	}

	protected override void Creating() { }

	private void Incrementing() => Then.Count++;

	public sealed class Thens
	{
		public int Count;
	}
}

public sealed class AwaitBehaviorTests : XUnitTestBase<AwaitBehaviorTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldHaveCompletedTask_AfterAwait()
	{
		When(Await(AsyncOperation));

		ThenLastTask.Should().NotBeNull();
		ThenLastTask!.IsCompleted.Should().BeTrue();
		ThenLastTask.IsFaulted.Should().BeFalse();
	}

	[Fact, NegativeTest]
	public void ShouldPropagateException_WhenAwaitedTaskFaults()
	{
		WhenLastActionDeferred(Await(FaultedOperation));

		DeferredAction.Should().Throw<AggregateException>()
			.WithInnerException<InvalidOperationException>()
			.WithMessage("Faulted");
	}

	protected override void Creating() { }

	private async Task AsyncOperation()
	{
		await Task.Delay(1);
		Then.Value = 42;
	}

	private static async Task FaultedOperation()
	{
		await Task.Delay(1);
		throw new InvalidOperationException("Faulted");
	}

	public sealed class Thens
	{
		public int Value;
	}
}

public sealed class ServicesBuildBehaviorTests : XUnitTestBase<ServicesBuildBehaviorTests.Thens>
{
	[Fact, NegativeTest]
	public void ShouldThrow_WhenAddingDependencyAfterBuild()
	{
		When(Creating);

		var f = () => AddDependency<TransientClass>();
		f.Should().Throw<InvalidOperationException>()
			.WithMessage("Cannot add to service collection after target is built.");
	}

	[Fact, NegativeTest]
	public void ShouldThrow_WhenBuildCalledTwice()
	{
		When(Creating);

		var f = () => Build();
		f.Should().Throw<InvalidOperationException>()
			.WithMessage("ServiceProvider already built. Build or BuildTarget can only be called once.");
	}

	[Fact, NegativeTest]
	public void ShouldThrow_WhenServiceProviderAccessedBeforeBuild()
	{
		When(NotCreating);

		IServiceProvider? captured = null;
		var f = () => captured = ServiceProvider;
		f.Should().Throw<InvalidOperationException>()
			.WithMessage("ServiceProvider used before calling BuildTarget<T> or Build.");
	}

	protected override void Creating()
	{
		Build();
	}

	public sealed class Thens { }
}

public sealed class TryGivenBehaviorTests : XUnitTestBase<TryGivenBehaviorTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldReturnFalse_WhenMemberNotDefined()
	{
		When(NotCreating);

		var found = TryGiven<string>("Missing", out var value);
		found.Should().BeFalse();
		value.Should().BeNull();
	}

	[Fact, PositiveTest]
	public void ShouldReturnTrueWithValue_WhenMemberIsDefined()
	{
		Given.Name = "Alice";

		When(NotCreating);

		var found = TryGiven<string>("Name", out var value);
		found.Should().BeTrue();
		value.Should().Be("Alice");
	}

	protected override void Creating() { }

	public sealed class Thens { }
}

public sealed class DisposableThenTests : XUnitTestBase<DisposableThenTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldDisposeThen_WhenThenImplementsIDisposable()
	{
		When(Creating);
		// Disposal verified in DisposeAsync override
	}

	public override async ValueTask DisposeAsync()
	{
		var thens = Then;
		await base.DisposeAsync();
		thens.Disposed.Should().BeTrue();
	}

	protected override void Creating() { }

	public sealed class Thens : IDisposable
	{
		public bool Disposed { get; private set; }
		public void Dispose() => Disposed = true;
	}
}

public sealed class AsyncDisposableThenTests : XUnitTestBase<AsyncDisposableThenTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldDisposeAsyncThen_WhenThenImplementsIAsyncDisposable()
	{
		When(Creating);
		// Disposal verified in DisposeAsync override
	}

	public override async ValueTask DisposeAsync()
	{
		var thens = Then;
		await base.DisposeAsync();
		thens.Disposed.Should().BeTrue();
	}

	protected override void Creating() { }

	public sealed class Thens : IAsyncDisposable
	{
		public bool Disposed { get; private set; }
		public ValueTask DisposeAsync()
		{
			Disposed = true;
			return ValueTask.CompletedTask;
		}
	}
}
