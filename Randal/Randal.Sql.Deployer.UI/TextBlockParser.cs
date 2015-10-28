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
