using System.IO;
using System.Xml.Linq;

namespace Randal.QuickXml
{
	public interface IQuickXmlGenerator
	{
		void GenerateQXml(TextWriter writer, XElement element);
	}

	public sealed class QuickXmlGenerator : IQuickXmlGenerator
	{
		public void GenerateQXml(TextWriter writer, XElement root)
		{
			Traverse(writer, root);
		}

		private static void Traverse(TextWriter writer, XElement element, int depth = 0)
		{
			var leadIn = new string('\t', depth);

			writer.Write(leadIn);
			writer.WriteLine(element.Name);

			foreach (var attribute in element.Attributes())
			{
				writer.Write(leadIn);
				writer.WriteLine(attribute.ToString().Replace("=\"", " ").TrimEnd('"'));
			}

			foreach (var node in element.Nodes())
			{
				var childElement = node as XElement;
				var comment = node as XComment;
				var text = node as XText;
				var data = node as XCData;

				if (childElement != null)
				{
					Traverse(writer, childElement, depth + 1);
					continue;
				}

				writer.Write(leadIn);
				if(comment != null)
					writer.WriteLine("\t!{0}", comment.Value);
				if(data != null)
					writer.WriteLine("[{0}]>", data.Value.Replace("[", "[[").Replace("]", "]]"));
				if(text != null)
					writer.WriteLine("\"{0}\"", text.Value.Replace("\"", "\"\""));
			}
		}
	}
}