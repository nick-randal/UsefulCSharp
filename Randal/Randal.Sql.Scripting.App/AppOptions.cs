// Useful C#
// Copyright (C) 2014 Nicholas Randal
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

using CommandLine;

namespace Randal.Sql.Scripting.App
{
	public sealed class AppOptions
	{
		[Option('p', "projectFolder", Required = true,
		   HelpText = "The project folder containg the config.json an all associated SQL files.")]
		public string ProjectFolder { get; set; }
	}
}