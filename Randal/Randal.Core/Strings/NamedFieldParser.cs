using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace Randal.Core.Strings
{
	public interface IParser
	{
		string Parse(string text, Func<string, string> getValueForExpressionFunc);
	}

	public sealed class NamedFieldParser : IParser
	{
		public string Parse(string text, Func<string, string> getValueForExpressionFunc)
		{
			using (var _reader = new StringReader(text))
			{
				Reset();
				GetValueForExpressionFunc = getValueForExpressionFunc;

				var formattedText = new StringBuilder();
				var fieldExpression = new StringBuilder();

				do
				{
					Position++;
					Current = _reader.Read();

					if (State == ParserState.Text)
					{
						HandleRegularCharacter(formattedText);
					}
					else if (State == ParserState.StartOfExpression)
					{
						HandleStartOfExpression(formattedText, fieldExpression);
					}
					else if (State == ParserState.Expression)
					{
						HandleFieldExpression(formattedText, fieldExpression);
					}
					else if (State == ParserState.EndOfExpression)
					{
						HandleEndOfExpression(formattedText);
					}

				} while (State != ParserState.EOF);

				return formattedText.ToString();
			}
		}

		private void Reset()
		{
			Position = -1;
			Current = -1;
			State = ParserState.Text;
		}

		private void HandleRegularCharacter(StringBuilder formattedText)
		{
			switch (Current)
			{
				case NoMoreText:
					State = ParserState.EOF;
					break;
				case OpenBrace:
					State = ParserState.StartOfExpression;
					break;
				case CloseBrace:
					State = ParserState.EndOfExpression;
					break;
				default:
					formattedText.Append((char)Current);
					break;
			}
		}

		private void HandleStartOfExpression(StringBuilder formattedText, StringBuilder fieldExpression)
		{
			switch (Current)
			{
				case NoMoreText:
				case CloseBrace:
					throw new FormatException("Opening brace with no field name specified at position " + Position + ".");
				case OpenBrace:
					formattedText.Append((char)OpenBrace);
					State = ParserState.Text;
					break;
				default:
					fieldExpression.Append((char)Current);
					State = ParserState.Expression;
					break;
			}
		}

		private void HandleFieldExpression(StringBuilder formattedText, StringBuilder fieldExpression)
		{
			switch (Current)
			{
				case NoMoreText:
					throw new FormatException("Opening brace with field expression '" + fieldExpression.ToString() + "' has no closing brace at position " + Position + ".");
				case CloseBrace:
					formattedText.Append(GetValueForExpressionFunc(fieldExpression.ToString()));
					fieldExpression.Clear();
					State = ParserState.Text;
					break;
				default:
					fieldExpression.Append((char)Current);
					break;
			}
		}

		private void HandleEndOfExpression(StringBuilder formattedText)
		{
			if (Current != CloseBrace)
				throw new FormatException("Unescaped closing brace found at position " + Position + ".");

			formattedText.Append((char)CloseBrace);
			State = ParserState.Text;
		}

		private int Current, Position;
		private ParserState State;
		private Func<string, string> GetValueForExpressionFunc;

		private enum ParserState
		{
			Text,
			StartOfExpression,
			Expression,
			EndOfExpression,
			EOF
		}

		private const int
			OpenBrace = '{',
			CloseBrace = '}',
			NoMoreText = -1
		;
	}
}
