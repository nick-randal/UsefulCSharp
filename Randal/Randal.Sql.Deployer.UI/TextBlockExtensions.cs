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