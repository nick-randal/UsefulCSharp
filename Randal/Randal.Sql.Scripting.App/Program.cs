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
using System.IO;
using Randal.Logging;

namespace Randal.Sql.Scripting.App
{
	public sealed class Program
	{
		private static int Main(string[] args)
		{
			var options = ParseCommandLineArguments(args);
			if (options == null)
				return 2;

			using (var logger = new AsyncFileLogger(new FileLoggerSettings(options.LogFolder, "SQL Scripter")))
			{
				LogHeader(logger, options);

				var scriptFileManager = new ScriptFileManager(Path.Combine(options.OutputFolder, options.Server));
				var server = new ServerWrapper(options.Server);

				var scripter =
					new Scripter(server, scriptFileManager, logger)
						.IncludeTheseDatabases(options.IncludeDatabases.ToArray())
						.ExcludedTheseDatabases(options.ExcludeDatabases.ToArray());

				ConfigureScriptingSources(options, scripter);

				scripter.DumpScripts();

				logger.Add("DONE".ToLogEntry());
			}

			return -1;
		}

		private static void ConfigureScriptingSources(AppOptions options, Scripter scripter)
		{
			if (options.ScriptFunctions)
			{	
				scripter.AddSources(
					new ScriptingSource("Functions", (srvr, db) => srvr.GetUserDefinedFunctions(db))
				);
			}

			if (options.ScriptStoredProcedures)
			{
				scripter.AddSources(
					new ScriptingSource("Sprocs", (srvr, db) => srvr.GetStoredProcedures(db))
				);
			}

			if (options.ScriptTables)
			{	
				scripter.AddSources(
					new ScriptingSource("Tables", (srvr, db) => srvr.GetTables(db))
				);
			}

			if (options.ScriptViews)
			{	
				scripter.AddSources(
					new ScriptingSource("Views", (srvr, db) => srvr.GetViews(db))	
				);
			}
		}

		private static AppOptions ParseCommandLineArguments(string[] args)
		{
			var parser = new AppOptionsParser();
			var results = parser.Parse(args);

			if (results.HelpCalled)
				return null;

			if (!results.HasErrors)
				return parser.Object;

			Console.WriteLine(results.ErrorText);
			return null;
		}

		private static void LogHeader(ILogger logger, AppOptions options)
		{
			logger.Add("SQL Scripting Application".ToLogEntry());

			var lines = new List<string>
			{
				string.Concat("Server           ", options.Server),
				string.Concat("Output Folder    ", options.OutputFolder),
				string.Concat("Log Folder       ", options.LogFolder),
				string.Concat("Included DBs     ", string.Join(", ", options.IncludeDatabases)),
				string.Concat("Excluded DBs     ", string.Join(", ", options.ExcludeDatabases)),
				string.Concat("Script UDFs      ", options.ScriptFunctions),
				string.Concat("Script Sprocs    ", options.ScriptStoredProcedures),
				string.Concat("Script Tables    ", options.ScriptTables),
				string.Concat("Script Views     ", options.ScriptViews),
			};

			lines.ForEach(l => logger.Add(l.ToLogEntryNoTs()));
		}
	}
}