// Useful C#
// Copyright (C) 2014-2022 Nicholas Randal
//
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace GwtUnit.XUnit;

public abstract class XUnitTestBase<TThens> : XUnitTestBase<TThens, dynamic>
	where TThens : class, new()
{
	protected XUnitTestBase()
	{
		Given = new DynamicEntity(MissingMemberBehavior.ReturnsNull);
		Then = new TThens();
		_services = new ServiceCollection();
	}

	protected XUnitTestBase(Action<IServiceCollection> configureServices) : this()
	{
		configureServices(_services);
	}

	public new readonly dynamic Given;

	protected IServiceCollection Services => _serviceProvider is null
		? _services
		: throw new InvalidOperationException("Cannot add to service collection after target is built.");

	/// <summary>
	/// ServiceProvider will be available after BuildTarget&lt;T>() is called.
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	public IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider used before calling BuildTarget<T>()");

	/// <summary>
	/// Add scoped service to the service collection.
	/// </summary>
	/// <typeparam name="TService"></typeparam>
	/// <returns></returns>
	public XUnitTestBase<TThens> AddDependency<TService>()
		where TService : class
	{
		Services.AddScoped<TService>();
		return this;
	}

	/// <summary>
	/// Add scoped service to the service collection.
	/// </summary>
	/// <typeparam name="TService"></typeparam>
	/// <typeparam name="TImplementation"></typeparam>
	/// <returns></returns>
	public XUnitTestBase<TThens> AddDependency<TService, TImplementation>()
		where TImplementation : class, TService
		where TService : class
	{
		Services.AddScoped<TService, TImplementation>();
		return this;
	}

	/// <summary>
	/// Add scoped service to the service collection.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TService"></typeparam>
	/// <returns></returns>
	public XUnitTestBase<TThens> AddDependency<TService>(Func<IServiceProvider, TService> factory)
		where TService : class
	{
		Services.AddScoped(factory);
		return this;
	}

	/// <summary>
	/// Add scoped service to the service collection.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TService"></typeparam>
	/// <typeparam name="TImplementation"></typeparam>
	/// <returns></returns>
	public XUnitTestBase<TThens> AddDependency<TService, TImplementation>(Func<IServiceProvider, TImplementation> factory)
		where TImplementation : class, TService
		where TService : class
	{
		Services.AddScoped<TService, TImplementation>(factory);
		Services.AddScoped(factory);
		return this;
	}

	/// <summary>
	/// Add singleton mock to the service collection.
	/// </summary>
	/// <param name="setupMock"></param>
	/// <typeparam name="T"></typeparam>
	public void CreateMockSingleton<T>(Action<Mock<T>>? setupMock = null)
		where T : class
	{
		Services.CreateMockSingleton(setupMock);
	}

	/// <summary>
	/// Add singleton mock to the service collection.
	/// </summary>
	/// <param name="setupMock"></param>
	/// <typeparam name="T"></typeparam>
	public void CreateMockSingleton<T>(Action<IServiceProvider, Mock<T>> setupMock)
		where T : class
	{
		Services.CreateMockSingleton(setupMock);
	}

	public void MockSingletonAs<TAs, TSource>(Action<Mock<TAs>>? setupMock = null)
		where TAs : class
		where TSource : class
	{
		Services.CreateMockSingletonAs<TAs, TSource>(setupMock);
	}

	/// <summary>
	/// Add scoped mock to the service collection.
	/// </summary>
	/// <param name="setupMock"></param>
	/// <typeparam name="T"></typeparam>
	public void CreateMock<T>(Action<Mock<T>>? setupMock = null)
		where T : class
	{
		Services.CreateMock(setupMock);
	}

	/// <summary>
	/// Add scoped mock to the service collection.
	/// </summary>
	/// <param name="setupMock"></param>
	/// <typeparam name="T"></typeparam>
	public void CreateMock<T>(Action<IServiceProvider, Mock<T>> setupMock)
		where T : class
	{
		Services.CreateMock(setupMock);
	}

	public void MockAs<TAs, TSource>(Action<Mock<TAs>>? setupMock = null)
		where TAs : class
		where TSource : class
	{
		Services.CreateMockAs<TAs, TSource>(setupMock);
	}

	public Mock<T> RequireMock<T>()
		where T : class
	{
		return ServiceProvider.GetRequiredService<Mock<T>>();
	}

	public T Require<T>()
		where T : class
	{
		return ServiceProvider.GetRequiredService<T>();
	}

	public T? Optional<T>()
		where T : class
	{
		return ServiceProvider.GetService<T>();
	}

	public void Build()
	{
		BuildProvider();
	}

	public TService BuildTarget<TService>(Func<IServiceProvider, TService> factory)
		where TService : class
	{

		_services.AddScoped(factory);
		BuildProvider();
		return _serviceProvider!.GetRequiredService<TService>();
	}

	public TService BuildTarget<TService>()
		where TService : class
	{
		_services.TryAddScoped<TService>();
		BuildProvider();
		return _serviceProvider!.GetRequiredService<TService>();
	}

	/// <summary>
	/// Determine if all provided members have been defined as Given values.
	/// </summary>
	/// <param name="members">A list of property names</param>
	/// <returns>True if all properties specified are defined, otherwise False.</returns>
	public bool GivensDefined(params string[] members)
	{
		return members.Length == 0 || members.All(member => Given.TestForMember(member));
	}

	/// <summary>
	/// Get the value for a Given.
	/// </summary>
	/// <param name="member"></param>
	/// <param name="value"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns>True - if the Given is defined, False - not defined.</returns>
	public bool TryGiven<T>(string member, out T? value)
	{
		if (Given.TestForMember(member))
		{
			value = Given[member];
			return true;
		}

		value = default;
		return false;
	}

	/// <summary>
	/// Return the Given value if defined or default value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="member"></param>
	/// <returns></returns>
	public T? GivenOrDefault<T>(string member)
	{
		return Given.TestForMember(member) ? (T)Given[member] : default;
	}

	/// <summary>
	/// Return the Given value if defined or provided default value.
	/// </summary>
	/// <param name="member"></param>
	/// <param name="defaultValue"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T GivenOrDefault<T>(string member, T defaultValue)
		where T: notnull
	{
		return Given.TestForMember(member) ? (T)Given[member] : defaultValue;
	}

	public override void Dispose()
	{
		_scopedProvider?.Dispose();
		_rootProvider?.Dispose();

		base.Dispose();
	}

	private void BuildProvider()
	{
		if(_rootProvider is not null)
			throw new InvalidOperationException($"ServiceProvider already built. {nameof(Build)} or {nameof(BuildTarget)} can only be called once.");

		_rootProvider = _services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
		_scopedProvider = _rootProvider.CreateScope();
		_serviceProvider = _scopedProvider.ServiceProvider;
	}

	private IServiceProvider? _serviceProvider;
	private ServiceProvider? _rootProvider;
	private IServiceScope? _scopedProvider;
	private readonly ServiceCollection _services;
}