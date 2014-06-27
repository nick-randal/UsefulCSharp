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

using Randal.Logging;

namespace Randal.Sql.Deployer.App
{
	public sealed class RunnerSettings
	{
		public RunnerSettings(string scriptProjectFolder, string logFolder, string server, bool rollback = false,
			bool noTransaction = false)
		{
			_scriptProjectFolder = scriptProjectFolder;
			_server = server;
			_noTransaction = noTransaction;
			_rollback = rollback;
			_fileLoggerSettings = new FileLoggerSettings(logFolder, "SqlScriptDeployer");
		}

		public FileLoggerSettings FileLoggerSettings
		{
			get { return _fileLoggerSettings; }
		}

		public string ScriptProjectFolder
		{
			get { return _scriptProjectFolder; }
		}

		public string Server
		{
			get { return _server; }
		}

		public bool NoTransaction
		{
			get { return _noTransaction; }
		}

		public bool UseTransaction
		{
			get { return _noTransaction == false; }
		}

		public bool ShouldRollback
		{
			get { return _rollback; }
		}

		private readonly bool _noTransaction, _rollback;
		private readonly FileLoggerSettings _fileLoggerSettings;
		private readonly string _scriptProjectFolder, _server;
	}
}