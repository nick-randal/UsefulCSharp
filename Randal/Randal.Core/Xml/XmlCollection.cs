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

using System.Collections.Generic;

namespace Randal.Core.Xml
{
	public sealed class XmlCollection<TItem>
	{
		public XmlCollection()
			: this(null)
		{
		}

		public XmlCollection(IEnumerable<TItem> items)
		{
			Items = items == null ? new List<TItem>() : new List<TItem>(items);
		}
		
		public List<TItem> Items { get; private set; }
		
		public void Add(TItem item)
		{
			Items.Add(item);
		}

		public void Add(object o)
		{
			Items.Add((TItem)o);
		}
	}
}