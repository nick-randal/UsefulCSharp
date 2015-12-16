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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
			_formatter = formatter ?? new ScriptFormatter();
			_logger = logger;
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
				_logger.PostEntryNoTimestamp("~~~~~~~~~~ {0,-20} ~~~~~~~~~~", database.Name);
				try
				{
					processed = DumpAllScripts(database);
				}
				catch (Exception ex)
				{
					_logger.PostException(ex);
				}
				finally
				{
					timeTracker.Stop();
					_logger.PostEntry("Elapsed time: {0}", timeTracker.Elapsed);
					_logger.PostEntryNoTimestamp("~~~~~~~~~~ processed {0} SQL objects ~~~~~~~~~~", processed);
					timeTracker.Reset();
				}
			}
		}

		private int DumpAllScripts(Database database)
		{
			_scriptFileManager.SetupDatabaseDirectory(database.Name);
			var processed = 0;
			
			SetupScriptFolders(database);

			var scriptableObjects = LoadScriptObjects(database);
			CheckForNameCollisions(scriptableObjects);

			foreach (var scriptableObject in scriptableObjects)
			{
				var so = scriptableObject.SchemaObject;

				_logger.PostEntry("{0} {1}.{2}", MapTypeName(so.GetType().Name), so.Schema, so.Name);

				if (scriptableObject.IsEncrypted)
				{
					_logger.PostEntry("WARNING: schema object '{0}.{1}' is encrypted. SKIPPING.", so.Schema, so.Name);
					continue;
				}

				_scriptFileManager.WriteScriptFile(
					database.Name,
					scriptableObject.ScriptingSource.SubFolder,
					so.ScriptFileName(),
					_formatter.Format(scriptableObject)
				);
				
				processed++;
			}

			return processed;
		}

		private List<ScriptableObject> LoadScriptObjects(Database database)
		{
			var scriptableObjects = new List<ScriptableObject>();

			_logger.PostEntry("Loading scriptable objects.");

			var cancellation = new CancellationTokenSource();

			var tasks = _sources.Select(source => 
				Task.Factory.StartNew(state =>
					{
						var temp = new List<ScriptableObject>();
						var src = (ScriptingSource) state;

						var server = new Server(_server.Name);
						var db = new Database(server, database.Name);
						db.Refresh();

						src
							.GetScriptableObjects(new ServerWrapper(server), db)
							.Select(so => new ScriptableObject(source, so)).ToList().ForEach(temp.Add);

						_logger.PostEntry("Found {0} {1}.", temp.Count, src.SubFolder);

						return temp;
					}, 
					source, 
					cancellation.Token, 
					TaskCreationOptions.None, 
					TaskScheduler.Default
				)
			)
			.ToList();

			Task.WhenAll(tasks).Wait(cancellation.Token);

			tasks.ForEach(t => scriptableObjects.AddRange(t.Result));

			_logger.PostEntry("Found {0} total schema objects.", scriptableObjects.Count);

			return scriptableObjects;
		}

		private void SetupScriptFolders(IDatabaseOptions database)
		{
			foreach (var source in _sources)
			{
				_logger.PostEntry("Setup script directory '{0}'.", source.SubFolder);
				_scriptFileManager.SetupScriptDirectory(database.Name, source.SubFolder);
			}
		}

		private void CheckForNameCollisions(IEnumerable<ScriptableObject> scriptableObjects)
		{
			var duplicates = scriptableObjects.GroupBy(so => so.SchemaObject.ScriptFileName())
				.Where(group => group.Count() > 1)
				.Select(group => group.Key)
				.ToList().AsReadOnly();

			if (!duplicates.Any())
				return;

			var csvDuplicates = string.Join(", ", duplicates);

			_logger.PostEntry("Duplicate scripts for: {0}", csvDuplicates);

			throw new DuplicateNameException(csvDuplicates);
		}

		private IEnumerable<Database> GetDatabases()
		{
			_logger.PostEntry("Getting databases based on options.");
			var databases = _server.GetDatabases().AsQueryable();

			if (_includeTheseDatabases.Count > 0)
				databases = databases.Where(d => _includeTheseDatabases.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase));

			if (_excludeTheseDatabases.Count > 0)
				databases =
					databases.Where(d => _excludeTheseDatabases.Contains(d.Name, StringComparer.InvariantCultureIgnoreCase) == false);

			var databaseList = databases.ToList();

			_logger.PostEntry("Databases: {0}",
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
		private readonly ILoggerSync _logger;
		private readonly List<ScriptingSource> _sources;
		private readonly List<string> _includeTheseDatabases, _excludeTheseDatabases; 
	}
}