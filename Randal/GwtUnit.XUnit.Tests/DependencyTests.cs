using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GwtUnit.XUnit.Tests
{

	[ExcludeFromCodeCoverage]
	public class A { }

	public interface IDidSomething
	{
		void CallMe();
	}

	public class B
	{
		private readonly IDidSomething _didSomething;
		public A A { get; }

		public B(A a, IDidSomething didSomething)
		{
			_didSomething = didSomething;
			A = a;
		}

		public void TakeAction()
		{
			_didSomething.CallMe();
		}
	}
	
	public sealed class DependencyTests : XUnitTestBase<DependencyTests.Thens>
	{
		[Fact]
		public void ShouldHaveValidInstance_WhenCreatingUsingDependencyInjection()
		{
			When(Creating);

			Then.Target.Should().NotBeNull();
			Then.Target.A.Should().NotBeNull();
		}

		[Fact]
		public void ShouldHaveMockEngaged_WhenTakingAction()
		{
			When(TakingAction);
			
			RequireMock<IDidSomething>().Verify(x => x.CallMe());
			Require<IDidSomething>().Should().NotBeNull();
		}
		
		[Fact]
		public void ShouldHaveMockSetup_WhenTakingAction()
		{
			Given.ThrowException = true;
			
			When(Defer(TakingAction));
			
			RequireMock<IDidSomething>().Verify(x => x.CallMe(), Times.Never);
			Require<IDidSomething>().Should().NotBeNull();
			DeferredAction.Should().Throw<InvalidOperationException>();
		}
		
		protected override void Creating()
		{
			Services.AddScoped(_ => new A());
			CreateMock<IDidSomething>(mock =>
			{
				if (TryGiven("ThrowException", out bool throwEx))
					mock.Setup(x => x.CallMe()).Throws<InvalidOperationException>();
			});

			Then.Target = BuildTarget<B>();
		}

		public void TakingAction()
		{
			Then.Target.TakeAction();
		}
		
		public sealed class Thens
		{
			public B Target;
		}
	}
}