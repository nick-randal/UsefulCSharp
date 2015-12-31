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
using System.IO;
using System.Linq;
using Randal.Core.Enums;
using Randal.Logging;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Helpers;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Sql.Deployer.Process
{
	public sealed class ScriptFileDeployer : ScriptDeployerBase
	{
		public ScriptFileDeployer(IScriptDeployerConfig config, IProject project, ISqlConnectionManager connectionManager, ILogger logger)
			: base(config, project)
		{
			if (project == null)
				throw new ArgumentNullException("project");

			_connectionManager = connectionManager;
			_logger = logger ?? new NullLogger();
			_patternLookup = new CatalogPatternLookup();

			var fileName = string.Format("{0}_{1}_v{2}.sql", connectionManager.Server, project.Configuration.Project, project.Configuration.Version.Replace('.', '-'));
			var path = Path.Combine("G:\\", fileName);
			_writer = new StreamWriter(File.Create(path, 16384, FileOptions.Asynchronous));

			_writer.WriteLine("-- Generated {0} on {1} by {2}", DateTime.Now, Environment.MachineName, Environment.UserName);
		}

		public override bool CanProceed()
		{
			WriteBeginTransaction();
			
			CreateProjectsTable();
			IsProjectValidUpgrade();
			AddProject();

			WriteEndTransaction();

			return true;
		}

		private void WriteBeginTransaction()
		{
			_writer.WriteLine("set transaction isolation level serializable");
			_writer.WriteLine("begin transaction");
			_writer.WriteLine();
		}

		private void WriteEndTransaction()
		{
			_writer.WriteLine();
			_writer.WriteLine("commit");
		}

		public override Returned DeployScripts()
		{
			WriteBeginTransaction();

			var phases = new List<SqlScriptPhase> { SqlScriptPhase.Pre, SqlScriptPhase.Main, SqlScriptPhase.Post };

			try
			{
				DeployPriorityScripts(phases.ToArray());
				phases.ForEach(DeployPhase);
				WriteEndTransaction();
				return Returned.Success;
			}
			catch (Exception ex)
			{
				_logger.PostException(ex);

				_writer.Flush();
				_writer.BaseStream.Position = 0;

				_writer.WriteLine("-- !!! Building of script file failed.  See the log file.");
				_writer.WriteLine("-- {0}", ex.Message);
				_writer.WriteLine();
				_writer.WriteLine();
				_writer.WriteLine("raiserror('Do not use this script file.', 18, 1)");
				_writer.WriteLine(); 
				_writer.WriteLine();
				
				return Returned.Failure;
			}
		}

		private void DeployPriorityScripts(SqlScriptPhase[] phases)
		{
			_logger.PostEntryNoTimestamp("{0}    Priority Scripts ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~{0}", Environment.NewLine);

			foreach (var script in Project.PriorityScripts)
			{
				_logger.PostEntry(Verbosity.Important, script.Name);

				foreach (var phase in phases)
				{
					if (script.HasSqlScriptPhase(phase) == false)
						continue;

					_logger.PostEntryNoTimestamp("  {0}", phase);
					WriteScript(script, phase);
					
				}
			}
		}

		private void DeployPhase(SqlScriptPhase sqlScriptPhase)
		{
			_logger.PostEntryNoTimestamp("{0}    {1} ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~{0}", Environment.NewLine,
				sqlScriptPhase);

			_writer.WriteLine("-- deploying {0}", sqlScriptPhase);
			_writer.WriteLine();

			foreach (var script in Project.NonPriorityScripts.Where(s => s.HasSqlScriptPhase(sqlScriptPhase)))
			{
				_logger.PostEntry(Verbosity.Important, "{0}  {1}", script.Name, sqlScriptPhase);
				WriteScript(script, sqlScriptPhase);
			}
		}

		private void WriteScript(SourceScript script, SqlScriptPhase phase)
		{
			if (script.HasPhaseExecuted(phase))
				return;

			var sql = script.RequestSqlScriptPhase(phase);
			if (sql == null)
				return;

			sql = PhaseDeploymentComment + sql;

			foreach (var catalog in GetCatalogs(script))
			{
				_logger.PostEntryNoTimestamp("    {0}", catalog);
				WriteCommand(catalog, sql);
			}
		}

		private IEnumerable<string> GetCatalogs(SourceScript script)
		{
			var matchingDatabases = new List<string>();

			foreach (var rgx in script.GetCatalogPatterns().Select(pattern => _patternLookup[pattern]))
			{
				matchingDatabases.AddRange(
					_connectionManager.DatabaseNames
						.Where(dbName => rgx.IsMatch(dbName))
					);
			}

			if(matchingDatabases.Count == 0)
				throw new InvalidOperationException("No matching catalogs found for script '" + script.Name + "'. Check the 'catalog' directive.");

			return matchingDatabases.Distinct().OrderBy(x => x);
		}

		private void CreateProjectsTable()
		{
			_logger.PostEntry("creating Projects table.");

			WriteCommand(DeployerConfig.ProjectsTableConfig.Database, DeployerConfig.ProjectsTableConfig.CreateTable);
		}

		private void AddProject()
		{
			_logger.PostEntry("adding project record.");

			WriteCommand(
				DeployerConfig.ProjectsTableConfig.Database, 
				DeployerConfig.ProjectsTableConfig.Insert, 
				Project.Configuration.Project, 
				Project.Configuration.Version, 
				Environment.MachineName, 
				Environment.UserName
			);
		}

		private void IsProjectValidUpgrade()
		{
			var readDbVersion = string.Format(DeployerConfig.ProjectsTableConfig.Read, Project.Configuration.Project, Project.Configuration.Version);

			_logger.PostEntry("Looking up project '{0}'", Project.Configuration.Project);

			_writer.WriteLine("Use " + DeployerConfig.ProjectsTableConfig.Database);

			_writer.WriteLine("if( ({0}) >= '{1}') begin", readDbVersion, Project.Configuration.Version);
			_writer.WriteLine("\traiserror('Project is older than current database version.', 17, 1)");
			_writer.WriteLine("\treturn");
			_writer.WriteLine("end{0}{0}", Environment.NewLine);
		}

		private void WriteCommand(string database, string command, params object[] values)
		{
			_writer.WriteLine("Use " + database);
			_writer.WriteLine(values.Length == 0 ? command : string.Format(command, values));
			_writer.WriteLine();
			_writer.WriteLine();
		}

		public override void Dispose()
		{
			_writer.Flush();
			_writer.Close();
			_writer.Dispose();
		}
		
		private readonly ILoggerSync _logger;
		private readonly ISqlConnectionManager _connectionManager;
		private readonly CatalogPatternLookup _patternLookup;
		private readonly StreamWriter _writer;
	}
}