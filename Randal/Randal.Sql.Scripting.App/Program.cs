using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Sql.Scripting.App
{
	public sealed class Program
	{
		public const string Local = ".", Staging = "TCSQLStaging01";

		static void Main(string[] args)
		{
			var server = new Server(Local);
			var depWalker = new DependencyWalker(server);
			var databases = server.Databases.Cast<Database>().ToList();

			foreach (var db in databases)
			{
				try
				{
					var folderPath = CreateDirectory(@"C:\__dev\Database\Dump", db);
					ProcessSprocs(depWalker, db, db.StoredProcedures.Cast<StoredProcedure>().ToList(), folderPath);
				}
				catch (ExecutionFailureException ex)
				{
					Console.WriteLine(ex);
				}
			}
		}

		static string CreateDirectory(string basePath, Database database)
		{
			var list = database.StoredProcedures.Cast<StoredProcedure>().ToList();
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

		static void ProcessSprocs(DependencyWalker walker, Database db, IEnumerable<StoredProcedure> list, string folderPath)
		{
			foreach (var sproc in list.Where(s => s.ImplementationType == ImplementationType.TransactSql && s.IsSystemObject == false))
			{
				if (sproc.Schema != "dbo")
					Console.WriteLine("{0} {1}", sproc.Schema, sproc.Name);

				WriteScriptFile(folderPath, sproc.Name, Format(sproc, walker));
			}
		}

		static void WriteScriptFile(string folder, string name, string text)
		{
			var script = new FileInfo(Path.Combine(folder, name + ".sql"));

			using (var writer = new StreamWriter(script.OpenWrite()))
			{
				writer.WriteLine(text);
			}
		}

		static string Format(StoredProcedure sproc, DependencyWalker walker)
		{
			var header = sproc.TextHeader.Replace("create procedure", "alter procedure");
			var parameters = sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = ");
			var needs = GetNeeds(sproc, walker);

			return string.Format(SprocScript,
				sproc.Parent.Name,
				sproc.Name,
				header,
				sproc.TextBody,
				string.Join(", ", parameters),
				needs
			);
		}

		static string GetNeeds(StoredProcedure sproc, DependencyWalker walker)
		{
			var depTree = walker.DiscoverDependencies(new[] { sproc }, true);

			var dependencies = walker.WalkDependencies(depTree)
				.Cast<DependencyNode>()
				.Where(x => x.Urn.Value != sproc.Urn && x.Urn.Type != "Table")
				.Select(x => x.Urn.GetNameForType(x.Urn.Type))
				.ToList();

			if (dependencies.Count == 0)
				return string.Empty;

			return "--:: need " + string.Join(", ", dependencies) + Environment.NewLine;
		}

		const string SprocScript =
		@"{5}--:: catalog {0}

--:: ignore
use {0}

--:: pre
exec coreCreateProcedure '{1}'
GO

--:: main
{2}
{3}

/*
	exec {1} {4}
*/";
	}
}
