using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace GwtUnit.XUnit;

public static class ServiceCollectionExtensions
{
	public static void CreateMock<T>(this IServiceCollection services, Action<Mock<T>>? setupMock = null)
		where T : class
	{
		services.TryAddScoped(_ =>
		{
			var mock = new Mock<T>();
			setupMock?.Invoke(mock);
			return mock;
		});
		services.AddScoped(p => p.GetRequiredService<Mock<T>>().Object);
	}

	public static void CreateMock<T>(this IServiceCollection services, Action<IServiceProvider, Mock<T>> setupMock)
		where T : class
	{
		services.TryAddScoped(p =>
		{
			var mock = new Mock<T>();
			setupMock(p, mock);
			return mock;
		});
		services.AddScoped(p => p.GetRequiredService<Mock<T>>().Object);
	}

	public static void CreateMockAs<TAs, TSource>(this IServiceCollection services, Action<Mock<TAs>>? setupMock = null)
		where TAs : class
		where TSource : class
	{
		services.AddScoped(p =>
		{
			var mock = p.GetRequiredService<Mock<TSource>>().As<TAs>();
			setupMock?.Invoke(mock);
			return mock;
		});

		services.AddScoped(p => p.GetRequiredService<Mock<TAs>>().Object);
	}
}