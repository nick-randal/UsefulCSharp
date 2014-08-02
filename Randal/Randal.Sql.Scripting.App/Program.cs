using System.IO;
using CommandLine;
using Randal.Logging;

namespace Randal.Sql.Scripting.App
{
	public sealed class Program
	{
		public const string Local = "localhost", Staging = "TCSQLStaging01";

		private static int Main(string[] args)
		{
			//var options = new AppOptions();
			//if (Parser.Default.ParseArguments(args, options) == false)
//				return 2;

			var scriptFileManager = new ScriptFileManager(Path.Combine(@"C:\__dev\Database\Dump", Local));
			var server = new ServerWrapper(Local);

			var settings = new FileLoggerSettings(@"c:\__dev\Research\Log", "scripter");
			using (var logger = new AsyncFileLogger(settings))
			{
				logger.Add("SQL Scripting Application".ToLogEntry());
				logger.Add(string.Concat("generating scripts for ", Local).ToLogEntry());

				var scripter = new Scripter(server, scriptFileManager, logger);
				
				scripter.DumpScripts( 
					new ScriptingSource("Sprocs", (srvr, db) => srvr.GetStoredProcedures(db)),
					new ScriptingSource("Functions", (srvr, db) => srvr.GetUserDefinedFunctions(db)),
					new ScriptingSource("Views", (srvr, db) => srvr.GetViews(db))
				);
			}

			return -1;
		}
	}
}