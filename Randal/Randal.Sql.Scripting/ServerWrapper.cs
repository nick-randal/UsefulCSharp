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

using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting
{
	public interface IServer
	{
		IEnumerable<Database> GetDatabases();
		IEnumerable<StoredProcedure> GetStoredProcedures(Database database);
		IEnumerable<UserDefinedFunction> GetUserDefinedFunctions(Database database);
		IEnumerable<View> GetViews(Database database);
		IEnumerable<Table> GetTables(Database database);
		IEnumerable<DependencyCollectionNode> GetDependencies(SqlSmoObject smo);
		SqlSmoObject GetSchemaObject(string urn);
		string Name { get; }
	}

	public sealed class ServerWrapper : IServer
	{
		public ServerWrapper(Server server)
		{
			_server = server;
		}

		public string Name { get { return _server.Name; } }

		public IEnumerable<Database> GetDatabases()
		{
			return _server.Databases.Cast<Database>().ToList();
		}

		public SqlSmoObject GetSchemaObject(string urn)
		{
			return _server.GetSmoObject(urn);
		}

		public IEnumerable<StoredProcedure> GetStoredProcedures(Database database)
		{
			return database.StoredProcedures
				.Cast<StoredProcedure>()
				.Where(sproc => sproc.ImplementationType == ImplementationType.TransactSql && sproc.IsSystemObject == false)
				.ToList()
				.AsReadOnly();
		}

		public IEnumerable<UserDefinedFunction> GetUserDefinedFunctions(Database database)
		{
			return database.UserDefinedFunctions
				.Cast<UserDefinedFunction>()
				.Where(udf => udf.ImplementationType == ImplementationType.TransactSql && udf.IsSystemObject == false)
				.ToList()
				.AsReadOnly();
		}

		public IEnumerable<View> GetViews(Database database)
		{
			return database.Views
				.Cast<View>()
				.Where(view => view.IsSystemObject == false)
				.ToList()
				.AsReadOnly();
		}

		public IEnumerable<Table> GetTables(Database database)
		{
			return database.Tables
				.Cast<Table>()
				.Where(table => table.IsSystemObject == false)
				.ToList()
				.AsReadOnly();
		}

		public IEnumerable<DependencyCollectionNode> GetDependencies(SqlSmoObject smo)
		{
			var dependencyWalker = new DependencyWalker(_server);
			var depTree = dependencyWalker.DiscoverDependencies(new[] { smo }, true);

			return dependencyWalker.WalkDependencies(depTree);
		}

		private readonly Server _server;
	}
}