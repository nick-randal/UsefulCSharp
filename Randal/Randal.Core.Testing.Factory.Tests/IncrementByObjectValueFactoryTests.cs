using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Randal.Core.Testing.Factory.Tests
{
	/// <summary>
	/// Created by nrandal on 9/10/2015 10:24:57 AM
	/// </summary>
	[TestClass]
	public sealed class IncrementByObjectValueFactoryTests : UnitTestBase<IncrementByObjectValueFactoryTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldHaveFirstValue_WhenGettingStringValue()
		{
			When(GettingStringValue);

			Then.StringValue.Should().Be("FirstName1");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveTenthValue_WhenIncrementingAndGettingStringValue()
		{
			When(Repeat(Incrementing, 9), GettingStringValue);

			Then.StringValue.Should().Be("FirstName10");
		}

		[TestMethod, PositiveTest]
		public void ShouldHaveFirstValue_WhenIncrementingResettingAndGettingStringValue()
		{
			When(Repeat(Incrementing, 9), Resetting, GettingStringValue);

			Then.StringValue.Should().Be("FirstName1");
		}

		protected override void Creating()
		{
			Then.Target = new IncrementByObjectValueFactory();
		}

		private void Incrementing()
		{
			Then.Target.Increment();
		}

		private void Resetting()
		{
			Then.Target.Reset();
		}

		private void GettingStringValue()
		{
			Then.StringValue = Then.Target.GetString(Given.FieldName ?? "FirstName");
		}

		public sealed class Thens
		{
			public IncrementByObjectValueFactory Target;
			public string StringValue;
		}
	}
}