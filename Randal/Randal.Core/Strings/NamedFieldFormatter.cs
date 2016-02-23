// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.IO;
using System.Text;

namespace Randal.Core.Strings
{
	using GetValueForKeyFunc = Func<string, string>;

	public interface IStringFormatter
	{
		string Parse(string text, Func<string, string> getValueForKeyFunc);
	}

	public sealed class NamedFieldFormatter : IStringFormatter
	{
		public string Parse(string text, GetValueForKeyFunc getValueForKeyFunc)
		{
			using (var reader = new StringReader(text))
			{
				var parser = new NamedFieldParser(getValueForKeyFunc);

				do
				{
					parser.Current = reader.Read();

					switch (parser.State)
					{
						case ParsingState.Text:
							HandleRegularCharacter(parser);
							break;
						case ParsingState.StartOfExpression:
							HandleStartOfExpression(parser);
							break;
						case ParsingState.Expression:
							HandleFieldExpression(parser);
							break;
						case ParsingState.EndOfExpression:
							HandleEndOfExpression(parser);
							break;
					}
				} while (parser.State != ParsingState.EndOfFile);

				return parser.FormattedText.ToString();
			}
		}

		private static void HandleRegularCharacter(NamedFieldParser parser)
		{
			switch (parser.Current)
			{
				case NoMoreText:
					parser.State = ParsingState.EndOfFile;
					break;
				case OpenBrace:
					parser.State = ParsingState.StartOfExpression;
					break;
				case CloseBrace:
					parser.State = ParsingState.EndOfExpression;
					break;
				default:
					parser.FormattedText.Append((char) parser.Current);
					break;
			}
		}

		private static void HandleStartOfExpression(NamedFieldParser parser)
		{
			switch (parser.Current)
			{
				case NoMoreText:
				case CloseBrace:
					throw new FormatException("Opening brace with no field name specified at position " + parser.Position + ".");
				case OpenBrace:
					parser.FormattedText.Append((char) OpenBrace);
					parser.State = ParsingState.Text;
					break;
				default:
					parser.FieldExpression.Append((char)parser.Current);
					parser.State = ParsingState.Expression;
					break;
			}
		}

		private static void HandleFieldExpression(NamedFieldParser parser)
		{
			switch (parser.Current)
			{
				case NoMoreText:
					throw new FormatException("Opening brace with field expression '" + parser.FieldExpression +
					                          "' has no closing brace at position " + parser.Position + ".");
				case CloseBrace:
					InsertValueForPlaceholder(parser);
					parser.State = ParsingState.Text;
					break;
				default:
					parser.FieldExpression.Append((char) parser.Current);
					break;
			}
		}

		private static void InsertValueForPlaceholder(NamedFieldParser parser)
		{
			parser.FormattedText.Append(parser.GetValueForKeyFunc(parser.FieldExpression.ToString()));
			parser.FieldExpression.Clear();
		}

		private static void HandleEndOfExpression(NamedFieldParser parser)
		{
			if (parser.Current != CloseBrace)
				throw new FormatException("Unescaped closing brace found at position " + parser.Position + ".");

			parser.FormattedText.Append((char) CloseBrace);
			parser.State = ParsingState.Text;
		}

		private enum ParsingState
		{
			Text,
			StartOfExpression,
			Expression,
			EndOfExpression,
			EndOfFile
		}

		private class NamedFieldParser
		{
			public NamedFieldParser(GetValueForKeyFunc func)
			{
				_current = NoMoreText;
				Position = -1;
				State = ParsingState.Text;
				GetValueForKeyFunc = func;
			}

			public ParsingState State { get; set; }
			public int Position { get; private set; }

			public int Current
			{
				get { return _current; }
				set
				{
					_current = value;
					Position++;
				}
			}

			private int _current;

			public readonly StringBuilder FormattedText = new StringBuilder();
			public readonly StringBuilder FieldExpression = new StringBuilder();
			public readonly GetValueForKeyFunc GetValueForKeyFunc;
		}

		private const int OpenBrace = '{', CloseBrace = '}', NoMoreText = -1;
	}
}