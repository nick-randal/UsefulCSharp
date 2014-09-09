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
		public QuickXmlParser(Parser<IEnumerable<IQuickXmlItem>> qxmlParserDefinition = null)
		{
			_namespacesLookup = new Dictionary<string, XNamespace>(StringComparer.InvariantCultureIgnoreCase);
			_qxmlParserDefinition = qxmlParserDefinition ?? QuickXmlParserDefinition.QxmlItems;
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
						AddAttribute(element, item);
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

		private void AddAttribute(XElement element, IQuickXmlItem item)
		{
			var nameParts = item.Name.Split(':');

			if (nameParts.Length == 1)
			{
				element.Add(new XAttribute(item.Name, item.Value));
				return;
			}

			var part1 = nameParts[0].Trim();
			var part2 = nameParts[1].Trim();
			XNamespace ns;

			if (part1 == "xmlns")
			{
				ns = item.Value.Trim();
				_namespacesLookup.Add(part2, ns);
				element.Add(new XAttribute(XNamespace.Xmlns + part2, item.Value));
				return;
			}

			if(_namespacesLookup.TryGetValue(part1, out ns) == false)
				throw new InvalidDataException("Namespace definition not found for " + item.Name);

			element.Add(new XAttribute(ns + part2, item.Value));
		}

		private void CreateComment(IQuickXmlItem item)
		{
			var comment = item.ToNode();
			var parentElement = _elementLookup.LastOrDefault(i => i.Key < item.Depth).Value;
			parentElement.Add(comment);
		}

		private XElement CreateElement(IQuickXmlItem item, ref XElement root)
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

		private readonly IDictionary<string, XNamespace> _namespacesLookup; 
		private readonly Parser<IEnumerable<IQuickXmlItem>> _qxmlParserDefinition;
		private readonly IDictionary<int, XElement> _elementLookup;
	}

	
}