##Given When Then unit testing pattern.

This library provides a couple base classes to facilitate unit testing. Using a DSL called Given When Then to facilitate and structure tests for readability and TDD.

The difference of base classes resides with the Given property.  One class uses a dynamic type and the other uses a class you specify.  In both cases the Thens class is specified using generic syntax.  The dynamic typing allows properties to be created on the the fly without having previously defined it.  The only downside is lack of intellisense in VisualStduio(c).

GWT is derives from AAA (Arrange Act Assert) but strives to provide organization and cleanliness in the code.

####Given (Arrange)
Setup the data and context for the test.

####When (Act)

Make the when actions composable.  The **When** and **WhenLastActionDeferred** methods take params array of Action methods.

####Then (Assert)
A class where all result context can be stored during a test and can be asserted on.  I prefer using FluentAssertions from NuGet.  These extension methods provide cleaner test failure messages and make the code more readable.

####Features
- Exception assertions closer to the origin of the thrown exception
- Optional overrides for *OnSetup OnTeardown* for less typing
- Given and Then automatically cleaned up before each test
- When assumes the *Creating* action will be done first and can be ommitted, however if Creating is provided then it will not be called automatically.

```csharp
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Someplace
{
	public sealed class TestObjectTests : UnitTestBase<TestObjectThens>
	{
		protected override void OnSetup()
		{
			// Optional : test setup, this is to avoid the extra noise of Attributes,
			// and there are some common setup tasks handled by the base class
		}

		protected override void OnTeardown()
		{
			// Optional : test teardown, this is to avoid the extra noise of Attributes,
			// and there are some common cleanup tasks handled by the base class
		}
		
		[TestMethod, PositiveTest]
		public void ShouldHaveValidInstanceWithValue_WhenCreatingObject_GivenValue123()
		{
			Given.NeededValue = 123;	// Given is a dynamic object, create any number of property values on the fly
			
			When(Creating);				// 'When' consumes and executes a list of Action
			
			Then.Target.Should().NotBeNull();
		}
		
		[TestMethod, PositiveTest]
		public void ShouldHaveFormattedText_WhenFormatting_GivenInstanceWithValue123()
		{
			Given.NeededValue = 123;
			
			When(Formatting);	// Creating can be left out, as it is assumed as our first action
			
			Then.Text.Should().Be("Object said, 123");
		}
		
		[TestMethod, NegativeTest]
		public void ShouldThrowFormatExcpetion_WhenFormatting_GivenUnescapedOpeningBrace()
		{
			Given.Text = "Hey {name,";

			WhenLastActionDeferred(Formatting);

			ThenLastAction.ShouldThrow<FormatException>("Oops");
		}
		
		[TestMethod, PositiveTest]
		public void ShouldRepeatAction_WhenRepeatIncrementing()
		{
			When(Repeat(Incrementing, 10));

			Then.Repetitions.Should().Be(10);
		}

		[TestMethod, PositiveTest]
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
	}

	public sealed class TestObjectThens : IDisposable // optionally define as IDisposable to have automatic disposal after each test
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
```
