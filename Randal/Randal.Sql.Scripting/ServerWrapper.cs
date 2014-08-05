// Useful C#
// Copyright (C) 2014 Nicholas Randal
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
	}

	public sealed class ServerWrapper : IServer
	{
		public ServerWrapper(string serverInstance)
		{
			_server = new Server(serverInstance);
			_dependencyWalker = new DependencyWalker(_server);
		}

		public IEnumerable<Database> GetDatabases()
		{
			return _server.Databases.Cast<Database>().ToList();
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
				.Where(view =>
				{
					if(view.IsIndexable || view.IsSchemaBound)
						throw new InvalidOperationException("Unexpected view encountered.");
					return view.IsSystemObject == false;
				})
				.ToList()
				.AsReadOnly();
		}

		public IEnumerable<Table> GetTables(Database database)
		{
			return database.Tables
				.Cast<Table>()
				.Where(view => view.IsSystemObject == false)
				.ToList()
				.AsReadOnly();
		}

		public IEnumerable<DependencyCollectionNode> GetDependencies(SqlSmoObject smo)
		{
			var depTree = _dependencyWalker.DiscoverDependencies(new[] { smo }, true);

			return _dependencyWalker.WalkDependencies(depTree);
		}

		private readonly DependencyWalker _dependencyWalker;
		private readonly Server _server;
	}
}