// Useful C#
// Copyright (C) 2015-2016 Nicholas Randal
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

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Randal.Core.T4
{
	public class CodeGenerator : ICodeGenerator
	{
		public CodeGenerator(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IReadOnlyList<DbCodeDefinition> GetCodeDefinitions(string commandText, CommandType commandType)

		{
			var codes = new List<DbCodeDefinition>();

			using (var connection = OpenConnection())
			using (var command = new SqlCommand(commandText, connection) { CommandType = commandType })
			using(var reader = command.ExecuteReader())
			{
				if (reader.HasRows == false)
					return codes.AsReadOnly();

				if(reader.FieldCount < 4)
					throw new InvalidDataException("SQL command provided should have returned 4 fields but returned " + reader.FieldCount.ToString());

				while (reader.Read())
				{
					codes.Add(
						new DbCodeDefinition(reader.GetValue(0), reader.GetValue(1), reader.GetValue(2), reader.GetValue(3))
					);
				}
			}

			return codes.AsReadOnly();
		}

		private SqlConnection OpenConnection()
		{
			var connection = new SqlConnection(_connectionString);
			connection.Open();
			return connection;
		}

		private readonly string _connectionString;
	}
}