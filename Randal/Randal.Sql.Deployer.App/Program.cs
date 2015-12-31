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
using System.ServiceModel;
using System.Threading.Tasks;
using Randal.Logging;
using Randal.Sql.Deployer.Shared;

namespace Randal.Sql.Deployer.App
{
	internal sealed class SqlDeployerProgram
	{
		private static int Main(string[] args)
		{
			try
			{
				var start = DateTime.UtcNow;
				var options = ParseCommandLineArguments(args);
				if (options == null)
					return 2;

				return RunUnderLogging(options, start);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return (int) RunnerResolution.ExceptionThrown;
			}
		}

		private static int RunUnderLogging(AppOptions options, DateTime start)
		{
			IRunnerSettings settings = (RunnerSettings)options;

			using (var logger = new Logger())
			using (var rollingFileLogSink = new RollingFileLogSink(settings.FileLoggerSettings))
			{
				try
				{
					logger.AddLogSink(rollingFileLogSink);

					SendLogFilePathToExchange(logger, rollingFileLogSink, options);

					var runner = new Runner(settings, logger);

					return (int)runner.Go();
				}
				catch (Exception ex)
				{
					logger.PostException(ex);
					throw ex;
				}
				finally
				{
					logger.PostEntryNoTimestamp("Elapsed time {0}", DateTime.UtcNow - start);
					logger.CompleteAllAsync().Wait(new TimeSpan(0, 0, 3));
				}
			}
		}

		private static void SendLogFilePathToExchange(ILoggerSync logger, IRollingFileLogSink fileLogSink, AppOptions options)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(options.ExchangePath))
				{
					logger.PostEntry("Exchange -e option not set, skipping.");
					return;
				}

				logger.PostEntry("Attempting connection to UI Host '" + SharedConst.LogExchangeNetPipe + "'.");

				var endPoint = new EndpointAddress(SharedConst.LogExchangeNetPipe);
				var pipeFactory = new ChannelFactory<ILogExchange>(new NetNamedPipeBinding(), endPoint);
				var logExchange = pipeFactory.CreateChannel();

				logExchange.ReportLogFilePath(fileLogSink.CurrentLogFilePath ?? "Path not found.");
			}
			catch(EndpointNotFoundException ex)
			{
				logger.PostException(ex);
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
	}
}