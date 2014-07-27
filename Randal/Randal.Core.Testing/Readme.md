Given When Then unit testing pattern.

This library provides a base class to help facilitate and structure the Given When Then patter.  
One of the best features is the use oF the DLR for the Givens.  You can define any Given property on the fly without having previously defined it.  This probably sounds like C# heresy but it's true.

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
			// Optional : est setup
		}

		protected override void OnTeardown()
		{
			// Optional : test teardown
		}
		
		[TestMethod]
		public void ShouldHaveValidInstanceWithValueWhenCreatingObjectGivenValue123()
		{
			Given.NeededValue = 123;	// Given is a dynamic object, create any number of property values on the fly
			
			When(Creating);				// When consumes and executes a list of Action
			
			Then.Target.Should().NotBeNull();
		}
		
		[TestMethod]
		public void ShouldHaveFormattedTextWhenFormattingGivenInstanceWithValue123()
		{
			Given.NeededValue = 123;
			
			When(Creating, Formatting);
			
			Then.Text.Should().Be("Object said, 123");
		}
		
		[TestMethod]
		public void ShouldThrowFormatExcpetionWhenFormattingWithUnescapedOpeningBrace()
		{
			Given.Text = "Hey {name,";

			ThrowsExceptionWhen(Formatting);

			ThenLastAction.ShouldThrow<FormatException>("Oops");
		}
		
		protected override Creating()
		{
			// can check if a dynamic value is defined through  GivensDefined("NeededValue",...)

			Then.Target = new TestObject(Given.NeededValue);
		}
		
		private void Formatting()
		{
			Then.Tex = Then.Target.Format();
		}
	}

	public sealed class TestObjectThens : IDisposable // optionally define as IDisposable to have automatic disposal after each test
	{
		public TestObject Target;
		public string Text;

		public void Dispose()
		{
			// optionally define as IDisposable to have automatic disposal after each test
		}
	}
}
```
