Given When Then unit testing pattern.

This library provides a base class and DSL to help facilitate and structure the Given When Then test pattern.  
One of the best features is the use oF the DLR for the Givens.  You can define any Given property on the fly without having previously defined it.

The other focus is around composable When statements.

- Exception assertions closer to the point of being thrown
- Optional overrides for OnSetup OnTeardown for less typing
- Given and Then automatically cleaned up before each test
- When assumed Creating action will be done first and can be ommitted, however if Creating is provided then it will not be called automatically

```csharp
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Someplace
{
	public sealed class TestObjectTests : BaseUnitTest<TestObjectThens>
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

			ThrowsExceptionWhen(Formatting);

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
