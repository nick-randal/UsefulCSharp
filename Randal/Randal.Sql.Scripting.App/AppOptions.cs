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

using System;
using System.Collections.Generic;
using Fclp;

namespace Randal.Sql.Scripting.App
{
	public sealed class AppOptions
	{
		public string Server { get; set; }

		public string OutputFolder { get; set; }

		public string LogFolder { get; set; }

		public List<string> IncludeDatabases { get; set; }

		public List<string> ExcludeDatabases { get; set; }
	}

	public sealed class AppOptionsParser : FluentCommandLineBuilder<AppOptions>
	{
		public AppOptionsParser()
		{
			Setup(x => x.Server)
				.As('s', "server")
				.WithDescription(ServerHelpText)
				.Required();

			Setup(x => x.OutputFolder)
				.As('o', "outputFolder")
				.WithDescription(OutputFolderHelpText)
				.Required();

			Setup(x => x.LogFolder)
				.As('l', "logFolder")
				.WithDescription(LogFolderHelpText)
				.Required();

			Setup(x => x.IncludeDatabases)
				.As('i', "include")
				.WithDescription(IncludeDatabasesHelpText)
				.SetDefault(new List<string>());

			Setup(x => x.ExcludeDatabases)
				.As('x', "exclude")
				.WithDescription(ExcludeDatabasesHelptText)
				.SetDefault(new List<string>());
		}

		public const string
			ServerHelpText = @"SQL Server instance to script objects from.",
			OutputFolderHelpText = @"Folder path where schema objects will be scripted.",
			LogFolderHelpText = @"Folder path for the log file to be written.",
			IncludeDatabasesHelpText = "Only include these databases when scripting.",
			ExcludeDatabasesHelptText = "Exclude these databases when scripting."
		;
	}

	/*public sealed class AppOptions
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
	}*/
}