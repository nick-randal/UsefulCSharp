// // Useful C#
// // Copyright (C) 2014 Nicholas Randal
// // 
// // Useful C# is free software; you can redistribute it and/or modify
// // it under the terms of the GNU General Public License as published by
// // the Free Software Foundation; either version 2 of the License, or
// // (at your option) any later version.
// // 
// // This program is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// // GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Sprache;

namespace Randal.QuickXml
{
	public interface IQuickXmlParser
	{
		XElement ParseToXElement(TextReader reader);
		XElement ParseToXElement(string qxml);
		XDocument ParseToXDocument(TextReader reader);
		XDocument ParseToXDocument(string qxml);
	}

	public sealed class QuickXmlParser : IQuickXmlParser
	{
		public QuickXmlParser(Parser<IEnumerable<IQuickXmlItem>> qxmlParserDefinition = null)
		{
			_namespacesLookup = new Dictionary<string, XNamespace>(StringComparer.InvariantCultureIgnoreCase);
			_qxmlParserDefinition = qxmlParserDefinition ?? QuickXmlParserDefinition.QxmlItems;
			_elementLookup = new Dictionary<int, XElement>();
		}

		public XDocument ParseToXDocument(string qxml)
		{
			using (var reader = new StringReader(qxml))
				return ParseToXDocument(reader);
		}

		public XDocument ParseToXDocument(TextReader reader)
		{
			var items = _qxmlParserDefinition.Parse(reader.ReadToEnd());
			var doc = new XDocument();
			XElement element = null;

			foreach (var item in items)
			{
				if (item.Type == XmlNodeType.XmlDeclaration)
				{
					doc.Declaration = new XDeclaration(item.Name, item.Value, "no");
					continue;
				}

				if (item.Type == XmlNodeType.Element)
				{
					var root = doc.Root;
					element = CreateElement(item, ref root);
					if (doc.Root == null)
						doc.Add(element);
					continue;
				}

				switch (item.Type)
				{
					case XmlNodeType.Attribute:

						AddAttribute(element, item);
						break;
					case XmlNodeType.Text:
						if (element == null)
							throw new InvalidDataException("Item of type " + item.Type + " found before first element.");
						element.Add(new XText(item.Value));
						break;
					case XmlNodeType.CDATA:
						if (element == null)
							throw new InvalidDataException("Item of type " + item.Type + " found before first element.");
						element.Add(new XCData(item.Value));
						break;
					case XmlNodeType.Comment:
						CreateComment(item, doc);
						break;
				}
			}

			return doc;
		}

		public XElement ParseToXElement(string qxml)
		{
			using (var reader = new StringReader(qxml))
				return ParseToXElement(reader);
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
						element.Add(new XText(item.Value));
						break;
					case XmlNodeType.CDATA:
						element.Add(new XCData(item.Value));
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
			if (element == null)
				throw new InvalidDataException("Item of type " + item.Type + " found before first element.");

			var nameParts = new PartName(item.Name);

			if (nameParts.IsTwoPart == false)
			{
				if (item.Name == Constants.Xmlns)
				{
					element.Name = (XNamespace) item.Value + element.Name.LocalName;
				}
				else
					element.Add(new XAttribute(item.Name, item.Value));
				return;
			}

			XNamespace ns;

			if (nameParts.One == Constants.Xmlns)
			{
				ns = item.Value.Trim();
				_namespacesLookup[nameParts.Two] = ns;
				element.Add(new XAttribute(XNamespace.Xmlns + nameParts.Two, item.Value));
				return;
			}

			if (_namespacesLookup.TryGetValue(nameParts.One, out ns) == false)
				throw new InvalidDataException("Namespace definition not found for " + item.Name);

			element.Add(new XAttribute(ns + nameParts.Two, item.Value));
		}

		private void CreateComment(IQuickXmlItem item, XContainer document = null)
		{
			var comment = new XComment(item.Value);
			var parentElement = _elementLookup.LastOrDefault(i => i.Key < item.Depth).Value;

			if (parentElement != null)
				parentElement.Add(comment);
			else if (document != null)
				document.Add(comment);
		}

		private XElement CreateElement(IQuickXmlItem item, ref XElement root)
		{
			XElement element;
			XNamespace qualifiedNamespace = null;
			var nameParts = new PartName(item.Name);

			if (nameParts.IsTwoPart)
			{
				qualifiedNamespace = _namespacesLookup[nameParts.One];
				element = new XElement(nameParts.Two);
			}
			else
				element = new XElement(item.Name);

			if (root == null)
			{
				root = element;
				_elementLookup[0] = root;
				return element;
			}

			AddElementToParent(item, element, qualifiedNamespace);

			return element;
		}

		private void AddElementToParent(IQuickXmlItem item, XElement element, XNamespace qualifiedNamespace)
		{
			if (item.Depth == 0)
				throw new InvalidDataException("Only one root element allowed.  Element '" + item.Name +
				                               "' is invalid, check leading whitespace.");

			_elementLookup[item.Depth] = element;
			var parentElement = _elementLookup.LastOrDefault(i => i.Key < item.Depth).Value;

			if (qualifiedNamespace != null)
			{
				parentElement.Add(element);
				element.Name = qualifiedNamespace + element.Name.LocalName;
			}
			else
			{
				if (parentElement.Name.Namespace != XNamespace.None)
					element.Name = parentElement.Name.Namespace + element.Name.LocalName;
				parentElement.Add(element);
			}
		}

		private readonly IDictionary<string, XNamespace> _namespacesLookup;
		private readonly Parser<IEnumerable<IQuickXmlItem>> _qxmlParserDefinition;
		private readonly IDictionary<int, XElement> _elementLookup;
	}
}