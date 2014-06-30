using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Core.Strings
{
	public static class StringFormatterExtensions
	{
		internal static Func<string, IStringFormatter> Factory = (target) => new StringFormatter(target);

		public static IStringFormatter Format(this string target)
		{
			return Factory(target);
		}
	}
}
