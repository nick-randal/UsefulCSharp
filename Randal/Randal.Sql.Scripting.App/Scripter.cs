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
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting.App
{
	public sealed class Scripter
	{
		private readonly Server _server;
		private readonly IScriptFormatter _formatter;

		public Scripter(string serverName, IScriptFormatter formatter = null)
		{
			_server = new Server(serverName);
			var depWalker = new DependencyWalker(_server);
			_formatter = formatter ?? new ScriptFormatter(depWalker);
		}

		public void DumpScripts()
		{
			var databases = _server.Databases.Cast<Database>().ToList();

			foreach (var db in databases)
			{
				try
				{
					var folderPath = CreateDirectory(@"C:\__dev\Database\Dump", db);
					ProcessSprocs(db.StoredProcedures.Cast<StoredProcedure>().ToList(), folderPath);
				}
				catch (ExecutionFailureException ex)
				{
					Console.WriteLine(ex);
				}
			}
		}

		private static string CreateDirectory(string basePath, Database database)
		{
			var dbPath = Path.Combine(basePath, database.Name, "Sprocs");

			var directory = new DirectoryInfo(dbPath);

			if (directory.Exists)
			{
				foreach (var file in directory.GetFiles("*.sql", SearchOption.AllDirectories))
					file.Delete();
			}
			else
				directory.Create();

			return dbPath;
		}

		private void ProcessSprocs(IEnumerable<StoredProcedure> list, string folderPath)
		{
			foreach (
				var sproc in list.Where(s => s.ImplementationType == ImplementationType.TransactSql && s.IsSystemObject == false))
			{
				if (sproc.Schema != "dbo")
					Console.WriteLine("{0} {1}", sproc.Schema, sproc.Name);

				WriteScriptFile(folderPath, sproc.Name, _formatter.Format(sproc));
			}
		}

		private static void WriteScriptFile(string folder, string name, string text)
		{
			var script = new FileInfo(Path.Combine(folder, name + ".sql"));

			using (var writer = new StreamWriter(script.OpenWrite()))
			{
				writer.WriteLine(text);
			}
		}
	}
}