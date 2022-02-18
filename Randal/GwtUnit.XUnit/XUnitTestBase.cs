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

namespace GwtUnit.XUnit
{
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

		protected IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider used before calling BuildTarget<T>()");

		protected XUnitTestBase<TThens> AddDependency<TService>() where TService : class
		{
			Services.AddScoped<TService>();
			return this;
		}
		
		protected XUnitTestBase<TThens> AddDependency<TService, TImplementation>() 
			where TImplementation : class, TService
			where TService : class
		{
			Services.AddScoped<TService, TImplementation>();
			return this;
		}
		
		protected XUnitTestBase<TThens> AddDependency<TService>(Func<IServiceProvider, TService> factory) 
			where TService : class
		{
			Services.AddScoped(factory);
			return this;
		}
		
		protected XUnitTestBase<TThens> AddDependency<TService, TImplementation>(Func<IServiceProvider, TImplementation> factory) 
			where TImplementation : class, TService
			where TService : class
		{
			Services.AddScoped<TService, TImplementation>(factory);
			Services.AddScoped(factory);
			return this;
		}

		protected void CreateMock<T>(Action<Mock<T>>? setupMock = null) where T : class
		{
			Services.TryAddScoped(_ =>
			{
				var mock = new Mock<T>();
				setupMock?.Invoke(mock);
				return mock;
			});
			Services.AddScoped(p => p.GetRequiredService<Mock<T>>().Object);
		}

		protected void MockAs<TAs, TSource>(Action<Mock<TAs>>? setupMock = null) where TAs : class where TSource : class
		{
			Services.AddScoped(p =>
			{
				var mock = p.GetRequiredService<Mock<TSource>>().As<TAs>();
				setupMock?.Invoke(mock);
				return mock;
			});
			Services.AddScoped(p => p.GetRequiredService<Mock<TAs>>().Object);
		}

		protected void CreateMock<T>(Action<IServiceProvider, Mock<T>> setupMock) where T : class
		{
			Services.TryAddScoped(p =>
			{
				var mock = new Mock<T>();
				setupMock(p, mock);
				return mock;
			});
			Services.AddScoped(p => p.GetRequiredService<Mock<T>>().Object);
		}

		protected Mock<T> RequireMock<T>() where T : class
		{
			return ServiceProvider.GetRequiredService<Mock<T>>();
		}

		protected T Require<T>() where T : class
		{
			return ServiceProvider.GetRequiredService<T>();
		}

		public T BuildTarget<T>(Func<IServiceProvider, T>? factory = null) where T : class
		{
			if (factory is null)
				_services.AddScoped<T>();
			else
				_services.AddScoped(factory);
			
			_rootProvider = _services.BuildServiceProvider();
			_scopedProvider = _rootProvider.CreateScope();
			_serviceProvider = _scopedProvider.ServiceProvider;
			
			return _serviceProvider.GetRequiredService<T>();
		}

		public override void Dispose()
		{
			_scopedProvider?.Dispose();
			_rootProvider?.Dispose();

			base.Dispose();
		}

		/// <summary>
		/// Determine if all provided members have been defined as Given values.
		/// </summary>
		/// <param name="members">A list of property names</param>
		/// <returns>True if all properties specified are defined, otherwise False.</returns>
		protected bool GivensDefined(params string[] members)
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
		protected bool TryGiven<T>(string member, out T? value)
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
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		protected T? GivenOrDefault<T>(string member, T? defaultValue = default)
		{
			return Given.TestForMember(member) ? (T)Given[member] : defaultValue;
		}
		
		private IServiceProvider? _serviceProvider;
		private ServiceProvider? _rootProvider;
		private IServiceScope? _scopedProvider;
		private readonly ServiceCollection _services;
	}
}