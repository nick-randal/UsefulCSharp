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
using Fclp;

namespace Randal.Sql.Scripting.App
{
	public sealed class AppOptionsParser : FluentCommandLineParser<AppOptions>
	{
		public AppOptionsParser()
		{
			SetupHelp("?", "h", "help").Callback(x => Console.WriteLine(x));

			Setup(x => x.LogFolder)
				.As('l', "logFolder")
				.WithDescription(LogFolderHelpText)
				.Required();

			Setup(x => x.OutputFolder)
				.As('o', "outputFolder")
				.WithDescription(OutputFolderHelpText)
				.Required();

			Setup(x => x.Server)
				.As('s', "server")
				.WithDescription(ServerHelpText)
				.Required();

			Setup(x => x.ScriptFunctions)
				.As('f', "functions")
				.WithDescription(ScriptUdfHelp)
				.SetDefault(false);

			Setup(x => x.ScriptStoredProcedures)
				.As('p', "procedures")
				.WithDescription(ScriptSprocsHelp)
				.SetDefault(false);

			Setup(x => x.ScriptTables)
				.As('t', "tables")
				.WithDescription(ScriptTablesHelp)
				.SetDefault(false);

			Setup(x => x.ScriptViews)
				.As('v', "views")
				.WithDescription(ScriptViewsHelp)
				.SetDefault(false);

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
			ExcludeDatabasesHelptText = "Exclude these databases when scripting.",
			ScriptTablesHelp = "Include script definitions for Tables.",
			ScriptSprocsHelp = "Include script definitions for Stored Procedures.",
			ScriptViewsHelp = "Include script definitions for Views.",
			ScriptUdfHelp = "Include script definitions for User Defined Functions."
			;
	}
}