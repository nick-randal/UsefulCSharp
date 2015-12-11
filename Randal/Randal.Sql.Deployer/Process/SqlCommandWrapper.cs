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
using System.Data;
using System.Data.SqlClient;

namespace Randal.Sql.Deployer.Process
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

		public SqlCommandWrapper(SqlConnection connection, SqlTransaction transaction, string commandText,
			params object[] values)
		{
			string formatted;

			try
			{
				formatted = values.Length == 0 ? commandText : string.Format(commandText, values);
			}
			catch (FormatException ex)
			{
				throw new FormatException("Failed to format sql command '" + commandText + "'", ex);
			}

			_sqlCommand = new SqlCommand
			{
				Connection = connection,
				CommandType = CommandType.Text,
				CommandText = formatted
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