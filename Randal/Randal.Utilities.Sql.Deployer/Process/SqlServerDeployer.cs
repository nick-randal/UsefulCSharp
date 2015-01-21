// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
	public interface IScriptDeployer : IDisposable
	{
		bool CanUpgrade();
		Returned DeployScripts();
		IScriptDeployerConfig Config { get; }
	}

	public sealed class SqlServerDeployer : IScriptDeployer
	{
		public SqlServerDeployer(IScriptDeployerConfig config, IProject project, ISqlConnectionManager connectionManager, ILogger logger)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (connectionManager == null)
				throw new ArgumentNullException("connectionManager");

			_config = config;
			_project = project;
			_connectionManager = connectionManager;
			_logger = new LoggerStringFormatWrapper(logger ?? new NullLogger());
			_patternLookup = new CatalogPatternLookup();
		}

		public IScriptDeployerConfig Config { get { return _config; } }

		public bool CanUpgrade()
		{
			CreateProjectsTable();

			var valid = IsProjectValidUpgrade();

			if (valid)
				AddProject();

			return valid;
		}

		public Returned DeployScripts()
		{
			var phases = new[] {SqlScriptPhase.Pre, SqlScriptPhase.Main, SqlScriptPhase.Post};

			if (DeployPriorityScripts(phases) == Returned.Failure)
				return Returned.Failure;

			return phases.Any(phase => DeployPhase(phase) == Returned.Failure) 
				? Returned.Failure : Returned.Success;
		}

		private Returned DeployPriorityScripts(SqlScriptPhase[] phases)
		{
			_logger.AddEntryNoTimestamp("{0}    Priority Scripts ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~{0}", Environment.NewLine);

			foreach (var script in _project.PriorityScripts)
			{
				_logger.AddEntry(Verbosity.Important, script.Name);

				foreach (var phase in phases)
				{
					if (script.HasSqlScriptPhase(phase) == false)
						continue;

					_logger.AddEntryNoTimestamp("  {0}", phase);
					try
					{
						ExecSql(script, phase);
					}
					catch (Exception ex)
					{
						_logger.AddException(ex);
						return Returned.Failure;
					}
				}
			}

			return Returned.Success;
		}

		private Returned DeployPhase(SqlScriptPhase sqlScriptPhase)
		{
			_logger.AddEntryNoTimestamp("{0}    {1} ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~{0}", Environment.NewLine,
				sqlScriptPhase);

			foreach (var script in _project.NonPriorityScripts.Where(s => s.HasSqlScriptPhase(sqlScriptPhase)))
			{
				_logger.AddEntry(Verbosity.Important, "{0}  {1}", script.Name, sqlScriptPhase);

				try
				{
					ExecSql(script, sqlScriptPhase);
				}
				catch (Exception ex)
				{
					_logger.AddException(ex);
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

			var configuration = script.GetConfiguration();

			using (var command = _connectionManager.CreateCommand(sql))
			{
				foreach (var catalog in GetCatalogs(script))
				{
					_logger.AddEntryNoTimestamp("    {0}", catalog);
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
			_logger.AddEntry("creating Projects table.");

			using (var command = _connectionManager.CreateCommand(_config.ProjectsTableConfig.CreateTable))
			{
				command.Execute(_config.ProjectsTableConfig.Database);
			}
		}

		private void AddProject()
		{
			_logger.AddEntry("adding project record.");

			var values = new object[]
				{ _project.Configuration.Project, _project.Configuration.Version, Environment.MachineName, Environment.UserName };

			using (var command = _connectionManager.CreateCommand(_config.ProjectsTableConfig.Insert, values))
			{
				command.Execute(_config.ProjectsTableConfig.Database);
			}
		}

		private bool IsProjectValidUpgrade()
		{
			Version databaseVersion;
			var projectConfig = _project.Configuration;

			var projectVersion = new Version(projectConfig.Version);

			_logger.AddEntry("Looking up project '{0}'", projectConfig.Project);

			using (
				var command = _connectionManager.CreateCommand(_config.ProjectsTableConfig.Read, projectConfig.Project, projectConfig.Version))
			using (var reader = command.ExecuteReader(_config.ProjectsTableConfig.Database))
			{
				if (reader.HasRows == false || reader.Read() == false || reader.IsDBNull(0))
				{
					_logger.AddEntry("never encountered this project before, continuing.");
					return true;
				}

				databaseVersion = new Version(reader.GetString(0));
			}

			_logger.AddEntry("database version is '{0}' and config version is '{1}'", databaseVersion, projectConfig.Version);

			if (databaseVersion >= projectVersion)
			{
				_logger.AddEntry("project is older than or equal to what is currently deployed.");
				return false;
			}

			_logger.AddEntry("project is newer than what is currently deployed, continuing.");
			return true;
		}

		public void Dispose()
		{
		}

		private readonly IScriptDeployerConfig _config;
		private readonly IProject _project;
		private readonly ILoggerStringFormatWrapper _logger;
		private readonly ISqlConnectionManager _connectionManager;
		private readonly CatalogPatternLookup _patternLookup;
	}
}