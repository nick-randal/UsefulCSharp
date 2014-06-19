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
			if (projectConfig == null)
				return;

			_logger.AddEntry("Adding this package's product and version to products table.");

			var values = new object[] { projectConfig.Project, projectConfig.Version, Environment.MachineName, Environment.UserName };
			using (var command = _connectionManager.CreateCommand(TextResources.Sql.InsertProduct, values))
			{
				command.Execute(TextResources.Sql.Database.Master);
			}
		}

		public void Dispose()
		{
			
		}

		/*
		private int ExecuteText(string dbName, string command, params object[] values)
		{
			int ret = 0;
			SqlCommand cmd = null;

			if (string.IsNullOrEmpty(dbName) == false)
				_sqlcn.ChangeDatabase(dbName);

			if (string.IsNullOrEmpty(command))
			{
				var st = new StackTrace(true);
				_logger.Add(st.ToString());
				throw new ArgumentNullException("program");
			}

			using (cmd = new SqlCommand(string.Empty, _sqlcn))
			{
				cmd.CommandTimeout = 30;
				cmd.CommandType = CommandType.Text;
				if (_sqlTransaction != null)
					cmd.Transaction = _sqlTransaction;

				cmd.CommandText = string.Format(command, values);
				cmd.ExecuteNonQuery();
			}

			return ret;
		}*/

		/*
		public void ExecuteSqlText(string title, string commandText, string[] dataSources, bool noTrans)
		{
			SqlCommand cmd = null;
			Collection<string> sourceDatabases;

			if (string.IsNullOrEmpty(commandText) || dataSources.Length < 1)
				return;

			using (cmd = new SqlCommand(commandText))
			{
				if (_transaction == null || noTrans)
					cmd.Connection = NewSqlInstance;
				else
				{
					cmd.Connection = _connection;
					cmd.Transaction = _transaction;
				}

				cmd.CommandTimeout = LongTimeout;
				cmd.CommandType = CommandType.Text;

				// a datasource may have wild cards so for each datasource retrieve the real database names
				foreach (string dataSource in dataSources)
				{
					// get the list of databases based on the qualifier
					sourceDatabases = GetDatabases(dataSource);

					// execute the command for each database
					foreach (string database in sourceDatabases)
					{
						Log("-- executing in {0}", database);
						_connection.ChangeDatabase(database);
						cmd.ExecuteNonQuery();
					}
				}
			}
		}

		
		public void CreateProductsTable()
		{
			_logger.Add(new LogEntry("  Creating Product Version table."));

			ExecuteText(TextResources.Database.Master, TextResources.CreateProductTable);
		}

		public void AddProductVersion(InfoSection infoSection)
		{
			if (infoSection == null)
				return;

			_logger.Add(new LogEntry("Adding this package's product and version to products table."));)
			;

			ExecuteText(TextResources.Database.Master,
				Resources.AddProductVersion,
				infoSection.Product,
				infoSection.Package,
				infoSection.BuildVersion,
				Environment.MachineName,
				Environment.UserName);
		}

		public ErrorCode IsProductUpgrade(InfoSection infoSection)
		{
			SqlCommand cmd = null;
			SqlDataReader reader;
			Version verDatabase, verPackage;

			if (infoSection == null)
				return ErrorCode.Unexpected;

			verPackage = new Version(infoSection.BuildVersion);
			_logger.Add(Resources.ValidatingPackageVersion);

			using (cmd = new SqlCommand())
			{
				cmd.Connection = _sqlcn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = string.Format(Resources.GetProductVersion,
					infoSection.Product, infoSection.Package, infoSection.BuildVersion);

				using (reader = cmd.ExecuteReader())
				{
					// if no rows are returned then there has not been a package of this type installed before
					if (reader.HasRows == false || reader.Read() == false || reader.IsDBNull(0))
						return ErrorCode.NoError;

					verDatabase = new Version(reader.GetString(0));
				}
			}

			if (verDatabase > verPackage)
				return ErrorCode.OldPackageVersion;
			else if (verDatabase == verPackage)
				return ErrorCode.SamePackageVersion;
			else
				return ErrorCode.NoError;
		}
*/

		private readonly LoggerStringFormatDecorator _logger;
		private readonly ISqlConnectionManager _connectionManager;
	}
}