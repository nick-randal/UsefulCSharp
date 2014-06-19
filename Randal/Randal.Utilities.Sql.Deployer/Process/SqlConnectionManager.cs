using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Randal.Core.IO.Logging;

namespace Randal.Utilities.Sql.Deployer.Process
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
		public SqlConnectionManager(ISqlCommandWrapperFactory commandFactory)
		{
			_databaseNames = new List<string>();
			_scnBuilder = new SqlConnectionStringBuilder();
			_commandWrapperFactory = commandFactory ?? new SqlCommandWrapperFactory();
		}

		public string Server { get { return _scnBuilder.DataSource; } }

		public void OpenConnection(string newServer, string database)
		{
			OpenConnection(newServer, database, null, null);
		}

		public void OpenConnection(string newServer, string database, string userName, string password)
		{
			_scnBuilder.DataSource = Server;
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
			if(_connection == null || _connection.State != ConnectionState.Open)
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
			catch (SqlException) {}
			finally
			{
				_connection = null;
				_transaction = null;
			}
		}

		private static SqlConnection GetNewSqlConnection(DbConnectionStringBuilder builder)
		{
			try
			{
				var connection = new SqlConnection(builder.ConnectionString);

				connection.Open();
				if (connection.State == ConnectionState.Open)
					return connection;

				connection.Dispose();
				return null;

			}
			catch (SqlException)
			{
				return null;
			}
		}

		private void GetDatabaseNames()
		{
			using (var cmd = CreateCommand(TextResources.Sql.GetDatabases))
			{
				using (var reader = cmd.ExecuteReader(TextResources.Sql.Database.Master))
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