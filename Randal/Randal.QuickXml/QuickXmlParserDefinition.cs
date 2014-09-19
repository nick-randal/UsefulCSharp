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

using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace Randal.QuickXml
{
	public static class QuickXmlParserDefinition
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

		public static readonly Parser<IQuickXmlItem>
			Attribute =
				(
					from ws in SpaceOrTab.Many().Optional().Named("Attribute: Leading Whitespace")
					from id in Parse.Identifier(Parse.Letter, ElementChars).Named("Attribute: Name")
					from space in Parse.Chars(' ', '\t').AtLeastOnce().Named("Attribute: Value Separator")
					from value in Parse.AnyChar.Except(Parse.Chars('\r', '\n')).AtLeastOnce().Text().Named("Attribute: Value")
					from end in Parse.LineTerminator.Named("Attribute: End")
					select new QAttribute(ws.Get().Count(), id, value)
				),
			BlankLine = 
				(
					from ws in SpaceOrTab.Many().Optional().Named("EmptyLine: Leading Whitespace")
					from end in Parse.LineTerminator.Named("EmptyLine: End")
					select new QBlankLine()
				),
			Comment =
				(
					from ws in SpaceOrTab.Many().Optional().Named("Comment: Leading Whitespace")
					from bang in Parse.Char('!').Once().Named("Comment: Bang")
					from value in Parse.AnyChar.Except(Parse.Chars('\r', '\n')).AtLeastOnce().Text().Named("Comment: Text")
					from end in Parse.LineTerminator.Named("Comment: End")
					select new QComment(ws.Get().Count(), value)
				),
			Content =
				(
					from ws in SpaceOrTab.Many().Optional().Named("Content: Leading Whitespace")
					from qt in QuotedCell.Named("Content: Quoted Text")
					from end in Parse.LineTerminator.Named("Content: End")
					select new QContent(ws.Get().Count(), qt)
				),
			ContentData =
				(
					from ws in SpaceOrTab.Many().Optional().Named("Content: Leading Whitespace")
					from qt in BracketCell.Named("Content: Quoted Text")
					from end in Parse.LineTerminator.Named("Content: End")
					select new QData(ws.Get().Count(), qt)
				),
			Declaration =
				(
					from bang in Parse.Char('?').Once().Named("Declaration: QMark")
					from name in Parse.Decimal.Named("Declaration: Version")
					from space in Parse.Chars(' ', '\t').AtLeastOnce().Named("Declaration: Encoding Separator")
					from encoding in Parse.AnyChar.Except(Parse.Chars('\r', '\n')).AtLeastOnce().Text().Named("Declaration: Encoding")
					from end in Parse.LineTerminator.Named("Comment: End")
					select new QDeclaration(name, encoding)
				),
			Element =
				(
					from ws in SpaceOrTab.Many().Optional().Named("Element: Leading Whitespace")
					from id in Parse.Identifier(Parse.Letter, ElementChars).Named("Element: Name")
					from end in Parse.LineTerminator.Named("Element: End")
					select new QElement(ws.Get().Count(), id)
				)
			;

		public static readonly Parser<IEnumerable<IQuickXmlItem>> QxmlItems =
			from item in ContentData
				.Or(Declaration)
				.Or(Comment)
				.Or(Content)
				.Or(Attribute)
				.Or(Element)
				.Or(BlankLine)
				.Many()
				.End()
			select item;
	}
}