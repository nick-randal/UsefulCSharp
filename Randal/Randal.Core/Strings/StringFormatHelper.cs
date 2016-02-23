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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI;

namespace Randal.Core.Strings
{
	public interface IStringFormatHelper
	{
		string With(object source);
		string With(IDictionary<string, object> source);
		string With(NameValueCollection source);
	}

	public sealed class StringFormatHelper : IStringFormatHelper
	{
		public StringFormatHelper(string text, IStringFormatter formatter = null)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			_formatter = formatter ?? new NamedFieldFormatter();
			_textToFormat = text;
		}

		public string With(object source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return _formatter.Parse(_textToFormat, exp => GetObjectValue(source, exp));
		}

		public string With(IDictionary<string, object> source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return _formatter.Parse(_textToFormat, exp => GetKeyValue(source, exp));
		}

		public string With(NameValueCollection source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return _formatter.Parse(_textToFormat, exp => source[exp] ?? string.Empty);
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

			if (string.IsNullOrEmpty(format))
				return (DataBinder.Eval(source, field) ?? string.Empty).ToString();

			return DataBinder.Eval(source, field, "{0:" + format + "}");
		}

		private static string GetKeyValue(IDictionary<string, object> lookup, string key)
		{
			object value;

			if (lookup.TryGetValue(key, out value) == false)
				value = string.Empty;

			if (value == null)
				return string.Empty;

			return value.ToString();
		}

		private readonly string _textToFormat;
		private readonly IStringFormatter _formatter;
	}
}