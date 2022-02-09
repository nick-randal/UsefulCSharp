// Useful C#
// Copyright (C) 2014-2022 Nicholas Randal
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

namespace GwtUnit.XUnit
{
	public sealed class NullConverter : IDynamicEntityConverter
	{
		public int ConverterCount => 0;

		public bool HasConverters => false;

		public bool TryConversion(Type type, Dictionary<string, object> data, out object? result)
		{
			result = null;
			return false;
		}
	}
}