using System.IO;
using System.Xml.Linq;

namespace Randal.QuickXml
{
	public static class QuickXmlExtensions
	{
		public static string ToQuickXmlString(this XElement xml)
		{
			using (var writer = new StringWriter())
			{
				var generator = new QuickXmlGenerator();
				generator.GenerateQuickXml(writer, xml);
				return writer.ToString();
			}
		}

		public static string ToQuickXmlString(this XDocument xml)
		{
			using (var writer = new StringWriter())
			{
				var generator = new QuickXmlGenerator();
				generator.GenerateQuickXml(writer, xml);
				return writer.ToString();
			}
		}
	}
}
