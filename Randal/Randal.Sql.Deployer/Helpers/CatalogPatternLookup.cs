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
using System.Text.RegularExpressions;

namespace Randal.Sql.Deployer.Helpers
{
	public sealed class CatalogPatternLookup
	{
		public CatalogPatternLookup()
		{
			_patternsLookup = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
			_regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase |
			                RegexOptions.ExplicitCapture;
		}

		public int Count
		{
			get { return _patternsLookup.Count; }
		}

		public Regex this[string keyText]
		{
			get
			{
				Regex rgx;
				if (_patternsLookup.TryGetValue(keyText, out rgx))
					return rgx;

				var temp = Regex.Escape(keyText);
				rgx = new Regex(StartOfLine + temp.Replace(Wildcard, WildcardPattern) + EndOfLine, _regexOptions);
				_patternsLookup.Add(keyText, rgx);
				return rgx;
			}
		}

		private readonly RegexOptions _regexOptions;
		private readonly Dictionary<string, Regex> _patternsLookup;

		private const string
			Wildcard = "%",
			StartOfLine = "^",
			EndOfLine = "$",
			WildcardPattern = @"[._\w\d-]*"
			;
	}
}