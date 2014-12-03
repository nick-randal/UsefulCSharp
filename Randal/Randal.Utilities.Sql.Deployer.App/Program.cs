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
using Randal.Logging;

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
					new Runner(settings, logger).Go();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return 1;
			}

			return -1;
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