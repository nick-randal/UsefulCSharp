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
using System.Data.Common;
using System.Data.SqlClient;

namespace Randal.Sql.Deployer.Process
{
	public interface ISqlConnectionManager : IDisposable
	{
		string Server { get; }
		IReadOnlyList<string> DatabaseNames { get; }

		void OpenConnection(string newServer, string database, string userName, string password);
		void OpenConnection(string newServer, string database);

		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();

		ISqlCommandWrapper CreateCommand(string raw, params object[] values);
	}

	public sealed class SqlConnectionManager : ISqlConnectionManager
	{
		private readonly string _getDatabaseCommand;

		public SqlConnectionManager(string getDatabaseCommand, ISqlCommandWrapperFactory commandFactory = null)
		{
			_getDatabaseCommand = getDatabaseCommand;
			_databaseNames = new List<string>();
			_scnBuilder = new SqlConnectionStringBuilder();
			_commandWrapperFactory = commandFactory ?? new SqlCommandWrapperFactory();
		}

		public string Server
		{
			get { return _scnBuilder.DataSource; }
		}

		public void OpenConnection(string newServer, string database)
		{
			OpenConnection(newServer, database, null, null);
		}

		public void OpenConnection(string server, string database, string userName, string password)
		{
			_scnBuilder.DataSource = server;
			_scnBuilder.InitialCatalog = database;
			_scnBuilder.ConnectTimeout = 30;

			if (string.IsNullOrEmpty(userName) || password == null)
				_scnBuilder.IntegratedSecurity = true;
			else
			{
				_scnBuilder.IntegratedSecurity = false;
				_scnBuilder.UserID = userName;
				_scnBuilder.Password = password;
			}

			_connection = GetNewSqlConnection(_scnBuilder);

			GetDatabaseNames();
		}

		public void BeginTransaction()
		{
			if (_connection == null || _connection.State != ConnectionState.Open)
				throw new InvalidOperationException("Cannot begin transaction without and valid server connection.");

			_transaction = _connection.BeginTransaction(IsolationLevel.Serializable);
		}

		public void CommitTransaction()
		{
			if (_transaction == null)
				return;

			_transaction.Commit();
			_transaction.Dispose();
			_transaction = null;
		}

		public void RollbackTransaction()
		{
			if (_transaction == null)
				return;

			_transaction.Rollback();
			_transaction.Dispose();
			_transaction = null;
		}

		public IReadOnlyList<string> DatabaseNames
		{
			get { return _databaseNames; }
		}

		public ISqlCommandWrapper CreateCommand(string raw, params object[] values)
		{
			return _commandWrapperFactory.CreateCommand(_connection, _transaction, raw, values);
		}

		public void Dispose()
		{
			try
			{
				if (_connection != null)
					_connection.Dispose();

				if (_transaction != null)
					_transaction.Dispose();
			}
			catch (SqlException)
			{
			}
			finally
			{
				_connection = null;
				_transaction = null;
			}
		}

		private static SqlConnection GetNewSqlConnection(DbConnectionStringBuilder builder)
		{
			var connection = new SqlConnection(builder.ConnectionString);

			connection.Open();
			if (connection.State == ConnectionState.Open)
				return connection;

			connection.Dispose();
			return null;
		}

		private void GetDatabaseNames()
		{
			using (var cmd = CreateCommand(_getDatabaseCommand))
			{
				using (var reader = cmd.ExecuteReader("master"))
				{
					while (reader.Read())
						_databaseNames.Add(reader.GetString(0));
				}
			}

			_databaseNames.Sort();
		}

		private SqlConnection _connection;
		private SqlTransaction _transaction;
		private readonly SqlConnectionStringBuilder _scnBuilder;
		private readonly ISqlCommandWrapperFactory _commandWrapperFactory;
		private readonly List<string> _databaseNames;
	}
}