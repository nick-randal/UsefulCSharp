using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Testing
{
	[TestClass]
	public sealed class UnitTestBaseTests : UnitTestBase<UnitTestBaseTests.Thens>
	{
		[TestMethod, PositiveTest]
		public void ShouldRepeatAction_WhenRepeatIncrementing()
		{
			When(Repeat(Incrementing, 10));

			Then.Repetitions.Should().Be(10);
		}

		[TestMethod, PositiveTest]
		public void ShouldRepeatActionAtLeastOnce_WhenRepeatIncrementing_GivenInvalidRepetitions()
		{
			When(Repeat(Incrementing, -1234));

			Then.Repetitions.Should().Be(1);
		}

		[TestMethod, PositiveTest]
		public void ShouldAwaitAsynchronousFunction_WhenTestingAsyncMethod()
		{
			When(Await(Processing));

			Then.DelayedValue.Should().Be(4567);
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveActionActionAvailableThroughProperty_WhenExpectingAnException()
		{
			WhenLastActionDeferred(Exploding);

			ThenLastAction.Should().NotBeNull();
			ThenLastAction.ShouldThrow<Exception>("Did I do that?");
		}

		[TestMethod, NegativeTest]
		public void ShouldHaveGivensDefined_GivenValues()
		{
			Given.FirstName = "Bob";
			Given.LastName = "Jones";

			GivensDefined("FirstName", "LastName").Should().BeTrue();
		}

		protected override void Creating()
		{
			
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

		private static void Exploding()
		{
			throw new Exception("Did I do that?");
		}

		public sealed class Thens
		{
			public int Repetitions;
			public int DelayedValue;
		}
	}
}