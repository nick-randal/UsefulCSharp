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
using System.Collections.Concurrent;

namespace Randal.Core.Threading
{
	public sealed class ObjectPool<T>
	{
		public ObjectPool(Func<T> objectGenerator)
		{
			if (objectGenerator == null)
				throw new ArgumentNullException("objectGenerator");

			_objects = new ConcurrentBag<T>();
			_objectGenerator = objectGenerator;
		}

		public T GetObject()
		{
			T item;
			return _objects.TryTake(out item)
				? item
				: _objectGenerator();
		}

		public void PutObject(T item)
		{
			_objects.Add(item);
		}

		private readonly ConcurrentBag<T> _objects;
		private readonly Func<T> _objectGenerator;
	}
}