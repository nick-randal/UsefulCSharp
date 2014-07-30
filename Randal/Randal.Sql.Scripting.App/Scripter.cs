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

using System.Collections.Generic;
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

			_logger = new LoggerStringFormatWrapper(logger);
		}

		public void DumpScripts(string sprocsFolder = "Sprocs", string udfFolder = "Functions", string viewFolder = "Views")
		{
			foreach (var database in _server.GetDatabases())
			{
				_logger.AddEntryNoTimestamp("~~~~~~~~~~ {0,20} ~~~~~~~~~~", database.Name);
				try
				{
					ProcessObject(database, sprocsFolder, _server.GetStoredProcedures(database));

					ProcessObject(database, udfFolder, _server.GetUserDefinedFunctions(database));

					ProcessObject(database, viewFolder, _server.GetViews(database));
				}
				catch (ExecutionFailureException ex)
				{
					_logger.AddException(ex);
				}
			}
		}

		private void ProcessObject(IDatabaseOptions database, string subFolder, IEnumerable<ScriptSchemaObjectBase> source) 
		{
			_scriptFileManager.CreateDirectory(database.Name, subFolder);

			foreach (var sproc in source)
			{
				_logger.AddEntry("{0} {1}.{2}", sproc.GetType().Name, sproc.Schema, sproc.Name);
				if (sproc.Schema != "dbo")
					_logger.AddEntry("schema not dbo {0}", sproc.Schema);

				_scriptFileManager.WriteScriptFile(sproc.Name, _formatter.Format(sproc));
			}
		}

		private readonly IScriptFileManager _scriptFileManager;
		private readonly ILoggerStringFormatWrapper _logger;
	}
}