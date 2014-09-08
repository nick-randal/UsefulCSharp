using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Randal.QuickXml
{
	public static class QxmlParserDefinition
	{
		private static Parser<T> Escaped<T>(Parser<T> following)
		{
			return from escape in DoubleQuote
				   from f in following
				   select f;
		}

		private static Parser<T> Escaped<T>(Parser<T> first, Parser<T> following)
		{
			return from escape in first
				   from f in following
				   select f;
		}

		private static readonly Parser<char>
			SpaceOrTab = Parse.Chars(' ', '\t'),

			//End = Parse.Return('^').End().Or(Space.End()).Or(Space),

			OpenBraket = Parse.Char('['),

			CloseBraket = Parse.Char(']'),

			BracketCellContent =
				Parse.AnyChar
				.Except(CloseBraket.Or(OpenBraket))
				.Or(Escaped(OpenBraket, OpenBraket))
				.Or(Escaped(CloseBraket, CloseBraket)),

			ElementChars = Parse.LetterOrDigit.XOr(Parse.Chars('_', '-', ':', '.')),

			DoubleQuote = Parse.Char('"'),

			QuotedCellContent = Parse.AnyChar.Except(DoubleQuote).Or(Escaped(DoubleQuote))
		;


		public static readonly Parser<string>
			BracketCell =
			(
				from open in OpenBraket
				from content in BracketCellContent.Many().Text()
				from end in CloseBraket
				select content
			),

			QuotedCell =
			(
				from open in DoubleQuote
				from content in QuotedCellContent.Many().Text()
				from end in DoubleQuote
				select content
			)
		;

		public static readonly Parser<IQxmlItem>
			Element =
			(
				from ws in Parse.Optional(SpaceOrTab.Many()).Named("Element: Leading Whitespace")
				from id in Parse.Identifier(Parse.Letter, ElementChars).Named("Element: Name")
				from end in Parse.LineTerminator.Named("Element: End")
				select new QElement(ws.Get().Count(), id)
			),

			Attribute =
			(
				from ws in Parse.Optional(SpaceOrTab.Many()).Named("Attribute: Leading Whitespace")
				from id in Parse.Identifier(Parse.Letter, ElementChars).Named("Attribute: Name")
				from space in Parse.Chars(' ', '\t').AtLeastOnce().Named("Attribute: Value Separator")
				from value in Parse.AnyChar.Except(Parse.Chars('\r', '\n')).AtLeastOnce().Text().Named("Attribute: Value")
				from end in Parse.LineTerminator.Named("Attribute: End")
				select new QAttribute(ws.Get().Count(), id, value)
			),

			Comment =
			(
				from ws in Parse.Optional(SpaceOrTab.Many()).Named("Comment: Leading Whitespace")
				from bang in Parse.Char('!').Once().Named("Comment: Bang")
				from value in Parse.AnyChar.Except(Parse.Chars('\r', '\n')).AtLeastOnce().Text().Named("Comment: Text")
				from end in Parse.LineTerminator.Named("Comment: End")
				select new QComment(ws.Get().Count(), value)
			),

			Content =
			(
				from ws in Parse.Optional(SpaceOrTab.Many()).Named("Content: Leading Whitespace")
				from qt in QuotedCell.Named("Content: Quoted Text")
				from end in Parse.LineTerminator.Named("Content: End")
				select new QContent(ws.Get().Count(), qt)
			),

			ContentData =
			(
				from ws in Parse.Optional(SpaceOrTab.Many()).Named("Content: Leading Whitespace")
				from qt in BracketCell.Named("Content: Quoted Text")
				from end in Parse.LineTerminator.Named("Content: End")
				select new QData(ws.Get().Count(), qt)
			)
		;

		public static readonly Parser<IEnumerable<IQxmlItem>> QxmlItems =
			from item in ContentData
				.Or(Comment)
				.Or(Content)
				.Or(Attribute)
				.Or(Element)
				.Many()
				.End()
			select item;
	}
}