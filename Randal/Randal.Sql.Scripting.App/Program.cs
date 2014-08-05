using System;
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

			using (var logger = new AsyncFileLogger(new FileLoggerSettings(options.LogFolder, "scripter")))
			{
				LogHeader(logger, options);

				var scriptFileManager = new ScriptFileManager(Path.Combine(options.OutputFolder, options.Server));
				var server = new ServerWrapper(options.Server);

				var scripter = 
					new Scripter(server, scriptFileManager, logger)
						.IncludeTheseDatabases(options.IncludeDatabases.ToArray())
						.ExcludedTheseDatabases(options.ExcludeDatabases.ToArray())
						.SetupSources( 
							new ScriptingSource("Sprocs", (srvr, db) => srvr.GetStoredProcedures(db)),
							new ScriptingSource("Functions", (srvr, db) => srvr.GetUserDefinedFunctions(db)),
							new ScriptingSource("Views", (srvr, db) => srvr.GetViews(db)),
							new ScriptingSource("Tables", (srvr, db) => srvr.GetTables(db))
						);

				scripter.DumpScripts();

				logger.Add("DONE".ToLogEntry());
			}

			return -1;
		}

		private static AppOptions ParseCommandLineArguments(string[] args)
		{
			var parser = new AppOptionsParser();
			var results = parser.Parse(args);

			if (!results.HasErrors) 
				return parser.Object;

			Console.WriteLine(results.ErrorText);
			return null;
		}

		private static void LogHeader(ILogger logger, AppOptions options)
		{
			logger.Add("SQL Scripting Application".ToLogEntry());

			logger.Add(string.Concat("Server           ", options.Server).ToLogEntryNoTs());
			logger.Add(string.Concat("Output Folder    ", options.OutputFolder).ToLogEntryNoTs());
			logger.Add(string.Concat("Log Folder       ", options.LogFolder).ToLogEntryNoTs());
			logger.Add(string.Concat("Included DBs     ", string.Join(", ", options.IncludeDatabases)).ToLogEntryNoTs());
			logger.Add(string.Concat("Excluded DBs     ", string.Join(", ", options.ExcludeDatabases)).ToLogEntryNoTs());
		}
	}
}