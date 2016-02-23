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
using System.Globalization;
using System.Linq;

namespace Randal.Core.Strings
{
	public struct Text : IComparable, ICloneable, IComparable<string>, IEquatable<string>
	{
		private readonly string _s;

		public Text(string s)
		{
			_s = s;
		}

		public Text(Text t)
		{
			_s = t._s;
		}

		public static implicit operator string(Text t)
		{
			return t._s;
		}
	
		public static implicit operator Text(string s)
		{
			return new Text(s ?? string.Empty);
		}
	
		public static explicit operator int(Text t)
		{
			int value;
			int.TryParse(t._s, out value);
			return value;
		}
	
		public static explicit operator Text(int n)
		{
			return new Text(n.ToString(CultureInfo.CurrentCulture));
		}

		public static Text operator *(Text t, int n)
		{
			return new Text(string.Join(string.Empty, Enumerable.Repeat(t._s, n)));
		}
	
		public override int GetHashCode()
		{
			return _s.GetHashCode();
		}

		public object Clone()
		{
			return new Text((string)_s.Clone());
		}

		public override string ToString()
		{
			return _s;
		}

		public bool Equals(string other)
		{
			return _s.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return _s.Equals(obj);
		}

		public int CompareTo(string other)
		{
			return string.Compare(_s, other, StringComparison.Ordinal);
		}

		public int CompareTo(object obj)
		{
			return _s.CompareTo(obj);
		}

		public static readonly Text Empty = new Text();
	}
}
