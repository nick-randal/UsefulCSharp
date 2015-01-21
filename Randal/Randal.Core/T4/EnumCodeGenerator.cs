// Useful C#
// Copyright (C) 2015 Nicholas Randal
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
using System.Data.SqlClient;
using System.IO;

namespace Randal.Core.T4
{
	public class EnumCodeGenerator : IEnumCodeGenerator
	{
		public EnumCodeGenerator(string connectionString)
		{
			_connectionString = connectionString;
		}

		public IReadOnlyList<DbCodeDefinition> GetCodes(string commandText, CommandType commandType)

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

		public IReadOnlyList<string> GetDefinitionList(string commandText, CommandType commandType)
		{
			var lines = new List<String>();

			try
			{
				var codes = GetCodes(commandText, commandType);

				foreach (var code in codes)
				{
					lines.Add(FormattedAttribute(code));
					lines.Add(FormattedDefinition(code));
					lines.Add(string.Empty);
				}

				if (lines.Count > 2)
				{
					lines.RemoveAt(lines.Count - 1);
					lines[lines.Count - 1] = lines[lines.Count - 1].TrimEnd(',');
				}
			}
			catch (Exception ex)
			{
				lines.Add("/*");
				lines.Add(ex.ToString());
				lines.Add("*/");
			}

			return lines.AsReadOnly();
		}

		private static string FormattedDefinition(DbCodeDefinition code)
		{
			return string.Format("{0} = {1},", code.NameAsCSharpProperty, code.Code);
		}

		private static string FormattedAttribute(DbCodeDefinition code)
		{
			return string.Format(@"[Display(Name = ""{0}"", Description = ""{1}"")]", code.DisplayName, code.Description);
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