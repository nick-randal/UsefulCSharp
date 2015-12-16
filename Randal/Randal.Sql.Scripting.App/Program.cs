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
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Randal.Logging;

namespace Randal.Sql.Scripting.App
{
	public sealed class Program
	{
		private static int Main(string[] args)
		{
			var options = ParseCommandLineArguments(args);
			if (options == null)
				return ExitCodes.InvalidArguments;

			using (var logger = new Logger())
			using(var logSink = new RollingFileLogSink(new RollingFileSettings(options.LogFolder, "SQL Scripter")))
			{
				logger.AddLogSink(logSink);

				try
				{
					LogHeader(logger, options);

					SetupFolders(options.LogFolder, options.OutputFolder);

					var scriptFileManager = new ScriptFileManager(Path.Combine(options.OutputFolder, options.Server));
					var server = new ServerWrapper(new Server(options.Server));

					var scripter =
						new Scripter(server, scriptFileManager, logger)
							.IncludeTheseDatabases(options.IncludeDatabases.ToArray())
							.ExcludedTheseDatabases(options.ExcludeDatabases.ToArray());

					ConfigureScriptingSources(options, scripter);

					scripter.DumpScripts();

					logger.PostEntry("DONE");
				}
				catch (Exception ex)
				{
					logger.PostException(ex);
					Console.WriteLine(ex);
					return ExitCodes.UnexpectedException;
				}

				logger.CompleteAllAsync().Wait(new TimeSpan(0, 0, 3));
			}

			return ExitCodes.Ok;
		}

		private static void SetupFolders(params string[] paths)
		{
			foreach (var path in paths)
			{
				var directory = new DirectoryInfo(path);
				if (directory.Exists)
					continue;

				directory.Create();
			}
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

		private static void LogHeader(ILoggerSync logger, AppOptions options)
		{
			logger.PostEntry("SQL Scripting Application");
			logger.PostEntry(typeof (Program).Assembly.GetName().Version.ToString());

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

			lines.ForEach(l => logger.PostEntryNoTimestamp(l));
		}
	}

	internal static class ExitCodes
	{
		internal const int
			Ok = -1,
			UnexpectedException = 1,
			InvalidArguments = 2
		;
	}
}