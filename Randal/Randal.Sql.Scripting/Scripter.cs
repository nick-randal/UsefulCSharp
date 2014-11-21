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
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;
using Randal.Logging;
using ScriptableObject = System.Tuple<Randal.Sql.Scripting.ScriptingSource, Microsoft.SqlServer.Management.Smo.ScriptSchemaObjectBase>;

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

		public IReadOnlyList<string> IncludedDatabases { get { return _includeTheseDatabases.AsReadOnly(); } }

		public IReadOnlyList<string> ExcludedDatabases { get { return _excludeTheseDatabases.AsReadOnly(); } }

		public IReadOnlyList<ScriptingSource> Sources { get { return _sources; } } 

		public Scripter AddSources(params ScriptingSource[] sources)
		{
			_sources.AddRange(sources);
			return this;
		}

		public Scripter ClearSources()
		{
			_sources.Clear();
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
			var processed = 0;
			var timeTracker = new Stopwatch();

			if (_sources.Count == 0)
				throw new InvalidOperationException("Sources need to be setup prior to dumping scripts.");

			foreach (var database in GetDatabases())
			{
				timeTracker.Start();
				_logger.AddEntryNoTimestamp("~~~~~~~~~~ {0,-20} ~~~~~~~~~~", database.Name);
				try
				{
					processed = DumpAllScripts(database);
				}
				catch (Exception ex)
				{
					_logger.AddException(ex);
				}
				finally
				{
					timeTracker.Stop();
					_logger.AddEntry("Elapsed time: {0}", timeTracker.Elapsed);
					_logger.AddEntryNoTimestamp("~~~~~~~~~~ processed {0} SQL objects ~~~~~~~~~~", processed);
					timeTracker.Reset();
				}
			}
		}

		private int DumpAllScripts(Database database)
		{
			_scriptFileManager.SetupDatabaseDirectory(database.Name);
			var processed = 0;
			var scriptableObjects = new List<ScriptableObject>();

			foreach (var source in _sources)
			{
				_logger.AddEntry("Setup script directory '{0}'.", source.SubFolder);
				_scriptFileManager.SetupScriptDirectory(database.Name, source.SubFolder);
			}

			_logger.AddEntry("Loading scriptable objects.");
			foreach (var source in _sources)
			{	
				scriptableObjects.AddRange(
					source
						.GetScriptableObjects(_server, database)
						.Select(so => new ScriptableObject(source, so))
				);
			}

			_logger.AddEntry("Found {0} schema objects.", scriptableObjects.Count);
			CheckForNameCollisions(scriptableObjects);

			foreach (var sqlObject in scriptableObjects)
			{
				_logger.AddEntry("{0} {1}.{2}", MapTypeName(sqlObject.Item2.GetType().Name), sqlObject.Item2.Schema, sqlObject.Item2.Name);

				if (sqlObject.Item2.Schema != "dbo")
				{
					_logger.AddEntry("WARNING: schema not dbo, but '{0}'", sqlObject.Item2.Schema);
				}

				_scriptFileManager.WriteScriptFile(database.Name, sqlObject.Item1.SubFolder, sqlObject.Item2.ScriptFileName(), _formatter.Format(sqlObject.Item2));
				processed++;
			}

			return processed;
		}

		private void CheckForNameCollisions(IEnumerable<ScriptableObject> scriptableObjects)
		{
			var duplicates = scriptableObjects.GroupBy(so => so.Item2.ScriptFileName())
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToList().AsReadOnly();

			if (!duplicates.Any())
				return;

			var csvDuplicates = string.Join(", ", duplicates);

			_logger.AddEntry("Duplicate scripts for: {0}", csvDuplicates);

			throw new DuplicateNameException(csvDuplicates);
		}

		private IEnumerable<Database> GetDatabases()
		{
			_logger.AddEntry("Getting databases based on options.");
			var databases = _server.GetDatabases().AsQueryable();

			if (_includeTheseDatabases.Count > 0)
				databases = databases.Where(d => _includeTheseDatabases.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase));

			if (_excludeTheseDatabases.Count > 0)
				databases =
					databases.Where(d => _excludeTheseDatabases.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase) == false);

			var databaseList = databases.ToList();

			_logger.AddEntry("Databases: {0}",
				string.Join(", ", databaseList.Select(db => db.Name))
			);

			return databaseList;
		}

		private static string MapTypeName(string objectType)
		{
			string replaceWith;

			return TypeNameLookup.TryGetValue(objectType, out replaceWith) 
				? replaceWith 
				: objectType.ToLower();
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