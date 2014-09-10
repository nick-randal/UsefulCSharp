using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Tests.QuickXml
{
	public static class TestStringExtensions
	{
		public static string NoWhitespace(this string input)
		{
			return input
				.Replace("\r\n", "~")
				.Replace('\t', '*')
				.Replace(' ', '.');
		}
	}
}
