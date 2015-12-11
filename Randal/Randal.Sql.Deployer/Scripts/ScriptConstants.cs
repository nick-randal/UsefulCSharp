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
namespace Randal.Sql.Deployer.Scripts
{
	public static class ScriptConstants
	{
		public const string SqlExtension = ".sql";

		public static class Blocks
		{
			public const string 
				Catalog = "catalog",
				Ignore = "ignore",
				Main = "main",
				Need = "need",
				Options = "options",
				Pre = "pre",
				Post = "post"
			;
		}
	}
}