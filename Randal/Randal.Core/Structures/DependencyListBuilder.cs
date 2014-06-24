using System;
using System.Collections.Generic;
using System.Linq;

namespace Randal.Core.Structures
{
	public interface IDependency<in TKey, TValue>
	{
		IReadOnlyList<TValue> OriginalValues { get; }
		List<TValue> BuildDependencyList(Func<TValue, TKey> getKeyFunc, Func<TValue, IEnumerable<TKey>> getDependenciesFunc);
	}

	public sealed class DependencyListBuilder<TKey, TValue> : IDependency<TKey, TValue>
	{
		public DependencyListBuilder(IEnumerable<TValue> values)
		{
			if(values == null)
				throw new ArgumentNullException("values");

			_values = values.ToList();
		}

		public List<TValue> BuildDependencyList(Func<TValue, TKey> getKeyFunc, Func<TValue, IEnumerable<TKey>> getDependenciesFunc)
		{
			var lookup = _values.ToDictionary(getKeyFunc);
			var ordered = new List<TValue>();
			var added = new HashSet<TKey>();

			foreach (var current in _values)
			{
				AddItem(current, ordered, lookup, added, getKeyFunc, getDependenciesFunc);
			}

			return ordered;
		}

		private void AddItem(TValue current, ICollection<TValue> ordered, IReadOnlyDictionary<TKey, TValue> lookup, ISet<TKey> added, Func<TValue, TKey> getKeyFunc, Func<TValue, IEnumerable<TKey>> getDependenciesFunc)
		{
			var currentKey = getKeyFunc(current);
			if (added.Contains(currentKey))
				return;

			ordered.Add(current);
			added.Add(currentKey);

			foreach (var dependencyKey in getDependenciesFunc(current))
			{
				TValue dependency;

				if (lookup.TryGetValue(dependencyKey, out dependency) == false)
					throw new KeyNotFoundException("Item with key '" + currentKey + "' has dependency '" + dependencyKey +
					                               "', which was not found.");

				AddItem(dependency, ordered, lookup, added, getKeyFunc, getDependenciesFunc);
			}
		}

		public IReadOnlyList<TValue> OriginalValues { get { return _values; } }

		private readonly List<TValue> _values;
	}
}