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
	public interface IStringFormatter
	{
		string With(object source);
		string With(IDictionary<string, object> source);
		string With(NameValueCollection source);
	}

	public sealed class StringFormatter : IStringFormatter
	{
		public StringFormatter(string text, IParser parser = null)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			_parser = parser ?? new NamedFieldParser();
			_textToFormat = text;
		}
	
		public string With(object source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return _parser.Parse(_textToFormat, exp => GetObjectValue(source, exp));
		}

		public string With(IDictionary<string, object> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return _parser.Parse(_textToFormat, exp => GetKeyValue(source, exp));
		}

		public string With(NameValueCollection source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return _parser.Parse(_textToFormat, exp => source[exp] ?? string.Empty);
		}

		private static string GetObjectValue(object source, string field)
		{
			var format = string.Empty;
			var colonIndex = field.IndexOf(':');

			if (colonIndex > 0)
			{
				format = field.Substring(colonIndex + 1);
				field = field.Substring(0, colonIndex);
			}

			try
			{
				if (string.IsNullOrEmpty(format))
					return (DataBinder.Eval(source, field) ?? string.Empty).ToString();

				return DataBinder.Eval(source, field, "{0:" + format + "}") ?? string.Empty;
			}
			catch (HttpException ex)
			{
				throw new FormatException("Binding error", ex);
			}
		}

		private static string GetKeyValue(IDictionary<string, object> lookup, string key)
		{
			var format = string.Empty;
			object value;

			if (lookup.TryGetValue(key, out value) == false)
				value = string.Empty;

			return value.ToString();
		}

		private readonly string _textToFormat;
		private readonly IParser _parser;	
	}
}