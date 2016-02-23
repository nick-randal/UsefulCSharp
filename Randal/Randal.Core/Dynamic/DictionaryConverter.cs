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

namespace Randal.Core.Dynamic
{
	public sealed class DictionaryConverter : DynamicEntityConverter
	{
		private static readonly Func<Dictionary<string, object>, object> StandardDictionaryConverter =
			fromDictionary => fromDictionary.ToDictionary(entry => entry.Key, entry => entry.Value);

		public DictionaryConverter()
		{
			AddTypeConverter(typeof (IDictionary<string, object>), StandardDictionaryConverter);
			AddTypeConverter(typeof (Dictionary<string, object>), StandardDictionaryConverter);
			AddTypeConverter(typeof (IReadOnlyDictionary<string, object>), StandardDictionaryConverter);
			AddTypeConverter(typeof (ICollection<KeyValuePair<string, object>>), StandardDictionaryConverter);
			AddTypeConverter(typeof (IEnumerable<KeyValuePair<string, object>>), StandardDictionaryConverter);
		}
	}
}