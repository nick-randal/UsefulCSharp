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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Randal.Sql.Deployer.Scripts.Blocks
{
	public sealed class CatalogBlock : CsvParameterBlock
	{
		public CatalogBlock(string text) : base(ScriptConstants.Blocks.Catalog, text)
		{
			_catalogPatterns = new List<string>();
		}

		public IReadOnlyList<string> CatalogPatterns
		{
			get { return _catalogPatterns; }
		}

		public override IReadOnlyList<string> Parse()
		{
			var catalogs = base.Parse();
			var messages = new List<string>();

			foreach (var catalog in catalogs.Select(cat => cat.Trim()))
			{
				if (CatalogPatternValidation.IsMatch(catalog))
					_catalogPatterns.Add(catalog);
				else
					messages.Add("Invalid catalog pattern '" + catalog + "'");
			}

			IsValid = messages.Count == 0;

			return messages;
		}

		private readonly List<string> _catalogPatterns;

		private static readonly Regex CatalogPatternValidation = new Regex(@"^[%._\w\d-]+$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);
	}
}