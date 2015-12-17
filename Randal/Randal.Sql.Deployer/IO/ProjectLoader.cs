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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Randal.Core.Enums;
using Randal.Logging;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Scripts;

namespace Randal.Sql.Deployer.IO
{
	public sealed class ProjectLoader
	{
		public ProjectLoader(string projectPath, IScriptParserConsumer scriptParser, IScriptCheckerConsumer scriptChecker = null, ILoggerSync logger = null)
		{
			_logger = logger ?? new NullLogger();

			ProjectPath = projectPath;
			ScriptParser = scriptParser;
			ScriptChecker = scriptChecker;
			_allScripts = new List<SourceScript>();
		}

		public string ProjectPath { get; private set; }

		public IProjectConfig Configuration { get; private set; }

		public IReadOnlyList<SourceScript> AllScripts
		{
			get { return _allScripts; }
		}

		public Returned Load()
		{
			var projectDirectory = new DirectoryInfo(ProjectPath);

			_logger.PostEntry("load configuration");

			if (LoadConfiguration(projectDirectory) == Returned.Failure)
				return Returned.Failure;

			_logger.PostEntry("project '{0}' : '{1}'", Configuration.Project, Configuration.Version);
			_logger.PostEntry("priority scripts : [{0}]", string.Join(", ", Configuration.PriorityScripts));

			_logger.PostEntry("validating scripts");
			return LoadAndValidateScripts(projectDirectory);
		}

		private Returned LoadAndValidateScripts(DirectoryInfo projectDirectory)
		{
			var scriptFiles = projectDirectory.GetFiles("*.sql", SearchOption.AllDirectories);
			var result = Returned.Success;
			var errors = 0;
			var check = ScriptCheck.Passed;
			var messages = new List<string>();

			foreach (var file in scriptFiles)
			{
				string text;
				messages.Clear();

				using (var reader = file.OpenText())
				{
					text = reader.ReadToEnd();
				}

				text = ReplaceVars(text, messages);

				if(ScriptChecker != null)
					check = ScriptChecker.Validate(text, messages);

				var script = ScriptParser.Parse(file.Name, text);
				script.Validate(messages);

				if ((check == ScriptCheck.Passed || check == ScriptCheck.Warning) && script.IsValid && messages.Count == 0)
				{
					_allScripts.Add(script);
					continue;
				}

				LogScriptIssues(file.FullName, messages);
				result = Returned.Failure;
				errors++;
			}

			_logger.PostEntry("loaded and parsed {0} file(s), {1} had errors", scriptFiles.Length, errors);

			return result;
		}

		private string ReplaceVars(string text, ICollection<string> validationMessages)
		{
			return VarReplacementPattern.Replace(text, match =>
			{
				string value;
				var key = match.Groups[1].Value;
				if (Configuration.Vars.TryGetValue(key, out value) == false)
				{
					value = string.Empty;
					validationMessages.Add("Failed to find value to replace key '" + key + "'.");
				}

				return value;
			});
		}

		private void LogScriptIssues(string name, IEnumerable<string> messages)
		{
			if (messages == null)
				return;

			_logger.PostEntry(name);

			foreach (var message in messages)
				_logger.PostEntryNoTimestamp(message);
		}

		private Returned LoadConfiguration(DirectoryInfo projectDirectory)
		{
			FileInfo configFileJson, configFileXml;

			try
			{
				configFileJson = projectDirectory.GetFiles("config.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
				configFileXml = projectDirectory.GetFiles("config.xml", SearchOption.TopDirectoryOnly).FirstOrDefault();
			}
			catch (DirectoryNotFoundException ex)
			{
				_logger.PostException(ex, "project directory not found at '" + projectDirectory.FullName + "'");
				return Returned.Failure;
			}

			if (configFileJson == null && configFileXml == null)
			{
				_logger.PostEntry(Verbosity.Vital, "no configuration file found for project (config.json or config.xml).");
				return Returned.Failure;
			}

			if (configFileJson != null && configFileXml != null)
			{
				_logger.PostEntry(Verbosity.Vital, "ambiguous configuration files found for project (config.json and config.xml).");
				return Returned.Failure;
			}

			ReadProjectConfigurationFile(configFileJson ?? configFileXml);

			return Returned.Success;
		}

		private void ReadProjectConfigurationFile(FileInfo configFile)
		{
			try
			{
				using (var reader = configFile.OpenText())
				{
					if (configFile.Extension.Equals(".json", StringComparison.OrdinalIgnoreCase))
					{
							Configuration = JsonConvert.DeserializeObject<ProjectConfigJson>(reader.ReadToEnd());
					}
					else
					{
						var xs = new XmlSerializer(typeof(ProjectConfigXml));
						Configuration = (ProjectConfigXml)xs.Deserialize(reader.BaseStream);
					}
				}
			}
			catch (JsonReaderException jre)
			{
				throw new JsonReaderException("error loading config.json file, see inner exception details.", jre);
			}

			IList<string> messages;

			if (Configuration.Validate(out messages) == false)
			{
				throw new InvalidOperationException("Errors found validating project configuration. "
				                                    + Environment.NewLine
				                                    + string.Join(Environment.NewLine, messages));
			}
		}

		private readonly ILoggerSync _logger;
		private readonly List<SourceScript> _allScripts;
		private IScriptParserConsumer ScriptParser { get; set; }
		private IScriptCheckerConsumer ScriptChecker { get; set; }
		private static readonly Regex VarReplacementPattern = new Regex(@"\$(?<key>" + ProjectConfigBase.ValidVarPattern + @")\$",
			RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);
	}
}