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
