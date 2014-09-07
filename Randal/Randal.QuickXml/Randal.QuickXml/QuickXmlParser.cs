using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public QuickXmlParser()
		{
		}

		public XElement ParseToXElement(TextReader reader)
		{
			var items = Tokens.Parse(reader.ReadToEnd());
			XElement root = null, lastElement = null, element;

			foreach(var item in items)
			{
				if (item.Type == XmlNodeType.Element)
				{
					element = (XElement)item.ToNode();

					if (root == null)
						root = element;

					if (lastElement == null)
						lastElement = element;
					else
						lastElement.Add(element);
				}
			}
			
			return root;
		}

		private static Parser<T> Escaped<T>(Parser<T> following)
		{
			return from escape in DoubleQuote
				   from f in following
				   select f;
		}

		static Parser<T> Escaped<T>(Parser<T> first, Parser<T> following)
		{
			return from escape in first
				   from f in following
				   select f;
		}

		static readonly Parser<char>
			Space = Parse.Chars(' ', '\t'),
			End = Parse.Return('^').End().Or(Space.End()).Or(Space),
			OpenBraket = Parse.Char('['),
			CloseBraket = Parse.Char(']'),
			BracketCellContent =
				Parse.AnyChar
				.Except(CloseBraket.Or(OpenBraket))
				.Or(Escaped(OpenBraket, OpenBraket))
				.Or(Escaped(CloseBraket, CloseBraket)),

			ElementChars = Parse.LetterOrDigit.XOr(Parse.Chars('_', '-', ':', '.'))
		;

		static readonly Parser<char> DoubleQuote = Parse.Char('"');

		static readonly Parser<char> QuotedCellContent =
		  Parse.AnyChar.Except(DoubleQuote).Or(Escaped(DoubleQuote));

		static Parser<string> Token = Parse.LetterOrDigit.Until(End).Text();

		static readonly Parser<string> BracketCell =
			from open in OpenBraket
			from content in BracketCellContent.Many().Text()
			from end in CloseBraket
			select content;

		static readonly Parser<string> QuotedCell =
		  from open in DoubleQuote
		  from content in QuotedCellContent.Many().Text()
		  from end in DoubleQuote
		  select content;
		
		static Parser<IQxmlItem> Identifier =
			from ws in Parse.Optional(Space.Many()).Named("Identifier Leading Whitespace")
			from id in Parse.Identifier(Parse.Letter, ElementChars).Named("Identifier Name")
			select new QElement(ws.Get().Count(), id);

		static Parser<IQxmlItem> Element =
			from id in Identifier.Once().Named("Element Name")
			from end in Parse.LineTerminator.Named("End of Element")
			select id.First();

		static Parser<IQxmlItem> Attribute =
			from id in Identifier.Once().Named("Attribute Name")
			from space in Parse.Chars(' ', '\t').AtLeastOnce().Named("Attribute Value Separator")
			from value in Parse.AnyChar.Except(Parse.Chars('\r', '\n')).AtLeastOnce().Text().Named("Attribute Value")
			from end in Parse.LineTerminator.Named("End of Attribute")
			select new QAttribute(id.First().Depth, id.First().Name, value);

		static Parser<IQxmlItem> Content =
			from ws in Parse.Optional(Space.Many()).Named("Content: Leading Whitespace")
			from qt in QuotedCell.Named("Content: Quoted Text")
			from end in Parse.LineTerminator.Named("Content: End")
			select new QContent(ws.Get().Count(), qt);

		static Parser<IQxmlItem> ContentData =
			from ws in Parse.Optional(Space.Many()).Named("Content: Leading Whitespace")
			from qt in BracketCell.Named("Content: Quoted Text")
			from end in Parse.LineTerminator.Named("Content: End")
			select new QData(ws.Get().Count(), qt);

		static Parser<IEnumerable<IQxmlItem>> Tokens =
			from token in ContentData.Or(Content).Or(Attribute).Or(Element).Many().End()
			select token;
	}
}