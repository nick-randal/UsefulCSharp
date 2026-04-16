using Moq;

namespace GwtUnit.XUnit.Tests;

// -------------------------------------------------------------------------
// WhenAsync
// -------------------------------------------------------------------------

public sealed class WhenAsyncTests : XUnitTestBase<WhenAsyncTests.Thens>
{
	[Fact, PositiveTest]
	public async Task ShouldExecuteAsyncAction_WhenUsingWhenAsync()
	{
		await WhenAsync(AsyncOperation);

		Then.Value.Should().Be(42);
	}

	[Fact, PositiveTest]
	public async Task ShouldCallCreating_WhenUsingWhenAsync()
	{
		await WhenAsync(AsyncOperation);

		Then.CreatingCalled.Should().BeTrue();
	}

	[Fact, PositiveTest]
	public async Task ShouldExecuteMultipleAsyncActions_WhenUsingWhenAsync()
	{
		await WhenAsync(AsyncOperationA, AsyncOperationB);

		Then.Value.Should().Be(10);
		Then.OtherValue.Should().Be(20);
	}

	protected override void Creating() => Then.CreatingCalled = true;

	private async Task AsyncOperation()
	{
		await Task.Yield();
		Then.Value = 42;
	}

	private async Task AsyncOperationA()
	{
		await Task.Yield();
		Then.Value = 10;
	}

	private async Task AsyncOperationB()
	{
		await Task.Yield();
		Then.OtherValue = 20;
	}

	public sealed class Thens
	{
		public int Value;
		public int OtherValue;
		public bool CreatingCalled;
	}
}

// -------------------------------------------------------------------------
// DeferAsync / WhenLastAsyncActionDeferred
// -------------------------------------------------------------------------

public sealed class AsyncDeferredActionTests : XUnitTestBase<AsyncDeferredActionTests.Thens>
{
	[Fact, NegativeTest]
	public async Task ShouldStoreDeferredAsyncAction_WhenUsingDeferAsync()
	{
		When(DeferAsync(AsyncExploding));

		DeferredAsyncAction.Should().NotBeNull();
		await DeferredAsyncAction!.Invoking(f => f()).Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Boom");
	}

	[Fact, NegativeTest]
	public async Task ShouldDeferLastAsyncAction_WhenUsingWhenLastAsyncActionDeferred()
	{
		await WhenLastAsyncActionDeferred(AsyncSetup, AsyncExploding);

		Then.SetupRan.Should().BeTrue();
		DeferredAsyncAction.Should().NotBeNull();
		await DeferredAsyncAction!.Invoking(f => f()).Should().ThrowAsync<InvalidOperationException>()
			.WithMessage("Boom");
	}

	protected override void Creating() { }

	private async Task AsyncSetup()
	{
		await Task.Yield();
		Then.SetupRan = true;
	}

	private static async Task AsyncExploding()
	{
		await Task.Yield();
		throw new InvalidOperationException("Boom");
	}

	public sealed class Thens
	{
		public bool SetupRan;
	}
}

// -------------------------------------------------------------------------
// AddDependency<T>(T instance)
// -------------------------------------------------------------------------

public sealed class AddDependencyInstanceTests : XUnitTestBase<AddDependencyInstanceTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldResolveInstance_WhenRegisteredDirectly()
	{
		Given.UseInstance = true;

		When(Creating);

		Then.Target.Should().BeSameAs(Then.RegisteredInstance);
	}

	protected override void Creating()
	{
		if (GivenOrDefault("UseInstance", false))
		{
			Then.RegisteredInstance = new SimpleService();
			AddDependency(Then.RegisteredInstance);
		}
		else
		{
			AddDependency<SimpleService>();
		}

		Build();
		Then.Target = Require<SimpleService>();
	}

	public sealed class Thens
	{
		public SimpleService Target = null!;
		public SimpleService RegisteredInstance = null!;
	}
}

// -------------------------------------------------------------------------
// Thens<TTarget, TResult>
// -------------------------------------------------------------------------

public sealed class ThensWithResultTests : XUnitTestBase<ThensWithResultTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldHaveTargetAndResult_WhenUsingThensWithTwoTypeParams()
	{
		When(Creating);

		Then.Target.Should().NotBeNull();
		Then.Result.Should().Be("hello");
	}

	protected override void Creating()
	{
		Services.AddScoped<TransientClass>();
		Services.AddSingleton<SingletonClass>();
		CreateMock<IDidSomething>();
		MockAs<IDidSomethingElse, IDidSomething>();
		Then.Target = BuildTarget<ScopedClass>();
		Then.Result = "hello";
	}

	public sealed class Thens : Thens<ScopedClass, string> { }
}

// -------------------------------------------------------------------------
// GivensNone
// -------------------------------------------------------------------------

public sealed class GivensNoneTests : XUnitTestBase<GivensNoneTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldReturnTrue_WhenNoGivensAreDefined()
	{
		When(NotCreating);

		GivensNone().Should().BeTrue();
	}

	[Fact, PositiveTest]
	public void ShouldReturnFalse_WhenAnyGivenIsDefined()
	{
		Given.Name = "Alice";

		When(NotCreating);

		GivensNone().Should().BeFalse();
	}

	protected override void Creating() { }

	public sealed class Thens { }
}

// -------------------------------------------------------------------------
// GetMock<T> — pre-build access
// -------------------------------------------------------------------------

public sealed class GetMockTests : XUnitTestBase<GetMockTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldReturnMock_WhenAccessingBeforeBuild()
	{
		When(Creating);

		Then.PreBuildMock.Should().NotBeNull();
		Then.PreBuildMock.Should().BeSameAs(RequireMock<IDidSomething>());
	}

	[Fact, PositiveTest]
	public void ShouldAllowAdditionalSetup_WhenAccessingMockBeforeBuild()
	{
		When(TakingAction);

		RequireMock<IDidSomething>().Verify(x => x.CallMe(), Times.Once);
		Then.CallResult.Should().Be(99);
	}

	protected override void Creating()
	{
		Services.AddScoped<TransientClass>();
		Services.AddSingleton<SingletonClass>();
		CreateMock<IDidSomething>(m => m.Setup(x => x.CallMe()).Callback(() => Then.CallResult = 99));
		MockAs<IDidSomethingElse, IDidSomething>();

		// Capture mock before build — should be same instance post-build
		Then.PreBuildMock = GetMock<IDidSomething>();

		Then.Target = BuildTarget<ScopedClass>();
	}

	private void TakingAction()
	{
		Then.Target.TakeAction();
	}

	public sealed class Thens : Thens<ScopedClass>
	{
		public Mock<IDidSomething> PreBuildMock = null!;
		public int CallResult;
	}
}
