using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace GwtUnit.XUnit;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection CreateMock<T>(this IServiceCollection services,
		ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where T : class
		=> services.CreateMock<T>((_, _) => { }, lifetime);

	public static IServiceCollection CreateMock<T>(this IServiceCollection services, Action<Mock<T>> setupMock,
		ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where T : class
		=> services.CreateMock<T>((_, m) => setupMock.Invoke(m), lifetime);

	public static IServiceCollection CreateMock<T>(this IServiceCollection services,
		Action<IServiceProvider, Mock<T>> setupMock,
		ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where T : class
	{
		services.TryAdd(
			ServiceDescriptor.Describe(
				typeof(Mock<T>),
				p =>
				{
					var mock = new Mock<T>();
					setupMock.Invoke(p, mock);
					return mock;
				},
				ServiceLifetime.Singleton
			)
		);

		services.TryAdd(
			ServiceDescriptor.Describe(typeof(T), p => p.GetRequiredService<Mock<T>>().Object, lifetime)
		);
		return services;
	}

	public static void CreateMockAs<TAs, TSource>(this IServiceCollection services,
		ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where TAs : class
		where TSource : class
		=> services.CreateMockAs<TAs, TSource>(_ => { }, lifetime);

	public static void CreateMockAs<TAs, TSource>(this IServiceCollection services, Action<Mock<TAs>> setupMock,
		ServiceLifetime lifetime = ServiceLifetime.Scoped)
		where TAs : class
		where TSource : class
	{
		services.TryAdd(
			ServiceDescriptor.Describe(
				typeof(Mock<TAs>),
				p =>
				{
					var mock = p.GetRequiredService<Mock<TSource>>().As<TAs>();
					setupMock.Invoke(mock);
					return mock;
				},
				ServiceLifetime.Singleton
			)
		);

		services.TryAdd(
			ServiceDescriptor.Describe(typeof(TAs), p => p.GetRequiredService<Mock<TAs>>().Object, lifetime)
		);
	}

	[Obsolete("Use CreateMock(... ServiceLifetime) instead.")]
	public static void CreateMockSingleton<T>(this IServiceCollection services, Action<Mock<T>>? setupMock = null)
		where T : class
	{
		services.TryAddSingleton(
			_ =>
			{
				var mock = new Mock<T>();
				setupMock?.Invoke(mock);
				return mock;
			}
		);
		services.AddSingleton(p => p.GetRequiredService<Mock<T>>().Object);
	}

	[Obsolete("Use CreateMock(... ServiceLifetime) instead.")]
	public static void CreateMockSingleton<T>(this IServiceCollection services,
		Action<IServiceProvider, Mock<T>> setupMock)
		where T : class
	{
		services.TryAddSingleton(
			p =>
			{
				var mock = new Mock<T>();
				setupMock(p, mock);
				return mock;
			}
		);
		services.AddSingleton(p => p.GetRequiredService<Mock<T>>().Object);
	}

	[Obsolete("Use CreateMockAs(... ServiceLifetime) instead.")]
	public static void CreateMockSingletonAs<TAs, TSource>(this IServiceCollection services,
		Action<Mock<TAs>>? setupMock = null)
		where TAs : class
		where TSource : class
	{
		services.AddSingleton(
			p =>
			{
				var mock = p.GetRequiredService<Mock<TSource>>().As<TAs>();
				setupMock?.Invoke(mock);
				return mock;
			}
		);

		services.AddSingleton(p => p.GetRequiredService<Mock<TAs>>().Object);
	}
}