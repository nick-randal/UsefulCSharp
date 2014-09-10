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

using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Randal.QuickXml
{
	public interface IQuickXmlGenerator
	{
		void GenerateQuickXml(TextWriter writer, XElement element);
		void GenerateQuickXml(TextWriter writer, XDocument element);
	}

	public sealed class QuickXmlGenerator : IQuickXmlGenerator
	{
		public void GenerateQuickXml(TextWriter writer, XDocument xml)
		{
			var nav = xml.CreateNavigator();

			Traverse(writer, nav);
		}

		public void GenerateQuickXml(TextWriter writer, XElement xml)
		{
			var nav = xml.CreateNavigator();

			Traverse(writer, nav);
		}

		private static void Traverse(TextWriter writer, XPathNavigator nav, int depth = 0)
		{
			var leadIn = new string(Constants.Tab, depth);

			if (nav.NodeType == XPathNodeType.Root)
				nav.MoveToFirstChild();

			do
			{
				switch (nav.NodeType)
				{
					case XPathNodeType.Element:
						WriteElement(writer, nav, depth, leadIn);
						break;
					case XPathNodeType.Text:
						WriteTextData(writer, nav, leadIn);
						break;
					case XPathNodeType.Comment:
						WriteComment(writer, nav, leadIn);
						break;
					default:
						throw new InvalidDataException("Encountered unsupported node type of " + nav.NodeType);
				}
			} while (nav.MoveToNext());
		}

		private static void WriteElement(TextWriter writer, XPathNavigator nav, int depth, string leadIn)
		{
			writer.Write(leadIn);
			writer.WriteLine(nav.Name);

			WriteNamespaces(writer, nav, leadIn);

			if (nav.HasAttributes)
				WriteAttributes(writer, nav, leadIn);

			if (!nav.HasChildren)
				return;

			var childNav = nav.CreateNavigator();
			childNav.MoveToFirstChild();
			Traverse(writer, childNav, depth + 1);
		}

		private static void WriteComment(TextWriter writer, XPathItem nav, string leadIn)
		{
			writer.Write(leadIn);
			writer.Write(Constants.Exclamation);
			writer.WriteLine(nav.Value.Replace(Constants.NewLine, Constants.Space).Replace(Constants.CReturn, Constants.Space));
		}

		private static void WriteTextData(TextWriter writer, XPathNavigator nav, string leadIn)
		{
			writer.Write(leadIn);
			if (nav.UnderlyingObject is XCData)
			{
				var value = nav.Value.Replace(Constants.LBracketStr, Constants.EscapedLBracket)
					.Replace(Constants.RBracketStr, Constants.EscapedRBracket);
				WriteEnclosed(writer, Constants.LBracket, value, Constants.RBracket);
			}
			else
			{
				var value = nav.Value.Replace(Constants.LBracketStr, Constants.EscapedDoubleQuote);
				WriteEnclosed(writer, Constants.DoubleQuote, value);
			}
		}

		private static void WriteEnclosed(TextWriter writer, char begin, string text, char? end = null)
		{
			writer.Write(begin);
			writer.Write(text);
			writer.WriteLine(end ?? begin);
		}

		private static void WriteNamespaces(TextWriter writer, IXmlNamespaceResolver nav, string leadIn)
		{
			var namespaces = nav.GetNamespacesInScope(XmlNamespaceScope.Local);

			foreach (var n in namespaces)
			{
				writer.Write(leadIn);
				writer.Write(Constants.Xmlns);
				if (string.IsNullOrWhiteSpace(n.Key) == false)
				{
					writer.Write(Constants.Colon);
					writer.Write(n.Key);
				}
				writer.Write(Constants.Space);
				writer.WriteLine(n.Value);
			}
		}

		private static void WriteAttributes(TextWriter writer, XPathNavigator nav, string leadIn)
		{
			nav.MoveToFirstAttribute();

			do
			{
				writer.Write(leadIn);
				writer.Write(nav.Name);
				writer.Write(Constants.Space);
				writer.WriteLine(nav.Value);
			} while (nav.MoveToNextAttribute());

			nav.MoveToParent();
		}
	}
}