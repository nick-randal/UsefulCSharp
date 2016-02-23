// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
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
using System.Collections.Generic;
using System.Linq;

namespace Randal.Core.Structures
{
	public sealed class DependencyListBuilder<TKey, TValue> : IDependency<TKey, TValue>
	{
		public DependencyListBuilder(IEnumerable<TValue> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");

			_values = values.ToList();
		}

		public List<TValue> BuildDependencyList(Func<TValue, TKey> getKeyItemFunc,
			Func<TValue, IEnumerable<TKey>> getDependenciesFunc, IEqualityComparer<TKey> comparer = null)
		{
			var dependencyLookup = comparer == null 
				? _values.ToDictionary(getKeyItemFunc) 
				: _values.ToDictionary(getKeyItemFunc, comparer);

			var orderedItems = new List<TValue>();

			foreach (var currentItem in _values)
			{
				if (orderedItems.Contains(currentItem))
					continue;
				AddItem(currentItem, orderedItems, dependencyLookup, getKeyItemFunc, getDependenciesFunc);
			}

			return orderedItems;
		}

		private static void AddItem(
			TValue currentItem, 
			ICollection<TValue> orderedItems, 
			IReadOnlyDictionary<TKey, TValue> dependencyLookup, 
			Func<TValue, TKey> getKeyItemFunc, 
			Func<TValue, IEnumerable<TKey>> getDependenciesFunc, 
			IList<TKey> itemsAlreadyAdded = null)
		{
			itemsAlreadyAdded = itemsAlreadyAdded == null ? new List<TKey>() : new List<TKey>(itemsAlreadyAdded);

			var currentKey = getKeyItemFunc(currentItem);
			if (itemsAlreadyAdded.Contains(currentKey))
			{
				itemsAlreadyAdded.Add(currentKey);
				throw new InvalidOperationException(
					"A circular reference was detected.  Circular path: \r\n" + string.Join(" -> ", itemsAlreadyAdded));
			}

			itemsAlreadyAdded.Add(currentKey);

			foreach (var dependencyKey in getDependenciesFunc(currentItem))
			{
				TValue dependency;

				if (dependencyLookup.TryGetValue(dependencyKey, out dependency) == false)
					throw new KeyNotFoundException("Item with key '" + currentKey + "' has dependency '" + dependencyKey +
					                               "', which was not found.");

				AddItem(dependency, orderedItems, dependencyLookup, getKeyItemFunc, getDependenciesFunc, itemsAlreadyAdded);
			}

			if(orderedItems.Contains(currentItem) == false)
				orderedItems.Add(currentItem);
		}

		public IReadOnlyList<TValue> OriginalValues
		{
			get { return _values; }
		}

		private readonly List<TValue> _values;
	}
}