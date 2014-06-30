// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
	public interface IStringFormatter
	{
		string Parse(string text, Func<string, string> getValueForExpressionFunc);
	}

	public sealed class NamedFieldFormatter : IStringFormatter
	{
		public string Parse(string text, Func<string, string> getValueForExpressionFunc)
		{
			using (var reader = new StringReader(text))
			{
				var state = new NamedFieldParserState
				{
					Current = NoMoreText,
					Position = -1,
					State = ParserState.Text,
					GetValueForExpressionFunc = getValueForExpressionFunc
				};

				do
				{
					state.Position++;
					state.Current = reader.Read();

					switch (state.State)
					{
						case ParserState.Text:
							HandleRegularCharacter(state);
							break;
						case ParserState.StartOfExpression:
							HandleStartOfExpression(state);
							break;
						case ParserState.Expression:
							HandleFieldExpression(state);
							break;
						case ParserState.EndOfExpression:
							HandleEndOfExpression(state);
							break;
					}
				} while (state.State != ParserState.EndOfFile);

				return state.FormattedText.ToString();
			}
		}

		private static void HandleRegularCharacter(NamedFieldParserState state)
		{
			switch (state.Current)
			{
				case NoMoreText:
					state.State = ParserState.EndOfFile;
					break;
				case OpenBrace:
					state.State = ParserState.StartOfExpression;
					break;
				case CloseBrace:
					state.State = ParserState.EndOfExpression;
					break;
				default:
					state.FormattedText.Append((char) state.Current);
					break;
			}
		}

		private static void HandleStartOfExpression(NamedFieldParserState state)
		{
			switch (state.Current)
			{
				case NoMoreText:
				case CloseBrace:
					throw new FormatException("Opening brace with no field name specified at position " + state.Position + ".");
				case OpenBrace:
					state.FormattedText.Append((char) OpenBrace);
					state.State = ParserState.Text;
					break;
				default:
					state.FieldExpression.Append((char)state.Current);
					state.State = ParserState.Expression;
					break;
			}
		}

		private static void HandleFieldExpression(NamedFieldParserState state)
		{
			switch (state.Current)
			{
				case NoMoreText:
					throw new FormatException("Opening brace with field expression '" + state.FieldExpression +
					                          "' has no closing brace at position " + state.Position + ".");
				case CloseBrace:
					state.FormattedText.Append(state.GetValueForExpressionFunc(state.FieldExpression.ToString()));
					state.FieldExpression.Clear();
					state.State = ParserState.Text;
					break;
				default:
					state.FieldExpression.Append((char) state.Current);
					break;
			}
		}

		private static void HandleEndOfExpression(NamedFieldParserState state)
		{
			if (state.Current != CloseBrace)
				throw new FormatException("Unescaped closing brace found at position " + state.Position + ".");

			state.FormattedText.Append((char) CloseBrace);
			state.State = ParserState.Text;
		}

		private enum ParserState
		{
			Text,
			StartOfExpression,
			Expression,
			EndOfExpression,
			EndOfFile
		}

		private class NamedFieldParserState
		{
			public ParserState State;
			public int Position;
			public int Current;
			public readonly StringBuilder FormattedText = new StringBuilder();
			public readonly StringBuilder FieldExpression = new StringBuilder();
			public Func<string, string> GetValueForExpressionFunc;
		}

		private const int OpenBrace = '{', CloseBrace = '}', NoMoreText = -1;
	}
}