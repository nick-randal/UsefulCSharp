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
using System.IO;

namespace Randal.Sql.Scripting
{
	public interface IScriptFileManager
	{
		string CurrentFolder { get; }
		void CreateDirectory(string databaseName, string subFolder);
		void WriteScriptFile(string name, string text);
	}

	public sealed class ScriptFileManager : IScriptFileManager
	{
		public ScriptFileManager(string basePath)
		{
			_basePath = basePath;
		}

		public string CurrentFolder { get; private set; }

		public void CreateDirectory(string databaseName, string subFolder)
		{
			var dbPath = Path.Combine(_basePath, databaseName, subFolder);

			var directory = new DirectoryInfo(dbPath);

			if (directory.Exists)
			{
				foreach (var file in directory.GetFiles(Wildcard + SqlExtension, SearchOption.AllDirectories))
					file.Delete();
			}
			else
				directory.Create();

			CurrentFolder = dbPath;
		}

		public void WriteScriptFile(string name, string text)
		{
			var script = new FileInfo(Path.Combine(CurrentFolder, name + SqlExtension));

			using (var writer = new StreamWriter(script.OpenWrite()))
			{
				writer.WriteLine(text);
			}
		}

		private readonly string _basePath;
		private const string SqlExtension = ".sql", Wildcard = "*";
	}
}