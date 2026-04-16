## GWT Unit - Given When Then unit testing.

This library provides base classes for unit testing. Using a DSL called Given When Then to facilitate and structure tests for readability and TDD.

The difference of base classes resides with the Given property.  One class uses a dynamic type and the other uses a class you specify.  In both cases the Thens class is specified using generic syntax.  
The dynamic typing allows properties to be created on the fly without having previously defined them.  The only downside is lack of intellisense in Visual Studio©.

GWT derives from AAA (Arrange Act Assert) but strives to provide organization and readability in the tests.

#### Given (Arrange)

Setup the data and context for the test.

#### When (Act)

Make the When actions composable.  The **When** method takes a params array of Action methods.

#### Then (Assert)

A class where all result context can be stored during a test and can be asserted on.  I prefer using FluentAssertions from NuGet.  These extension methods provide cleaner test failure messages and make the code more readable.

#### Features

- Exception assertions closer to the origin of the thrown exception
- Given and Then automatically cleaned up after each test
- When assumes the *Creating* action will be done first and can be omitted, however if Creating is provided as an argument then it will not be called automatically.  Use the `NotCreating` sentinel action to skip Creating entirely without executing it as an action.
- XUnitTestBase is purpose built to accommodate the XUnit framework.
- Use Dependency Injection through IServiceCollection and IServiceProvider.

#### XUnit support

```csharp
using FluentAssertions;
using GwtUnit.XUnit;

namespace Someplace
{
	public sealed class TestObjectTests : XUnitTestBase<TestObjectTests.Thens>
	{		
		[Fact, PositiveTest]
		public void ShouldHaveValidInstanceWithValue_WhenCreatingObject_GivenValue123()
		{
			Given.NeededValue = 123;	// Given is a dynamic object, create any number of property values on the fly
			
			When(Creating);				// 'When' consumes and executes a list of Action
			
			Then.Target.Should().NotBeNull();
		}
		
		[Fact, PositiveTest]
		public void ShouldHaveFormattedText_WhenFormatting_GivenInstanceWithValue123()
		{
			Given.NeededValue = 123;
			
			When(Formatting);	// Creating can be left out, as it is assumed as our first action
			
			Then.Text.Should().Be("Object said, 123");
		}
		
		[Fact, NegativeTest]
		public void ShouldThrowFormatException_WhenFormatting_GivenUnescapedOpeningBrace()
		{
			Given.Text = "Hey {name,";

			When(Defer(Formatting));

			DeferredAction.Should().Throw<FormatException>("Oops");
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

			Then.DelayedValue.Should().Be(4567);
		}

		[Fact, NegativeTest]
		public void ShouldThrowException_WhenFormattingFails()
		{
			WhenLastActionDeferred(Formatting);

			DeferredAction.Should().Throw<FormatException>();
		}

		protected override void Creating()
		{
			// check if a dynamic value is defined via GivensDefined("NeededValue")
			// or retrieve it with GivenOrDefault<int>("NeededValue", defaultValue: 0)

			Then.Target = new TestObject(GivenOrDefault<int>("NeededValue"));
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

### Make use of dependency injection and helper methods

```csharp
public class MyTest : XUnitTestBase<MyTest.Thens>
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
        Services.AddScoped<A>();
        CreateMock<IDidSomething>(mock =>
        {
            if (TryGiven("ThrowException", out bool _))
                mock.Setup(x => x.CallMe()).Throws<InvalidOperationException>();
        });

        Then.Target = BuildTarget<B>();
    }
    
    public sealed class Thens : Thens<B>
    {
    }
}
```

### Given helper methods

```csharp
// Check if one or more Given values are defined
bool allDefined = GivensDefined("Name", "Age");

// Get a value if defined, otherwise return default(T)
string? name = GivenOrDefault<string>("Name");

// Get a value if defined, otherwise return the provided fallback
int age = GivenOrDefault("Age", defaultValue: 18);

// Try-pattern access
if (TryGiven("Name", out string? value))
    Console.WriteLine(value);
```

### Additional helpers

```csharp
// Skip Creating entirely — useful when testing infrastructure that does not need a target
When(NotCreating, SomeOtherAction);

// Add an extra interface to an existing mock
CreateMock<IMyService>();
MockAs<IDisposable, IMyService>();

// Register a service via factory
AddDependency<IMyService>(_ => new MyService("config"), ServiceLifetime.Singleton);

// Resolve an optional service (returns null if not registered)
var optional = Optional<IMyOptionalService>();

// Run async code synchronously inside a test action
private void PerformingAsync() => UnAsync(async () =>
{
    Then.Result = await Then.Target.DoWorkAsync();
});
```
