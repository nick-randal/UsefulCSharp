using System;
using System.Collections.Generic;

namespace Randal.Core.Structures
{
	public interface IDependency<TKey, TValue>
	{
		IReadOnlyList<TValue> OriginalValues { get; }
		List<TValue> BuildDependencyList(Func<TValue, TKey> getKeyFunc, Func<TValue, IEnumerable<TKey>> getDependenciesFunc, IEqualityComparer<TKey> comparer);
	}
}