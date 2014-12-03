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
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Randal.Core.Enums;
using Randal.Logging;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Sql.Deployer.IO
{
	public sealed class ProjectLoader
	{
		public ProjectLoader(string projectPath, IScriptParserConsumer scriptParser, IScriptCheckerConsumer scriptChecker = null, ILogger logger = null)
		{
			_logger = new LoggerStringFormatWrapper(logger ?? new NullLogger());

			ProjectPath = projectPath;
			ScriptParser = scriptParser;
			ScriptChecker = scriptChecker;
			_allScripts = new List<SourceScript>();
		}

		public string ProjectPath { get; private set; }

		public ProjectConfig Configuration { get; private set; }

		public IReadOnlyList<SourceScript> AllScripts
		{
			get { return _allScripts; }
		}

		public Returned Load()
		{
			var projectDirectory = new DirectoryInfo(ProjectPath);

			_logger.AddEntry("load configuration");

			if (LoadConfiguration(projectDirectory) == Returned.Failure)
				return Returned.Failure;

			_logger.AddEntry("project '{0}' : '{1}'", Configuration.Project, Configuration.Version);
			_logger.AddEntry("priority scripts : [{0}]", string.Join(", ", Configuration.PriorityScripts));

			_logger.AddEntry("validating scripts");
			return LoadAndValidateScripts(projectDirectory);
		}

		private Returned LoadAndValidateScripts(DirectoryInfo projectDirectory)
		{
			var scriptFiles = projectDirectory.GetFiles("*.sql", SearchOption.AllDirectories);
			var result = Returned.Success;
			var errors = 0;
			var check = ScriptCheck.Passed;

			foreach (var file in scriptFiles)
			{
				using (var reader = file.OpenText())
				{
					IEnumerable<string> checkMessages = null;

					var text = reader.ReadToEnd();

					if(ScriptChecker != null)
						check = ScriptChecker.Validate(text, out checkMessages);
					var script = ScriptParser.Parse(file.Name, text);
					var validationMessages = script.Validate();

					if ((check == ScriptCheck.Passed || check == ScriptCheck.Warning) && script.IsValid && validationMessages.Count == 0)
					{
						_allScripts.Add(script);
						continue;
					}

					result = Returned.Failure;
					errors++;
					LogScriptIssues(file.Name, checkMessages, validationMessages);
				}
			}

			_logger.AddEntry("loaded and parsed {0} file(s), {1} had errors", scriptFiles.Length, errors);

			return result;
		}

		private void LogScriptIssues(string name, IEnumerable<string> checkMessages, IEnumerable<string> validationMessages)
		{
			_logger.AddEntry(name);

			if (checkMessages != null)
			{
				foreach (var message in checkMessages)
					_logger.AddEntryNoTimestamp(message);
			}

			foreach (var message in validationMessages)
				_logger.AddEntryNoTimestamp(message);
		}

		private Returned LoadConfiguration(DirectoryInfo projectDirectory)
		{
			FileInfo configFile;

			try
			{
				configFile = projectDirectory.GetFiles("config.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
			}
			catch (DirectoryNotFoundException ex)
			{
				_logger.AddException(ex, "project directory not found at '" + projectDirectory.FullName + "'");
				return Returned.Failure;
			}

			if (configFile == null)
			{
				_logger.AddEntry(Verbosity.Vital, "no 'config.json' configuration file found for project.");
				return Returned.Failure;
			}

			using (var reader = configFile.OpenText())
			{
				try
				{
					Configuration = JsonConvert.DeserializeObject<ProjectConfig>(reader.ReadToEnd());
				}
				catch (JsonReaderException jre)
				{
					throw new JsonReaderException("error loading config.json file, see inner exception details.", jre);
				}
			}

			return Returned.Success;
		}

		private readonly ILoggerStringFormatWrapper _logger;
		private readonly List<SourceScript> _allScripts;
		private IScriptParserConsumer ScriptParser { get; set; }
		private IScriptCheckerConsumer ScriptChecker { get; set; }
	}
}