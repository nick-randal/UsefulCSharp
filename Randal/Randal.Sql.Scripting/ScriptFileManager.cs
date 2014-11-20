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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Randal.Sql.Scripting
{
	public interface IScriptFileManager
	{
		string CurrentFolder { get; }
		string SetupDatabaseDirectory(string databaseName);
		string SetupScriptDirectory(string databaseName, string subFolder);
		void WriteScriptFile(string name, string text);
		Task WriteScriptFileAsync(string name, string text);
	}

	public sealed class ScriptFileManager : IScriptFileManager
	{
		public ScriptFileManager(string basePath)
		{
			_basePath = basePath;
		}

		public string CurrentFolder { get; private set; }

		public string SetupDatabaseDirectory(string databaseName)
		{
			var directory = new DirectoryInfo(Path.Combine(_basePath, databaseName));

			if (directory.Exists)
			{
				foreach (var file in directory.GetFiles(Wildcard + SqlExtension, SearchOption.AllDirectories))
					file.Delete();

				directory
					.GetDirectories(Wildcard, SearchOption.TopDirectoryOnly)
					.ToList()
					.ForEach(subDir => subDir.Delete(true));
			}
			else
				directory.Create();

			CreateConfigFile(directory.FullName, databaseName);

			return directory.FullName;
		}

		public string SetupScriptDirectory(string databaseName, string subFolder)
		{
			var directory = new DirectoryInfo(Path.Combine(_basePath, databaseName, subFolder));

			if (directory.Exists == false)
				directory.Create();

			CurrentFolder = directory.FullName;

			return directory.FullName;
		}

		private static void CreateConfigFile(string dbPath, string databaseName)
		{
			var file = new FileInfo(Path.Combine(dbPath, ConfigFileName));
			var version = DateTime.Today.ToString(VersionFormat);

			if (file.Exists == false)
				file.Create();

			using (var stream = new StreamWriter(file.OpenWrite()))
			{
				stream.Write(ConfigFileFormat, databaseName, version);
				stream.Flush();
			}
		}

		public IScriptFileManager DeleteAllFiles()
		{
			return this;
		}

		public void WriteScriptFile(string name, string text)
		{
			if (name.EndsWith(SqlExtension) == false)
				name += SqlExtension;

			var script = new FileInfo(Path.Combine(CurrentFolder, name));

			using (var writer = new StreamWriter(script.OpenWrite()))
			{
				writer.WriteLine(text);
			}
		}

		public async Task WriteScriptFileAsync(string name, string text)
		{
			if (name.EndsWith(SqlExtension) == false)
				name += SqlExtension;

			var script = new FileInfo(Path.Combine(CurrentFolder, name));

			using (var writer = new StreamWriter(script.OpenWrite()))
			{
				await writer.WriteLineAsync(text);
			}
		}

		private readonly string _basePath;
		private const string 
			SqlExtension = ".sql", 
			Wildcard = "*",
			VersionFormat = "yy.MM.dd.01",
			ConfigFileName = "config.json",
			ConfigFileFormat = @"{{
	""Project"": ""{0}"",
	""Version"": ""{1}"",
	""PriorityScripts"": [  ]
}}";

	}
}