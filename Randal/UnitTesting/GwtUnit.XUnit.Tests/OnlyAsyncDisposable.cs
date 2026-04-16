using System.Diagnostics.CodeAnalysis;

namespace GwtUnit.XUnit.Tests;

[ExcludeFromCodeCoverage]
public sealed class OnlyAsyncDisposable : IAsyncDisposable
{
	public bool Disposed { get; set; }

	public ValueTask DisposeAsync()
	{
		Disposed = true;
		return ValueTask.CompletedTask;
	}
}