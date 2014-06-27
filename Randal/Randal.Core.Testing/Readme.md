Given When Then unit testing pattern.

This library provides a base class to help facilitate and structure the Given When Then patter.  
One of the best features is the use oF the DLR for the Givens.  You can define any Given property on the fly without having previously defined it.  This probably sounds like C# heresy but it's true.

The other focus is around composable When statements.

```csharp
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Someplace
{
	public sealed class TestObjectTests : BaseUnitTest<TestObjectThens>
	{
		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
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
		
		private void Creating()
		{
			Then.Target = new TestObject(Given.NeededValue);
		}
		
		private void Formatting()
		{
			Then.Tex = Then.Target.Format();
		}
	}

	public sealed class TestObjectThens
	{
		public TestObject Target;
		public string Text;
	}
}
```
