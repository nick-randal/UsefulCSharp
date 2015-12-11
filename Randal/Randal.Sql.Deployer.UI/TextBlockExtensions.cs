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

using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Randal.Sql.Deployer.UI
{
	public static class TextBlockExtensions
	{
		public static TextBlock WriteLine(this TextBlock textBlock, SolidColorBrush foreground, string format, params object[] values)
		{
			var text = string.Format(format, values);

			textBlock.Inlines.Add(new Run(text) { Foreground = foreground });
			textBlock.Inlines.Add(new LineBreak());

			return textBlock;
		}

		public static TextBlock WriteLine(this TextBlock textBlock, string format, params object[] values)
		{
			var text = string.Format(format, values);

			textBlock.Inlines.AddRange(new TextBlockParser(text).Inlines);
			textBlock.Inlines.Add(new LineBreak());

			return textBlock;
		}

		public static TextBlock LineBreak(this TextBlock textBlock)
		{
			textBlock.Inlines.Add(new LineBreak());
			return textBlock;
		}
	}
}