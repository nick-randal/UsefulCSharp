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
using Fclp;

namespace Randal.Sql.Deployer.App
{
	public sealed class AppOptions
	{
		public string ProjectFolder { get; set; }

		public string LogFolder { get; set; }

		public string Server { get; set; }

		public bool NoTransaction { get; set; }

		public bool Rollback { get; set; }

		public bool CheckFilesOnly { get; set; }

		public bool BypassCheck { get; set; }

		public string ExchangePath { get; set; }

		public static explicit operator RunnerSettings(AppOptions options)
		{
			return new RunnerSettings(
					options.ProjectFolder,
					options.LogFolder,
					options.Server,
					options.Rollback,
					options.NoTransaction,
					options.CheckFilesOnly,
					options.BypassCheck
				);
		}
	}

	public sealed class AppOptionsParser : FluentCommandLineParser<AppOptions>
	{
		public AppOptionsParser()
		{
			SetupHelp("?", "h", "help").Callback(x => Console.WriteLine(x));

			Setup(x => x.ProjectFolder)
				.As('p', "projectFolder")
				.WithDescription(ProjectFolderHelpText)
				.Required();

			Setup(x => x.LogFolder)
				.As('l', "logFolder")
				.WithDescription(LogFolderHelpText)
				.Required();

			Setup(x => x.Server)
				.As('s', "server")
				.WithDescription(ServerHelpText)
				.Required();

			Setup(x => x.NoTransaction)
				.As('n', "noTrans")
				.WithDescription(NoTransactionHelpText)
				.SetDefault(false);

			Setup(x => x.Rollback)
				.As('r', "rollback")
				.WithDescription(RollbackHelptText)
				.SetDefault(false);

			Setup(x => x.CheckFilesOnly)
				.As('c', "checkOnly")
				.WithDescription(CheckFilesOnlyText)
				.SetDefault(false);

			Setup(x => x.BypassCheck)
				.As('b', "bypassCheck")
				.WithDescription(CheckFilesOnlyText)
				.SetDefault(false);

			Setup(x => x.ExchangePath)
				.As('e', "exchangePath")
				.WithDescription(ExchangePathText)
				.SetDefault(null);
		}

		public const string
			ProjectFolderHelpText = @"The project folder containg the config.json or config.xml and all associated SQL scripts.",
			ServerHelpText = @"The SQL Server that the project will be deployed against.",
			LogFolderHelpText = @"Directory where the log file will be written.",
			NoTransactionHelpText = "Do not use a transaction to execute scripts.",
			RollbackHelptText = "Rollback the transaction, even if there were no errors.",
			CheckFilesOnlyText = "Checks the scripts against provided patterns and no scripts will be deployed.  Cannot be used with noTrans.",
			BypassCheckText = "Bypasses the pattern checker for scripts.",
			ExchangePathText = "The data exchange channel path."
		;

		
	}
}