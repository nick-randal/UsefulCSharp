using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class CatalogBlock : CsvParameterBlock
	{
		public CatalogBlock(string text) : base("catalog", text)
		{
			_catalogPatterns = new List<string>();
		}

		public IReadOnlyList<string> CatalogPatterns { get { return _catalogPatterns; } } 

		public override IReadOnlyList<string> Parse()
		{
			var catalogs = base.Parse();
			var messages = new List<string>();

			foreach (var catalog in catalogs.Select(cat => cat.Trim()))
			{
				if (CatalogPatternValidation.IsMatch(catalog))
				{
					var temp = Regex.Escape(catalog);
					temp = StartOfLine + temp.Replace(Wildcard, WildcardPattern) + EndOfLine;
					_catalogPatterns.Add(temp);
				}
				else
					messages.Add("Invalid catalog pattern '" + catalog + "'");
			}

			IsValid = messages.Count == 0;

			return messages;
		}

		private readonly List<string> _catalogPatterns;
		private const string 
			Wildcard = "%", 
			StartOfLine = "^", 
			EndOfLine = "$",
			WildcardPattern = @"[_\w\d-]*"
		;
		
		private static readonly Regex CatalogPatternValidation = new Regex(@"^[%_\w\d-]+$", 
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
	}
}