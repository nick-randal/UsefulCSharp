/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Randal.Core.IO.Logging;
using Randal.Utilities.Sql.Deployer.Helpers;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Utilities.Sql.Deployer.Process
{
	public interface IScriptDeployer
	{
		bool CanUpgrade();
		void DeployScripts();
	}

	public sealed class ScriptDeployer : IScriptDeployer
	{
		public ScriptDeployer(IProject project, ISqlConnectionManager connectionManager, ILogger logger)
		{
			if(project == null)
				throw new ArgumentNullException("project");
			if(connectionManager == null)
				throw new ArgumentNullException("connectionManager");

			_project = project;
			_connectionManager = connectionManager;
			_logger = new LoggerStringFormatDecorator(logger ?? new NullLogger());
			_patternLookup = new CatalogPatternLookup();
		}

		public bool CanUpgrade()
		{
			CreateProductsTable();

			var valid = IsProjectValidUpgrade();

			if (valid)
				AddProductVersion();

			return valid;
		}

		public void DeployScripts()
		{
			DeployPhase(SqlScriptPhase.Pre);
			DeployPhase(SqlScriptPhase.Main);
			DeployPhase(SqlScriptPhase.Post);
		}

		private void DeployPhase(SqlScriptPhase sqlScriptPhase)
		{
			foreach (var script in _project.NonPriorityScripts.Where(s => s.HasSqlScriptPhase(sqlScriptPhase)))
			{
				var sql = script.RequestSqlScriptPhase(sqlScriptPhase);
				if (sql == null)
					continue;

				var configuration = script.GetConfiguration();

				using (var command = _connectionManager.CreateCommand(sql))
				{
					foreach (var catalog in GetCatalogs(script))
					{
						if(configuration == null)
							command.Execute(catalog);
						else
							command.Execute(catalog, configuration.Settings.Timeout);
					}
				}
			}
		}

		private IEnumerable<string> GetCatalogs(SourceScript script)
		{
			var matchingDatabases = new List<string>();

			foreach (var pattern in script.GetCatalogPatterns())
			{
				var rgx = _patternLookup[pattern];

				matchingDatabases.AddRange(
					_connectionManager.DatabaseNames
					.Where(dbName => rgx.IsMatch(dbName))
				);
			}

			return matchingDatabases.Distinct().OrderBy(x => x);
		}

		private void CreateProductsTable()
		{
			_logger.AddEntry("Creating Product Version table.");

			using (var command = _connectionManager.CreateCommand(TextResources.Sql.CreateProductsTable))
			{
				command.Execute(TextResources.Sql.Database.Master);
			}
		}

		private void AddProductVersion()
		{
			_logger.AddEntry("Adding this package's product and version to products table.");

			var values = new object[] { _project.Configuration.Project, _project.Configuration.Version, Environment.MachineName, Environment.UserName };
			using (var command = _connectionManager.CreateCommand(TextResources.Sql.InsertProduct, values))
			{
				command.Execute(TextResources.Sql.Database.Master);
			}
		}

		private bool IsProjectValidUpgrade()
		{
			Version databaseVersion;
			var config = _project.Configuration;

			var projectVersion = new Version(config.Version);

			using (var command = _connectionManager.CreateCommand(TextResources.Sql.GetProductVersion, config.Project, config.Version))
			using (var reader = command.ExecuteReader(TextResources.Sql.Database.Master))
			{
				if (reader.HasRows == false || reader.Read() == false || reader.IsDBNull(0))
				{
					_logger.AddEntry("Never encountered this project before, continuing.");
					return true;
				}

				databaseVersion = new Version(reader.GetString(0));
			}

			_logger.AddEntry("Found version in database for '{0}' as '{1}'", config.Project, databaseVersion);

			if (databaseVersion >= projectVersion)
			{
				_logger.AddEntry("Project is older than or equal to what is currently deployed.");
				return false;
			}

			_logger.AddEntry("Project is newer than what is currently deployed, continuing.");
			return true;
		}

		private readonly IProject _project;
		private readonly LoggerStringFormatDecorator _logger;
		private readonly ISqlConnectionManager _connectionManager;
		private CatalogPatternLookup _patternLookup;
	}
}