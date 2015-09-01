// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
				var options = ParseCommandLineArguments(args);
				if (options == null)
					return 2;

				IRunnerSettings settings = new RunnerSettings(
					options.ProjectFolder, 
					options.LogFolder, 
					options.Server, 
					options.Rollback,
					options.NoTransaction,
					options.CheckFilesOnly,
					options.BypassCheck
				);


				using (var logger = new AsyncFileLogger(settings.FileLoggerSettings))
				{
					SendLogFilePathToExchange(logger);
					var runner = new Runner(settings, logger);
					return (int)runner.Go();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return (int)RunnerResolution.ExceptionThrown;
			}
		}

		private static void SendLogFilePathToExchange(AsyncFileLogger logger)
		{
			try
			{
				logger.Add(("Attempting connect to UI Host '" + SharedConst.LogExchangeNetPipe + "'.").ToLogEntry());

				var endPoint = new EndpointAddress(SharedConst.LogExchangeNetPipe);
				var pipeFactory = new ChannelFactory<ILogExchange>(new NetNamedPipeBinding(), endPoint);
				var logExchange = pipeFactory.CreateChannel();

				logExchange.ReportLogFilePath(logger.CurrentLogFilePath ?? "Path not found.");

			}
			catch(EndpointNotFoundException ex)
			{
				logger.Add(ex.ToLogEntryException());
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