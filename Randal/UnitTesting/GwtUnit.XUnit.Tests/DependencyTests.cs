using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GwtUnit.XUnit.Tests;

[ExcludeFromCodeCoverage]
public class A { }

public interface IDidSomething : IDidSomethingElse
{
	void CallMe();
}

public interface IOther
{
	A Get();
}

public interface IDidSomethingElse
{
	void CallMeAgain();
}

[ExcludeFromCodeCoverage]
public class B
{
	public A A { get; }

	public B(A a, IDidSomething didSomething)
	{
		_didSomething = didSomething;
		A = a;
	}

	public void TakeAction()
	{
		_didSomething.CallMe();
		_didSomething.CallMeAgain();
	}

	private readonly IDidSomething _didSomething;
}

[ExcludeFromCodeCoverage]
public class C
{
	public C(A a)
	{

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
	public void ShouldThrowException_WhenCreatingUsingDependencyInjection_GivenBadScopeDependency()
	{
		Given.BadScopeDependency = true;

		WhenLastActionDeferred(Creating);

		DeferredAction.Should().Throw<InvalidOperationException>()
			.WithMessage("Error while validating the service descriptor 'ServiceType: GwtUnit.XUnit.Tests.C Lifetime: Singleton ImplementationType: GwtUnit.XUnit.Tests.C': Cannot consume scoped service 'GwtUnit.XUnit.Tests.A' from singleton 'GwtUnit.XUnit.Tests.C'.");
	}

	[Fact]
	public void ShouldHaveMockEngaged_WhenTakingAction()
	{
		When(TakingAction);

		RequireMock<IDidSomething>().Verify(x => x.CallMe());
		RequireMock<IDidSomething>().Verify(x => x.CallMeAgain());
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

	[Fact]
	public void ShouldNotThrow_WhenResolvingInterface()
	{
		When(Creating, Defer(() => {Require<IOther>();}));

		DeferredAction.Should().NotThrow();
	}

	protected override void Creating()
	{
		Services.AddScoped<A>();

		if(GivenOrDefault("BadScopeDependency", false))
			Services.AddSingleton<C>();

		CreateMock<IDidSomething>(mock =>
		{
			if (TryGiven("ThrowException", out bool _))
				mock.Setup(x => x.CallMe()).Throws<InvalidOperationException>();
		});

		CreateMock<IOther>(
			(p, m) =>
			{
				m.Setup(x => x.Get()).Returns(p.GetRequiredService<A>());
			});

		MockAs<IDidSomethingElse, IDidSomething>();

		Then.Target = BuildTarget<B>();
	}

	private void TakingAction()
	{
		Then.Target.TakeAction();
	}

	public sealed class Thens
	{
		public B Target = null!;
	}
}