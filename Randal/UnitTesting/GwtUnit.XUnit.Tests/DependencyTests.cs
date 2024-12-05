using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GwtUnit.XUnit.Tests;

public interface IOther
{
	SingletonClass TestMe();
}

public interface IDidSomething : IDidSomethingElse
{
	void CallMe();
}

public interface IDidSomethingElse
{
	void CallMeAgain();
}

[ExcludeFromCodeCoverage]
public sealed class TransientClass { }

[ExcludeFromCodeCoverage]
public sealed class SingletonClass { }

[ExcludeFromCodeCoverage]
public sealed class ScopedClass
{
	public TransientClass TransientClass { get; }

	public ScopedClass(TransientClass transientClass, IDidSomething didSomething)
	{
		_didSomething = didSomething;
		TransientClass = transientClass;
	}

	public void TakeAction()
	{
		_didSomething.CallMe();
		_didSomething.CallMeAgain();
	}

	private readonly IDidSomething _didSomething;
}

[ExcludeFromCodeCoverage]
public class AnotherSingletonClass
{
	public AnotherSingletonClass(TransientClass transientClass)
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
		Then.Target.TransientClass.Should().NotBeNull();
	}

	[Fact]
	public void ShouldThrowException_WhenCreatingUsingDependencyInjection_GivenBadScopeDependency()
	{
		Given.BadScopeDependency = true;

		WhenLastActionDeferred(Creating);

		DeferredAction.Should().Throw<InvalidOperationException>()
			.WithMessage($"Error while validating the service descriptor " +
				$"'ServiceType: GwtUnit.XUnit.Tests.{nameof(AnotherSingletonClass)} Lifetime: Singleton ImplementationType: GwtUnit.XUnit.Tests.{nameof(AnotherSingletonClass)}': " +
				$"Cannot consume scoped service 'GwtUnit.XUnit.Tests.{nameof(TransientClass)}' from singleton 'GwtUnit.XUnit.Tests.{nameof(AnotherSingletonClass)}'.");
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
		When(Creating, Defer(RequiringOtherInterface));

		DeferredAction.Should().NotThrow();
	}

	[Theory, InlineData(2), InlineData(5)]
	public void CountsShouldBeUnaffected_GivenSingletonMocks(int count)
	{
		When(Creating, Repeat(
			() =>
			{
				Require<IOther>().TestMe();
			}, count));

		RequireMock<IOther>().Verify(x => x.TestMe(), Times.Exactly(count));
	}

	protected override void Creating()
	{
		Services.AddScoped<TransientClass>();
		Services.AddSingleton<SingletonClass>();

		if(GivenOrDefault("BadScopeDependency", false))
			Services.AddSingleton<AnotherSingletonClass>();

		CreateMock<IDidSomething>(mock =>
		{
			if (TryGiven("ThrowException", out bool _))
				mock.Setup(x => x.CallMe()).Throws<InvalidOperationException>();
		});

		CreateMock<IOther>(
			(p, m) =>
			{
				m.Setup(x => x.TestMe()).Returns(p.GetRequiredService<SingletonClass>());
			}, ServiceLifetime.Singleton);

		MockAs<IDidSomethingElse, IDidSomething>();

		Then.Target = BuildTarget<ScopedClass>();
	}

	private void RequiringOtherInterface()
	{
		Require<IOther>();
	}

	private void TakingAction()
	{
		Then.Target.TakeAction();
	}

	public sealed class Thens
	{
		public ScopedClass Target = null!;
	}
}