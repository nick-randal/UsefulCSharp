using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Randal.Utilities.Sql.Deployer.Configuration;
using Randal.Utilities.Sql.Deployer.Scripts;

namespace Randal.Utilities.Sql.Deployer.IO
{
	public sealed class ProjectLoader
	{
		public ProjectLoader(string projectPath, IScriptParserConsumer scriptParser = null)
		{
			ProjectPath = projectPath;
			Parser = scriptParser ?? new ScriptParserFactory().CreateStandardParser();
			_allScripts = new List<SourceScript>();
		}

		public string ProjectPath { get; private set; }

		public ProjectConfig Configuration { get; private set; }

		public IReadOnlyList<SourceScript> AllScripts { get { return _allScripts; } } 

		public IReadOnlyList<string> Load()
		{
			var messages = new List<string>();
			var projectDirectory = new DirectoryInfo(ProjectPath);

			if (LoadConfiguration(projectDirectory, messages) == false) 
				return messages;
			
			LoadAndValidateScripts(projectDirectory, messages);

			return messages;
		}

		private void LoadAndValidateScripts(DirectoryInfo projectDirectory, List<string> messages)
		{
			var scriptFiles = projectDirectory.GetFiles("*.sql", SearchOption.AllDirectories);
			foreach (var file in scriptFiles)
			{
				using (var reader = file.OpenText())
				{
					var text = reader.ReadToEnd();

					var script = Parser.Parse(file.Name, text);
					var validationMessages = script.Validate();

					if (script.IsValid == false || validationMessages.Count > 0)
						messages.AddRange(validationMessages);
					else
						_allScripts.Add(script);
				}
			}
		}

		private bool LoadConfiguration(DirectoryInfo projectDirectory, ICollection<string> messages)
		{
			FileInfo configFile;
			try
			{
				configFile = projectDirectory.GetFiles("config.json", SearchOption.TopDirectoryOnly).FirstOrDefault();
			}
			catch (DirectoryNotFoundException)
			{
				messages.Add("Project directory not found at '" + projectDirectory.FullName + "'");
				return false;
			}

			if (configFile == null)
			{
				messages.Add("No 'config.json' configuration file found for project.");
				return false;
			}

			using (var reader = configFile.OpenText())
			{
				Configuration = new ProjectConfig(JObject.Parse(reader.ReadToEnd()));
			}

			return true;
		}

		private readonly List<SourceScript> _allScripts;
		private IScriptParserConsumer Parser { get; set; }
	}
}