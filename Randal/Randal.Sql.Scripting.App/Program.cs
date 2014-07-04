using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Randal.Logging;

namespace Randal.Sql.Scripting.App
{
	public sealed class Program
	{
		public const string Local = ".", Staging = "TCSQLStaging01";

		private static void Main(string[] args)
		{
			var scriptFileManager = new ScriptFileManager(@"C:\__dev\Database\Dump");
			var server = new ServerWrapper(Local);

			var settings = new FileLoggerSettings(@"c:\__dev\Research\Log", "scripter");
			using (var logger = new AsyncFileLogger(settings))
			{
				new Scripter(server, scriptFileManager, logger).DumpScripts();
			}
		}
	}
}