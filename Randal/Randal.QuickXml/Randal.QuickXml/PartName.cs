using System;
using System.IO.IsolatedStorage;

namespace Randal.QuickXml
{
	public sealed class PartName
	{
		public PartName(string name)
		{
			var parts = name.Split(Splitter, 2, StringSplitOptions.RemoveEmptyEntries);
			IsTwoPart = parts.Length == 2;
			One = parts[0].Trim();

			if(IsTwoPart)
				Two = parts[1].Trim();
		}

		public bool IsTwoPart { get; private set; }

		public string One { get; private set; }
		public string Two { get; private set; }

		private static readonly char[] Splitter = {':'};
	}
}