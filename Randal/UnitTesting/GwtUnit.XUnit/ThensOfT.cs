namespace GwtUnit.XUnit;

/// <summary>
/// Base class for the Thens assertions holder. Provides a typed <see cref="Target"/> property
/// for the system under test.
/// </summary>
/// <typeparam name="TTarget">The type of the system under test.</typeparam>
public abstract class Thens<TTarget>
	where TTarget : class
{
	public TTarget Target { get; set; } = null!;
}

/// <summary>
/// Base class for the Thens assertions holder when the test also produces a distinct result value.
/// Provides a typed <see cref="Thens{TTarget}.Target"/> for the system under test
/// and a typed <see cref="Result"/> for the output.
/// </summary>
/// <typeparam name="TTarget">The type of the system under test.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the action under test.</typeparam>
public abstract class Thens<TTarget, TResult>
	where TTarget : class
{
	public TTarget Target { get; set; } = null!;

	public TResult? Result { get; set; }
}
