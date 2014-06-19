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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Randal.Core.IO.Logging;
using Randal.Utilities.Sql.Deployer.Configuration;

namespace Randal.Utilities.Sql.Deployer.Process
{
	public interface IScriptDeployer
	{
	}

	public sealed class ScriptDeployer : IScriptDeployer, IDisposable
	{
		public ScriptDeployer(ISqlConnectionManager connectionManager, ILogger logger)
		{
			_connectionManager = connectionManager;
			_logger = new LoggerStringFormatDecorator(logger);
		}

		public bool CanUpgrade(IProjectConfig projectConfig)
		{
			CreateProductsTable();

			var valid = IsProjectValidUpgrade(projectConfig);

			if (valid)
				AddProductVersion(projectConfig);

			return valid;
		}

		public void CreateProductsTable()
		{
			_logger.AddEntry("Creating Product Version table.");

			using (var command = _connectionManager.CreateCommand(TextResources.Sql.CreateProductsTable))
			{
				command.Execute(TextResources.Sql.Database.Master);
			}
		}

		public void AddProductVersion(IProjectConfig projectConfig)
		{
			_logger.AddEntry("Adding this package's product and version to products table.");

			var values = new object[] { projectConfig.Project, projectConfig.Version, Environment.MachineName, Environment.UserName };
			using (var command = _connectionManager.CreateCommand(TextResources.Sql.InsertProduct, values))
			{
				command.Execute(TextResources.Sql.Database.Master);
			}
		}

		public bool IsProjectValidUpgrade(IProjectConfig projectConfig)
		{
			Version databaseVersion;

			var projectVersion = new Version(projectConfig.Version);

			using (var command = _connectionManager.CreateCommand(TextResources.Sql.GetProductVersion, projectConfig.Project, projectConfig.Version))
			using (var reader = command.ExecuteReader(TextResources.Sql.Database.Master))
			{
				if (reader.HasRows == false || reader.Read() == false || reader.IsDBNull(0))
				{
					_logger.AddEntry("Never encountered this project before, continuing.");
					return true;
				}

				databaseVersion = new Version(reader.GetString(0));
			}
		
			_logger.AddEntry("Found version in database for '{0}' as '{1}'", projectConfig.Project, databaseVersion);

			if (databaseVersion >= projectVersion)
			{
				_logger.AddEntry("Project is older than or equal to what is currently deployed.");
				return false;
			}

			_logger.AddEntry("Project is newer than what is currently deployed, continuing.");
			return true;
		}

		public void Dispose()
		{
			_connectionManager.Dispose();
		}

		private readonly LoggerStringFormatDecorator _logger;
		private readonly ISqlConnectionManager _connectionManager;
	}
}