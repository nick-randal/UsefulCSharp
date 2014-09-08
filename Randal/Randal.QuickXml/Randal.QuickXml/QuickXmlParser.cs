using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sprache;
using System.Xml;

namespace Randal.QuickXml
{
	public interface IQuickXmlParser
	{
		XElement ParseToXElement(TextReader reader);
	}

	public sealed class QuickXmlParser : IQuickXmlParser
	{
		public QuickXmlParser(Parser<IEnumerable<IQxmlItem>> qxmlParserDefinition)
		{
			_qxmlParserDefinition = qxmlParserDefinition;
			_elementLookup = new Dictionary<int, XElement>();
		}

		public XElement ParseToXElement(TextReader reader)
		{
			var items = _qxmlParserDefinition.Parse(reader.ReadToEnd());
			XElement root = null, element = null;

			foreach (var item in items)
			{
				if (item.Type == XmlNodeType.Element)
				{
					element = CreateElement(item, ref root);
					continue;
				}

				if (element == null)
					throw new InvalidDataException("Item of type " + item.Type + " found before first element.");

				switch (item.Type)
				{
					case XmlNodeType.Attribute:
						element.Add(new XAttribute(item.Name, item.Value));
						break;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						element.Add(item.ToNode());
						break;
					case XmlNodeType.Comment:
						CreateComment(item);
						break;
				}
			}

			return root;
		}

		private void CreateComment(IQxmlItem item)
		{
			var comment = item.ToNode();
			var parentElement = _elementLookup.LastOrDefault(i => i.Key < item.Depth).Value;
			parentElement.Add(comment);
		}

		private XElement CreateElement(IQxmlItem item, ref XElement root)
		{
			var element = (XElement) item.ToNode();

			if (root == null)
			{
				root = element;
				_elementLookup[0] = root;
			}
			else
			{
				if (item.Depth == 0)
					throw new InvalidDataException("Only one root element allowed.  Element '" + item.Name +
					                               "' is invalid, check leading whitespace.");

				_elementLookup[item.Depth] = element;
				var parentElement = _elementLookup.LastOrDefault(i => i.Key < item.Depth).Value;
				parentElement.Add(element);
			}

			return element;
		}

		private readonly Parser<IEnumerable<IQxmlItem>> _qxmlParserDefinition;
		private readonly IDictionary<int, XElement> _elementLookup;
	}

	
}