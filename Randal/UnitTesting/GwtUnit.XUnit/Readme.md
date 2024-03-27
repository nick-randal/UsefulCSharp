## GWT Unit - Given When Then unit testing.

This library provides a base classes for unit testing. Using a DSL called Given When Then to facilitate and structure tests for readability and TDD.

The difference of base classes resides with the Given property.  One class uses a dynamic type and the other uses a class you specify.  In both cases the Thens class is specified using generic syntax.  
The dynamic typing allows properties to be created on the the fly without having previously defined it.  The only downside is lack of intellisense in VisualStduio(c).

GWT derives from AAA (Arrange Act Assert) but strives to provide organization and readability in the tests.

#### Given (Arrange)

Setup the data and context for the test.

#### When (Act)

Make the When actions composable.  The **When** method takes params array of Action methods.

#### Then (Assert)

A class where all result context can be stored during a test and can be asserted on.  I prefer using FluentAssertions from NuGet.  These extension methods provide cleaner test failure messages and make the code more readable.

#### Features

- Exception assertions closer to the origin of the thrown exception
- Given and Then automatically cleaned up before each test
- When assumes the *Creating* action will be done first and can be ommitted, however if Creating is provided then it will not be called automatically.
- XUnitTestBase is purpose built to accomodate the XUnit framework.
- Use Dependency Injection through IServiceCollection and IServiceProvider.

#### XUnit support

```csharp
using FluentAssertions;
using Randal.Core.Testing.XUnit;

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
		public void ShouldThrowFormatExcpetion_WhenFormatting_GivenUnescapedOpeningBrace()
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

		protected override Creating()
		{
			// can check if a dynamic value is defined through  GivensDefined("NeededValue",...)

			Then.Target = new TestObject(Given.NeededValue);
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

		public sealed class Thens : IDisposable // optionally define as IDisposable to have automatic disposal after each test
		{
			public TestObject Target;
			public string Text;
			public int Repetitions;
			public int DelayedValue;

			public void Dispose()
			{
				// optionally define as IDisposable to have automatic disposal after each test
			}
		}
	}
}
```

### Make use of dependency injection and helper methods

```c#
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
            if (TryGiven("ThrowException", out bool throwEx))
                mock.Setup(x => x.CallMe()).Throws<InvalidOperationException>();
        });

        Then.Target = BuildTarget<B>();
    }
}
```
