using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Randal.Utilities.Sql.Deployer.Helpers
{
	public sealed class CatalogPatternLookup
	{
		public CatalogPatternLookup()
		{
			_patternsLookup = new Dictionary<string, Regex>(StringComparer.InvariantCultureIgnoreCase);
			_regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
		}

		public int Count { get { return _patternsLookup.Count; } }

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