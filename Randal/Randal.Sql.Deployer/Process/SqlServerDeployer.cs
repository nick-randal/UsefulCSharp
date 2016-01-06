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
using System.Linq;
using Randal.Core.Enums;
using Randal.Logging;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Helpers;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Sql.Deployer.Process
{
	public sealed class SqlServerDeployer : ScriptDeployerBase
	{
		public SqlServerDeployer(IScriptDeployerConfig config, IProject project, ISqlConnectionManager connectionManager, ILoggerSync logger)
			: base(config, project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (connectionManager == null)
				throw new ArgumentNullException("connectionManager");

			_connectionManager = connectionManager;
			_logger = logger ?? new NullLogger();
			_patternLookup = new CatalogPatternLookup();
		}

		public override bool CanProceed()
		{
			CreateProjectsTable();

			var canProceed = IsProjectVersionValid();

			if (canProceed)
				AddProject();

			return canProceed;
		}

		public override Returned DeployScripts()
		{
			var phases = new[] {SqlScriptPhase.Pre, SqlScriptPhase.Main, SqlScriptPhase.Post};

			if (DeployPriorityScripts(phases) == Returned.Failure)
				return Returned.Failure;

			return phases.Any(phase => DeployPhase(phase) == Returned.Failure) 
				? Returned.Failure : Returned.Success;
		}

		private Returned DeployPriorityScripts(SqlScriptPhase[] phases)
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
					try
					{
						ExecSql(script, phase);
					}
					catch (Exception ex)
					{
						_logger.PostException(ex);
						return Returned.Failure;
					}
				}
			}

			return Returned.Success;
		}

		private Returned DeployPhase(SqlScriptPhase sqlScriptPhase)
		{
			_logger.PostEntryNoTimestamp("{0}    {1} ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~{0}", Environment.NewLine,
				sqlScriptPhase);

			foreach (var script in Project.NonPriorityScripts.Where(s => s.HasSqlScriptPhase(sqlScriptPhase)))
			{
				_logger.PostEntry(Verbosity.Important, "{0}  {1}", script.Name, sqlScriptPhase);

				try
				{
					ExecSql(script, sqlScriptPhase);
				}
				catch (Exception ex)
				{
					_logger.PostException(ex);
					return Returned.Failure;
				}
			}

			return Returned.Success;
		}

		private void ExecSql(SourceScript script, SqlScriptPhase phase)
		{
			if (script.HasPhaseExecuted(phase))
				return;

			var sql = script.RequestSqlScriptPhase(phase);
			if (sql == null)
				return;

			sql = PhaseDeploymentComment + sql;

			var configuration = script.GetConfiguration();

			using (var command = _connectionManager.CreateCommand(sql))
			{
				foreach (var catalog in GetCatalogs(script))
				{
					_logger.PostEntryNoTimestamp("    {0}", catalog);
					if (configuration == null)
						command.Execute(catalog);
					else
						command.Execute(catalog, configuration.Settings.Timeout);
				}
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

			using (var command = _connectionManager.CreateCommand(DeployerConfig.ProjectsTableConfig.CreateTable))
			{
				command.Execute(DeployerConfig.ProjectsTableConfig.Database);
			}
		}

		private void AddProject()
		{
			_logger.PostEntry("adding project record.");

			var values = new object[]
				{ Project.Configuration.Project, Project.Configuration.Version, Environment.MachineName, Environment.UserName };

			using (var command = _connectionManager.CreateCommand(DeployerConfig.ProjectsTableConfig.Insert, values))
			{
				command.Execute(DeployerConfig.ProjectsTableConfig.Database);
			}
		}

		private bool IsProjectVersionValid()
		{
			Version databaseVersion;
			var projectConfig = Project.Configuration;

			var projectVersion = new Version(projectConfig.Version);

			_logger.PostEntry("Looking up project '{0}'", projectConfig.Project);

			using (
				var command = _connectionManager.CreateCommand(DeployerConfig.ProjectsTableConfig.Read, projectConfig.Project, projectConfig.Version))
			using (var reader = command.ExecuteReader(DeployerConfig.ProjectsTableConfig.Database))
			{
				if (reader.HasRows == false || reader.Read() == false || reader.IsDBNull(0))
				{
					_logger.PostEntry("never encountered this project before, continuing.");
					return true;
				}

				databaseVersion = new Version(reader.GetString(0));
			}

			_logger.PostEntry("database version is '{0}' and config version is '{1}'", databaseVersion, projectVersion);

			return databaseVersion < projectVersion;
		}

		private readonly ILoggerSync _logger;
		private readonly ISqlConnectionManager _connectionManager;
		private readonly CatalogPatternLookup _patternLookup;
	}
}