using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GwtUnit.XUnit.Tests;

[ExcludeFromCodeCoverage]
public sealed class ScopedLifetimeService { }

[ExcludeFromCodeCoverage]
public sealed class SimpleService { }

public sealed class ServiceLifetimeTests : XUnitTestBase<ServiceLifetimeTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldReturnSameInstance_WhenResolvingSingletonMultipleTimes()
	{
		When(Creating);

		Then.FirstSingleton.Should().BeSameAs(Then.SecondSingleton);
	}

	[Fact, PositiveTest]
	public void ShouldReturnDifferentInstances_WhenResolvingTransientMultipleTimes()
	{
		When(Creating);

		Then.FirstTransient.Should().NotBeSameAs(Then.SecondTransient);
	}

	[Fact, PositiveTest]
	public void ShouldReturnSameInstance_WhenResolvingScopedMultipleTimes()
	{
		When(Creating);

		Then.FirstScoped.Should().BeSameAs(Then.SecondScoped);
	}

	[Fact, PositiveTest]
	public void ShouldReturnSameMockInstance_WhenResolvingMockMultipleTimes()
	{
		When(Creating);

		var first = RequireMock<IDidSomething>();
		var second = RequireMock<IDidSomething>();
		first.Should().BeSameAs(second);
	}

	[Fact, PositiveTest]
	public void ShouldInvokeFactory_WhenResolvingServiceRegisteredWithFactory()
	{
		When(Creating);

		Then.FactoryService.Should().NotBeNull();
		Then.FactoryInvoked.Should().BeTrue();
	}

	protected override void Creating()
	{
		Services.AddSingleton<SingletonClass>();
		Services.AddTransient<TransientClass>();
		Services.AddScoped<ScopedLifetimeService>();
		CreateMock<IDidSomething>();
		AddDependency<SimpleService>(_ => { Then.FactoryInvoked = true; return new SimpleService(); });
		Build();

		Then.FirstSingleton = Require<SingletonClass>();
		Then.SecondSingleton = Require<SingletonClass>();
		Then.FirstTransient = Require<TransientClass>();
		Then.SecondTransient = Require<TransientClass>();
		Then.FirstScoped = Require<ScopedLifetimeService>();
		Then.SecondScoped = Require<ScopedLifetimeService>();
		Then.FactoryService = Require<SimpleService>();
	}

	public sealed class Thens
	{
		public SingletonClass FirstSingleton = null!;
		public SingletonClass SecondSingleton = null!;
		public TransientClass FirstTransient = null!;
		public TransientClass SecondTransient = null!;
		public ScopedLifetimeService FirstScoped = null!;
		public ScopedLifetimeService SecondScoped = null!;
		public SimpleService FactoryService = null!;
		public bool FactoryInvoked;
	}
}

public sealed class MockAsChainingTests : XUnitTestBase<MockAsChainingTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldResolveBothInterfaces_WhenMockAsIsUsed()
	{
		When(Creating);

		Require<IDidSomething>().Should().NotBeNull();
		Require<IDidSomethingElse>().Should().NotBeNull();
	}

	[Fact, PositiveTest]
	public void ShouldShareUnderlyingMock_WhenMockAsIsUsed()
	{
		When(Creating);

		var primary = RequireMock<IDidSomething>();
		var secondary = RequireMock<IDidSomethingElse>();
		primary.Should().NotBeNull();
		secondary.Should().NotBeNull();
	}

	[Fact, PositiveTest]
	public void ShouldVerifyCallsOnBothInterfaces_WhenMockAsIsUsed()
	{
		When(TakingAction);

		RequireMock<IDidSomething>().Verify(x => x.CallMe(), Times.Once);
		RequireMock<IDidSomethingElse>().Verify(x => x.CallMeAgain(), Times.Once);
	}

	protected override void Creating()
	{
		Services.AddScoped<TransientClass>();
		CreateMock<IDidSomething>();
		MockAs<IDidSomethingElse, IDidSomething>();
		Then.Target = BuildTarget<ScopedClass>();
	}

	private void TakingAction() => Then.Target.TakeAction();

	public sealed class Thens : Thens<ScopedClass> { }
}

#pragma warning disable CS0618
public sealed class ObsoleteCreateMockSingletonTests : XUnitTestBase<ObsoleteCreateMockSingletonTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldRegisterMock_WhenUsingCreateMockSingletonWithActionSetup()
	{
		When(Creating);

		Require<IDidSomething>().Should().NotBeNull();
		RequireMock<IDidSomething>().Should().NotBeNull();
	}

	[Fact, PositiveTest]
	public void ShouldRegisterMock_WhenUsingCreateMockSingletonWithProviderSetup()
	{
		Given.UseProviderSetup = true;

		When(Creating);

		Require<IDidSomething>().Should().NotBeNull();
		RequireMock<IDidSomething>().Should().NotBeNull();
	}

	protected override void Creating()
	{
		if (GivenOrDefault("UseProviderSetup", false))
			CreateMockSingleton<IDidSomething>((_, _) => { });
		else
			CreateMockSingleton<IDidSomething>(_ => { });

		Build();
	}

	public sealed class Thens { }
}

public sealed class ObsoleteMockSingletonAsTests : XUnitTestBase<ObsoleteMockSingletonAsTests.Thens>
{
	[Fact, PositiveTest]
	public void ShouldAddInterface_WhenUsingMockSingletonAsWithoutSetup()
	{
		When(Creating);

		Require<IDidSomethingElse>().Should().NotBeNull();
	}

	[Fact, PositiveTest]
	public void ShouldAddInterface_WhenUsingMockSingletonAsWithSetup()
	{
		Given.UseSetup = true;

		When(Creating);

		Require<IDidSomethingElse>().Should().NotBeNull();
	}

	protected override void Creating()
	{
		CreateMockSingleton<IDidSomething>(_ => { });

		if (GivenOrDefault("UseSetup", false))
			MockSingletonAs<IDidSomethingElse, IDidSomething>(_ => { });
		else
			MockSingletonAs<IDidSomethingElse, IDidSomething>();

		Build();
	}

	public sealed class Thens { }
}
#pragma warning restore CS0618
