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

using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Randal.Logging;

namespace Randal.Sql.Scripting.App
{
	public sealed class Scripter
	{
		private readonly IServer _server;
		private readonly IScriptFormatter _formatter;

		public Scripter(IServer server, IScriptFileManager scriptFileManager, ILogger logger, IScriptFormatter formatter = null)
		{
			_server = server;
			_scriptFileManager = scriptFileManager;
			_formatter = formatter ?? new ScriptFormatter(_server);

			if(logger is ILoggerStringFormatDecorator == false)
				_logger = new LoggerStringFormatDecorator(logger);
			else
				_logger = (ILoggerStringFormatDecorator)logger;
		}

		public void DumpScripts(string sprocsFolder = "Sprocs", string udfFolder = "Functions", string viewFolder = "Views")
		{
			foreach (var database in _server.GetDatabases())
			{
				_logger.AddEntryNoTimestamp("~~~~~~~~~~ {0,20} ~~~~~~~~~~", database.Name);
				try
				{
					ProcessStoredProcedures(database, sprocsFolder);
					ProcessUserDefinedFunctions(database, udfFolder);
					ProcessViews(database, viewFolder);
				}
				catch (ExecutionFailureException ex)
				{
					_logger.AddException(ex);
				}
			}
		}

		private void ProcessStoredProcedures(Database database, string subFolder)
		{
			_scriptFileManager.CreateDirectory(database.Name, subFolder);
			
			foreach (var sproc in _server.GetStoredProcedures(database))
			{
				_logger.AddEntry("{0}.{1}", sproc.Schema, sproc.Name);
				if (sproc.Schema != "dbo")
					_logger.AddEntry("{0} {1}", sproc.Schema, sproc.Name);

				_scriptFileManager.WriteScriptFile(sproc.Name, _formatter.Format(sproc));
			}
		}

		private void ProcessUserDefinedFunctions(Database database, string subFolder)
		{
			_scriptFileManager.CreateDirectory(database.Name, subFolder);
		}

		private void ProcessViews(Database database, string subFolder)
		{
			_scriptFileManager.CreateDirectory(database.Name, subFolder);
		}

		private readonly IScriptFileManager _scriptFileManager;
		private readonly ILoggerStringFormatDecorator _logger;
	}
}