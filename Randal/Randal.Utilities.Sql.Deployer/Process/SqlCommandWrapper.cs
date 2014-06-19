using System;
using System.Data;
using System.Data.SqlClient;

namespace Randal.Utilities.Sql.Deployer.Process
{
	public interface ISqlCommandWrapper : IDisposable
	{
		void Execute(string databaseName, int timeout = 30);
		SqlDataReader ExecuteReader(string databaseName, int timeout = 30);
	}

	public sealed class SqlCommandWrapper : ISqlCommandWrapper
	{
		public SqlCommandWrapper(SqlConnection connection, string commandText, params object[] values)
			: this(connection, null, commandText, values)
		{
			
		}

		public SqlCommandWrapper(SqlConnection connection, SqlTransaction transaction, string commandText, params object[] values)
		{
			_sqlCommand = new SqlCommand
			{
				Connection = connection,
				CommandType = CommandType.Text,
				CommandText = string.Format(commandText, values)
			};

			if (transaction != null)
				_sqlCommand.Transaction = transaction;
		}

		public void Execute(string databaseName, int timeout = 30)
		{
			if (databaseName == null)
				throw new ArgumentNullException("databaseName");

			if (timeout <= 0)
				timeout = 30;

			_sqlCommand.Connection.ChangeDatabase(databaseName);
			_sqlCommand.CommandTimeout = timeout;
			_sqlCommand.ExecuteNonQuery();
		}

		public SqlDataReader ExecuteReader(string databaseName, int timeout = 30)
		{
			if (databaseName == null)
				throw new ArgumentNullException("databaseName");

			if (timeout <= 0)
				timeout = 30;

			_sqlCommand.Connection.ChangeDatabase(databaseName);
			_sqlCommand.CommandTimeout = timeout;
			return _sqlCommand.ExecuteReader();
		}

		public void Dispose()
		{
 			_sqlCommand.Dispose();
		}

		private readonly SqlCommand _sqlCommand;
	}
}