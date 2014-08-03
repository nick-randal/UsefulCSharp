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

using System.Collections.Generic;
using CommandLine;

namespace Randal.Sql.Scripting.App
{
	public sealed class AppOptions
	{
		[Option('s', "server", Required = true, HelpText = ServerHelpText)]
		public string Server { get; set; }

		[Option('o', "outputFolder", Required = true, HelpText = OutputFolderHelpText)]
		public string ProjectFolder { get; set; }

		[Option('l', "logFolder", Required = true, HelpText = LogFolderHelpText)]
		public string LogFolder { get; set; }

		[Option('i', "include", Required = false, HelpText = IncludeDatabasesHelpText)]
		public IEnumerable<string> IncludeDatabases { get; set; }

		[Option('x', "exclude", Required = false, HelpText = ExcludeDatabasesHelptText)]
		public IEnumerable<string> ExcludeDatabases { get; set; }

		public const string 
			ServerHelpText = @"SQL Server instance to script objects from.",
			OutputFolderHelpText = @"Folder path where schema objects will be scripted.",
			LogFolderHelpText = @"Folder path for the log file to be written.",
			IncludeDatabasesHelpText = "Only include these databases when scripting.",
			ExcludeDatabasesHelptText = "Exclude these databases when scripting."
		;
	}
}