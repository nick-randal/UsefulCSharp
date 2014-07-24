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
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting
{
	public interface IServer
	{
		IEnumerable<Database> GetDatabases();
		IEnumerable<StoredProcedure> GetStoredProcedures(Database database);
		IEnumerable<Urn> GetDependencies(SqlSmoObject smo);
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
			return database.StoredProcedures.Cast<StoredProcedure>().Where(s => s.ImplementationType == ImplementationType.TransactSql && s.IsSystemObject == false).ToList();
		}

		public IEnumerable<Urn> GetDependencies(SqlSmoObject smo)
		{
			var depTree = _dependencyWalker.DiscoverDependencies(new[] { smo }, true);

			return _dependencyWalker.WalkDependencies(depTree)
				.Cast<DependencyNode>()
				.Select(x => x.Urn);
		}

		private readonly DependencyWalker _dependencyWalker;
		private readonly Server _server;
	}
}