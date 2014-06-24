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

using CommandLine;
using CommandLine.Text;

namespace Randal.Utilities.Sql.Deployer.App
{
	public sealed class AppOptions
	{
		[Option('p', "projectFolder", Required = true,
			HelpText = "The project folder containg the config.json an all associated SQL files.")]
		public string ProjectFolder { get; set; }

		[Option('l', "logFolder", Required = true, HelpText = "Directory where the log file will be written.")]
		public string LogFolder { get; set; }

		[Option('s', "server", Required = true, HelpText = "The SQL Server that the project will be deployed against.")]
		public string Server { get; set; }

		[Option('n', "noTrans", Required = false, HelpText = "Do not use a transaction to execute scripts.")]
		public bool NoTransaction { get; set; }

		[Option('r', "rollback", Required = false, HelpText = "Rollback the transaction, even if there were no errors.")]
		public bool Rollback { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }

		[HelpOption]
		public string GetUsage()
		{
			return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}