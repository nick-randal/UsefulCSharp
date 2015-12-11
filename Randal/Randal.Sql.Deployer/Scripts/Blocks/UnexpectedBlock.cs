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

namespace Randal.Sql.Deployer.Scripts.Blocks
{
	public sealed class UnexpectedBlock : BaseScriptBlock
	{
		public UnexpectedBlock(string keyword, string text) : base(keyword, text)
		{
		}

		public override IReadOnlyList<string> Parse()
		{
			return new List<string> {"Unexpected keyword '" + Keyword + "' found for this block."};
		}
	}
}