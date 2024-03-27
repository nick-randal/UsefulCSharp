namespace GwtUnit.XUnit;

public abstract class Thens<T>
	where T : class
{
	public T Target { get; set; } = null!;
}