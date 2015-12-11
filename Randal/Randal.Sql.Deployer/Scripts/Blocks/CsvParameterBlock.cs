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
using System.Linq;

namespace Randal.Sql.Deployer.Scripts.Blocks
{
	public abstract class CsvParameterBlock : BaseScriptBlock
	{
		protected CsvParameterBlock(string keyword, string text)
			: base(keyword, text)
		{
		}

		public override IReadOnlyList<string> Parse()
		{
			var index = Text.IndexOfAny(EndOfLine);

			var text = index < 0 ? Text : Text.Substring(0, index);

			return text.Split(CatalogSplit, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
		}

		private static readonly char[] CatalogSplit = {','};
		private static readonly char[] EndOfLine = {'\r', '\n'};
	}
}