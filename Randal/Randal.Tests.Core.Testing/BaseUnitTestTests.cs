using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Randal.Core.Testing.UnitTest;

namespace Randal.Tests.Core.Testing
{
	[TestClass]
	public sealed class BaseUnitTestTests : BaseUnitTest<BaseUnitTestThens>
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
			ThrowsExceptionWhen(Exploding);

			ThenLastAction.Should().NotBeNull();
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

		private void Exploding()
		{
			throw new Exception("Did I do that?");
		}
	}

	public sealed class BaseUnitTestThens
	{
		public int Repetitions;
		public int DelayedValue;
	}
}
