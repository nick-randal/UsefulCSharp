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
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Cursors = System.Windows.Input.Cursors;

namespace Randal.Sql.Deployer.UI
{
	public sealed class TextBlockParser
	{
		public TextBlockParser(string text)
		{
			_inlines = new List<Inline>();
			Parse(text);
		}

		public IEnumerable<Inline> Inlines
		{
			get { return _inlines;  }
		}

		private void Parse(string text)
		{
			var matches = LinkPattern.Matches(text);

			var index = 0;
			int len;

			foreach (Match m in matches)
			{
				len = m.Index - index;
				if (len > 0)
					ProcessText(text.Substring(index, len));

				_inlines.Add(
					new Run(m.Groups["link"].Value)
					{
						Foreground = Brushes.LightSkyBlue, 
						TextDecorations = TextDecorations.Underline, 
						Cursor = Cursors.Hand
					}
				);

				index = m.Index + m.Length;
			}

			len = text.Length - index;

			if (len > 0)
				ProcessText(text.Substring(index, len));
		}

		private void ProcessText(string text)
		{
			var parts = text.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);

			for (var n = 0; n < parts.Length; n++)
			{
				if((n % 2) != 0)
					_inlines.Add(new LineBreak());

				_inlines.Add(new Run(parts[n]));
			}
		}

		private static readonly Regex LinkPattern = new Regex(@"link:\[(?<link>[^\]]+)]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
		private readonly List<Inline> _inlines;
		private static readonly char[] SplitChars = {'\r', '\n'};
	}
}
