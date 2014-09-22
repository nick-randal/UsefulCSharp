using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Core.Strings
{
	public struct Text : IEquatable<string>
	{
		private string _s;
	
		public static implicit operator string(Text t)
		{
			return t._s;
		}
	
		public static implicit operator Text(string s)
		{
			return new Text { _s = s ?? string.Empty };
		}
	
		public static explicit operator int(Text t)
		{
			int value;
			int.TryParse(t._s, out value);
			return value;
		}
	
		public static explicit operator Text(int n)
		{
			return new Text { _s = n.ToString() };
		}
	
		public override int GetHashCode()
		{
			return _s.GetHashCode();
		}
	
		public override string ToString()
		{
			return _s;
		}

		public bool Equals(string other)
		{
			return _s.Equals(other);
		}

		public static readonly Text Empty;
	}
}
