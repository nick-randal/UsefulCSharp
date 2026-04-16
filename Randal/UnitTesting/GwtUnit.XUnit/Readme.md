## GWT Unit - Given When Then unit testing.

This library provides base classes for unit testing. Using a DSL called Given When Then to facilitate and structure tests for readability and TDD.

GWT derives from AAA (Arrange Act Assert) but strives to provide organization and readability in the tests.

#### Requirements

- **.NET 8.0, 9.0, or 10.0**
- **xUnit v3** (`xunit.v3`) — this library is not compatible with xUnit v2. The trait system, `IAsyncLifetime`, and test runner model differ between the two versions.

#### Given (Arrange)

Setup the data and context for the test. By default `Given` is a dynamic object — properties are created on the fly without pre-defining them. The only downside is lack of intellisense in Visual Studio©. For full intellisense support, use the two-parameter base class with a strongly-typed Givens class (see [Strongly-typed Given](#strongly-typed-given)).

#### When (Act)

Make the When actions composable. `When` takes a params array of `Action` methods and calls `Creating` automatically first unless explicitly provided or `NotCreating` is used. For async tests, use `WhenAsync` instead (see [Async tests](#async-tests)).

#### Then (Assert)

A class where all result context can be stored during a test and asserted on. I prefer using AwesomeAssertions from NuGet. These extension methods provide cleaner test failure messages and make the code more readable.

#### Features

- Exception assertions closer to the origin of the thrown exception
- Given and Then automatically cleaned up after each test
- `When` assumes the `Creating` action will be done first and can be omitted. If `Creating` is provided as an argument it will not be called automatically. Use the `NotCreating` sentinel to skip `Creating` entirely.
- `WhenAsync` for naturally async xUnit v3 test methods
- `XUnitTestBase` is purpose built to accommodate the XUnit framework.
- Dependency Injection through `IServiceCollection` and `IServiceProvider`.
- `[PositiveTest]` and `[NegativeTest]` xUnit trait attributes that tag tests with `Category=Positive` or `Category=Negative` for filtering.

---

### XUnit support

```csharp
using AwesomeAssertions;
using GwtUnit.XUnit;

namespace Someplace
{
	public sealed class TestObjectTests : XUnitTestBase<TestObjectTests.Thens>
	{		
		[Fact, PositiveTest]
		public void ShouldHaveValidInstanceWithValue_WhenCreatingObject_GivenValue123()
		{
			Given.NeededValue = 123;	// Given is dynamic — create any property on the fly

			When(Creating);				// 'When' consumes and executes a list of Actions

			Then.Target.Should().NotBeNull();
		}

		[Fact, PositiveTest]
		public void ShouldHaveFormattedText_WhenFormatting_GivenInstanceWithValue123()
		{
			Given.NeededValue = 123;

			When(Formatting);	// Creating can be omitted — it is called automatically as the first action

			Then.Text.Should().Be("Object said, 123");
		}

		[Fact, NegativeTest]
		public void ShouldThrowFormatException_WhenFormatting_GivenUnescapedOpeningBrace()
		{
			Given.Text = "Hey {name,";

			// Defer captures the last action without executing it, so you can assert on the exception
			When(Defer(Formatting));

			DeferredAction.Should().Throw<FormatException>("Oops");
		}

		[Fact, NegativeTest]
		public void ShouldThrowException_WhenFormattingFails()
		{
			// WhenLastActionDeferred runs all actions except the last, which is stored as DeferredAction
			WhenLastActionDeferred(Formatting);

			DeferredAction.Should().Throw<FormatException>();
		}

		[Fact, PositiveTest]
		public void ShouldRepeatAction_WhenRepeatIncrementing()
		{
			When(Repeat(Incrementing, 10));

			Then.Repetitions.Should().Be(10);
		}

		[Fact, PositiveTest]
		public void ShouldAwaitAsynchronousFunction_WhenTestingAsyncMethod()
		{
			When(Await(Processing));

			// ThenLastTask holds the Task captured by Await() — check status if needed
			ThenLastTask!.IsCompleted.Should().BeTrue();
			Then.DelayedValue.Should().Be(4567);
		}

		protected override void Creating()
		{
			// Check if a dynamic value is defined
			bool hasValue = GivensDefined("NeededValue");

			// Retrieve it with a fallback default
			int value = GivenOrDefault("NeededValue", defaultValue: 0);

			Then.Target = new TestObject(value);
		}

		private void Formatting()
		{
			Then.Text = Then.Target.Format();
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

		public sealed class Thens : IAsyncDisposable // optionally implement IDisposable or IAsyncDisposable for automatic disposal after each test
		{
			public TestObject Target;
			public string Text;
			public int Repetitions;
			public int DelayedValue;

			public ValueTask DisposeAsync()
			{
				// cleanup resources here
				return ValueTask.CompletedTask;
			}
		}
	}
}
```

---

### Async tests

xUnit v3 supports `async Task` test methods natively. Use `WhenAsync` to write async-first tests without bridging back to sync:

```csharp
[Fact, PositiveTest]
public async Task ShouldReturnResult_WhenProcessingAsync()
{
    Given.Input = "hello";

    await WhenAsync(ProcessingAsync);

    Then.Result.Should().Be("HELLO");
}

[Fact, NegativeTest]
public async Task ShouldThrow_WhenProcessingAsyncFails()
{
    // WhenLastAsyncActionDeferred stores the last func as DeferredAsyncAction
    await WhenLastAsyncActionDeferred(SetupAsync, FailingAsync);

    await DeferredAsyncAction!.Invoking(f => f())
        .Should().ThrowAsync<InvalidOperationException>();
}

[Fact, NegativeTest]
public async Task ShouldThrow_WhenUsingDeferAsync()
{
    // DeferAsync stores the async action for later assertion
    When(DeferAsync(FailingAsync));

    await DeferredAsyncAction!.Invoking(f => f())
        .Should().ThrowAsync<InvalidOperationException>();
}

protected override void Creating() { }

private async Task ProcessingAsync()
{
    await Task.Yield();
    Then.Result = GivenOrDefault<string>("Input")!.ToUpper();
}

private async Task SetupAsync()
{
    await Task.Yield();
    Then.SetupRan = true;
}

private static async Task FailingAsync()
{
    await Task.Yield();
    throw new InvalidOperationException("Boom");
}
```

| Method | Use when |
|--------|----------|
| `WhenAsync(params Func<Task>[])` | Async equivalent of `When` — use in `async Task` tests |
| `WhenLastAsyncActionDeferred(params Func<Task>[])` | Async equivalent of `WhenLastActionDeferred` |
| `DeferAsync(Func<Task>)` | Stores an async action as `DeferredAsyncAction` without executing it |
| `Await(Func<Task>)` | Wraps async in a sync `Action` for use inside `When` — prefer `WhenAsync` for new tests |

---

### Dependency injection

`XUnitTestBase<TThens>` exposes an `IServiceCollection` via `Services` for registration and builds the container via `Build()` or `BuildTarget<T>()`.

```csharp
public sealed class MyTest : XUnitTestBase<MyTest.Thens>
{
    [Fact]
    public void ShouldHaveMockEngaged_WhenTakingAction()
    {
        When(TakingAction);

        RequireMock<IDidSomething>().Verify(x => x.CallMe());
        Require<IDidSomething>().Should().NotBeNull();
    }

    protected override void Creating()
    {
        // Register a concrete type
        Services.AddScoped<A>();

        // Register a mock — setup lambda is optional
        CreateMock<IDidSomething>(mock =>
        {
            if (TryGiven("ThrowException", out bool _))
                mock.Setup(x => x.CallMe()).Throws<InvalidOperationException>();
        });

        // BuildTarget resolves TService after building the container
        Then.Target = BuildTarget<B>();
    }

    public sealed class Thens : Thens<B> { }
}
```

#### `Thens<T>` and `Thens<TTarget, TResult>` base classes

`Thens<T>` gives the nested Thens class a typed `Target` property for the system under test. `Thens<TTarget, TResult>` adds a typed `Result` property for when the test also produces a distinct output value:

```csharp
// Single type parameter — Target only
public sealed class Thens : Thens<MyService>
{
    // inherits: public MyService Target { get; set; }
    public string Output = null!;
}

// Two type parameters — Target + Result
public sealed class Thens : Thens<MyService, string>
{
    // inherits: public MyService Target { get; set; }
    // inherits: public string? Result { get; set; }
}
```

#### `Build()` vs `BuildTarget<T>()`

| Method | Use when |
|--------|----------|
| `BuildTarget<T>()` | You want to resolve the root service under test in one call |
| `BuildTarget<T>(Func<IServiceProvider, T>)` | The target requires custom construction logic |
| `Build()` | You just need the container available; resolve services manually via `Require<T>()` or `ServiceProvider` |

```csharp
// Resolve manually after Build()
Build();
Then.Target = Require<MyService>();

// Factory overload — useful when the target needs non-DI arguments
Then.Target = BuildTarget<MyService>(p => new MyService(p.GetRequiredService<IDep>(), "extra-arg"));
```

`ServiceProvider` is available after `Build()` or `BuildTarget<T>()` is called. `Services` throws if accessed after the container is built.

#### `CreateMock<T>` overloads

`Mock<T>` is always registered as a singleton regardless of the service lifetime. This ensures `RequireMock<T>()` and `GetMock<T>()` always return the same instance you configured.

```csharp
// No setup — just register the mock at the given lifetime
CreateMock<IMyService>(ServiceLifetime.Singleton);

// Setup via Action<Mock<T>> — most common form
CreateMock<IMyService>(mock =>
{
    mock.Setup(x => x.DoWork()).Returns(42);
});

// Setup via Action<IServiceProvider, Mock<T>> — when setup depends on another resolved service
CreateMock<IMyService>((provider, mock) =>
{
    var dep = provider.GetRequiredService<IOtherService>();
    mock.Setup(x => x.DoWork()).Returns(dep.Value);
});
```

#### `GetMock<T>` — pre-build mock access

`GetMock<T>()` returns the mock before or after `Build()` is called. Use it when you need to inspect or further configure a mock inside `Creating()` after initial registration:

```csharp
protected override void Creating()
{
    CreateMock<IMyService>(mock =>
    {
        mock.Setup(x => x.GetName()).Returns("default");
    });

    // Further configure the same mock instance before the container is built
    if (GivenOrDefault("UseSpecialName", false))
        GetMock<IMyService>().Setup(x => x.GetName()).Returns("special");

    Then.Target = BuildTarget<MyComponent>();
}
```

#### `AddDependency` overloads

```csharp
// Concrete type self-registration
AddDependency<MyService>();
AddDependency<MyService>(ServiceLifetime.Singleton);

// Interface-to-implementation mapping
AddDependency<IMyService, MyService>();

// Factory registration
AddDependency<IMyService>(p => new MyService(p.GetRequiredService<IConfig>()));
AddDependency<IMyService>(p => new MyService("config"), ServiceLifetime.Singleton);

// Pre-built instance — registered as singleton
var instance = new MyService("specific-config");
AddDependency<IMyService>(instance);
```

#### `MockAs<TAs, TSource>`

Adds an additional interface to an existing mock. `TSource` must already be registered via `CreateMock<TSource>`.

```csharp
CreateMock<IMyService>();
MockAs<IDisposable, IMyService>();

// Both interfaces now resolve to the same underlying mock object
Require<IMyService>();
Require<IDisposable>();
```

---

### Given helper methods

```csharp
// Check if one or more Given values are defined
bool allDefined = GivensDefined("Name", "Age");

// Check if no Given values have been defined at all
bool empty = GivensNone();

// Get a value if defined, otherwise return default(T)
string? name = GivenOrDefault<string>("Name");

// Get a value if defined, otherwise return the provided fallback
int age = GivenOrDefault("Age", defaultValue: 18);

// Try-pattern access
if (TryGiven("Name", out string? value))
    Console.WriteLine(value);
```

---

### Strongly-typed Given

When you want intellisense on Given values, use the two-parameter base class and provide a concrete Givens class:

```csharp
public sealed class MyTest : XUnitTestBase<MyTest.Thens, MyTest.Givens>
{
    [Fact, PositiveTest]
    public void ShouldProcess_WhenGivenName()
    {
        Given.Name = "Alice";   // fully typed — intellisense works

        When(Processing);

        Then.Result.Should().Be("Hello, Alice");
    }

    protected override void Creating() { }

    private void Processing()
    {
        Then.Result = $"Hello, {Given.Name}";
    }

    public sealed class Givens
    {
        public string Name { get; set; } = null!;
    }

    public sealed class Thens
    {
        public string Result = null!;
    }
}
```

---

### Additional helpers

```csharp
// Skip Creating entirely — useful when the test does not need a target
When(NotCreating, SomeOtherAction);

// Resolve an optional service — returns null if not registered
var optional = Optional<IMyOptionalService>();

// Run async code synchronously inside a test action (alternative to WhenAsync)
private void PerformingAsync() => UnAsync(async () =>
{
    Then.Result = await Then.Target.DoWorkAsync();
});

// UnAsync with a return value
private void FetchingResult() => UnAsync(async () =>
{
    Then.Result = await Then.Target.FetchAsync();
});
```
