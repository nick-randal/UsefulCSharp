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
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Randal.Logging;

namespace Randal.Sql.Scripting
{
	public sealed class Scripter
	{
		public Scripter(IServer server, IScriptFileManager scriptFileManager, ILogger logger, IScriptFormatter formatter = null)
		{
			_server = server;
			_scriptFileManager = scriptFileManager;
			_includeTheseDatabases = new List<string>();
			_excludeTheseDatabases = new List<string>();
			_sources = new List<ScriptingSource>();
			_formatter = formatter ?? new ScriptFormatter(_server);
			_logger = new LoggerStringFormatWrapper(logger);
		}

		public Scripter SetupSources(params ScriptingSource[] sources)
		{
			_sources.Clear();
			_sources.AddRange(sources);
			return this;
		}

		public Scripter IncludeTheseDatabases(params string[] databases)
		{
			_includeTheseDatabases.Clear();

			if (databases.Length == 0)
				return this;

			_includeTheseDatabases.AddRange(databases);
			return this;
		}

		public Scripter ExcludedTheseDatabases(params string[] databases)
		{
			_excludeTheseDatabases.Clear();

			if (databases.Length == 0)
				return this;

			_excludeTheseDatabases.AddRange(databases);
			return this;
		}

		public void DumpScripts()
		{
			foreach (var database in GetDatabases())
			{
				_logger.AddEntryNoTimestamp("~~~~~~~~~~ {0,-20} ~~~~~~~~~~", database.Name);
				try
				{
					foreach(var source in _sources)
						ProcessObject(database, source.SubFolder, source.GetScriptableObjects(_server, database));
				}
				catch (ExecutionFailureException ex)
				{
					_logger.AddException(ex);
				}
			}
		}

		private IEnumerable<Database> GetDatabases()
		{
			var databases = _server.GetDatabases().AsQueryable();

			if (_includeTheseDatabases.Count > 0)
				databases = databases.Where(d => _includeTheseDatabases.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase));

			if (_excludeTheseDatabases.Count > 0)
				databases =
					databases.Where(d => _excludeTheseDatabases.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase) == false);
			
			return databases.ToList();
		}

		private void ProcessObject(IDatabaseOptions database, string subFolder, IEnumerable<ScriptSchemaObjectBase> source) 
		{
			_scriptFileManager.CreateDirectory(database.Name, subFolder);

			foreach (var sproc in source)
			{
				_logger.AddEntry("{0} {1}.{2}", MapTypeName(sproc.GetType().Name), sproc.Schema, sproc.Name);
				if (sproc.Schema != "dbo")
					_logger.AddEntry("schema not dbo {0}", sproc.Schema);

				_scriptFileManager.WriteScriptFile(sproc.Name, _formatter.Format(sproc));
			}
		}

		private static string MapTypeName(string objectType)
		{
			string replaceWith;

			if (TypeNameLookup.TryGetValue(objectType, out replaceWith))
				return replaceWith;

			return objectType.ToLower();
		}

		private static readonly IDictionary<string, string> TypeNameLookup =
			new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
			{
				{"StoredProcedure", "sproc"}, {"UserDefinedFunction", "udf"}, {"View", "view"}
			};
		private readonly IServer _server;
		private readonly IScriptFormatter _formatter;
		private readonly IScriptFileManager _scriptFileManager;
		private readonly ILoggerStringFormatWrapper _logger;
		private readonly List<ScriptingSource> _sources;
		private readonly List<string> _includeTheseDatabases, _excludeTheseDatabases; 
	}
}