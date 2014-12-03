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
	public interface IRunnerSettings
	{
		FileLoggerSettings FileLoggerSettings { get; }
		string ScriptProjectFolder { get; }
		string Server { get; }
		bool NoTransaction { get; }
		bool UseTransaction { get; }
		bool ShouldRollback { get; }
		bool CheckFilesOnly { get; }
		bool BypassCheck { get; }
	}

	public sealed class RunnerSettings : IRunnerSettings
	{
		public RunnerSettings(string scriptProjectFolder, string logFolder, string server, 
			bool rollback = false, bool noTransaction = false, bool checkFilesOnly = false, bool bypassCheck = false)
		{
			if(checkFilesOnly && bypassCheck)
				throw new ArgumentException("bypassCheck and checkFilesOnly cannot both be true.", "bypassCheck");

			if(checkFilesOnly && (noTransaction == true || rollback == false))
				throw new ArgumentException("When 'checkFilesOnly' is True, then 'noTransaction' must be False and 'rollback' must be True.", "checkFilesOnly");

			_scriptProjectFolder = scriptProjectFolder;
			_server = server;
			_noTransaction = noTransaction;
			_rollback = rollback;
			_checkFilesOnly = checkFilesOnly;
			_bypassCheck = bypassCheck;

			_fileLoggerSettings = new FileLoggerSettings(logFolder, "SqlScriptDeployer");
		}

		public FileLoggerSettings FileLoggerSettings { get { return _fileLoggerSettings; } }

		public string ScriptProjectFolder { get { return _scriptProjectFolder; } }

		public string Server { get { return _server; } }

		public bool NoTransaction { get { return _noTransaction; } }

		public bool UseTransaction { get { return _noTransaction == false; } }

		public bool ShouldRollback { get { return _rollback; } }

		public bool CheckFilesOnly { get { return _checkFilesOnly; } }

		public bool BypassCheck { get { return _bypassCheck; } }

		private readonly bool _noTransaction, _checkFilesOnly, _rollback, _bypassCheck;
		private readonly FileLoggerSettings _fileLoggerSettings;
		private readonly string _scriptProjectFolder, _server;
	}
}