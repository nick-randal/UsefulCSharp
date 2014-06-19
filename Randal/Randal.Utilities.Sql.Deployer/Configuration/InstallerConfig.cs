using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Randal.Utilities.Sql.Deployer.Configuration
{
	public interface IProjectConfig
	{
		IReadOnlyList<string> PriorityScripts { get; }
		string Version { get; }
		string Project { get; }
	}

	public sealed class ProjectConfig : IProjectConfig
	{
		public ProjectConfig(JObject jsonObject)
		{
			if(jsonObject == null)
				throw new ArgumentNullException("jsonObject");

			var scripts = jsonObject["PriorityScripts"] ?? new JArray();
			_priorityScripts = new List<string>(scripts.Select(s => s.Value<string>()));

			Version = (string)jsonObject["Version"] ?? "01.01.01.01";
			Project = (string)jsonObject["Project"] ?? "Unknown";
		}

		public ProjectConfig(string project, string version, IEnumerable<string> priorityScripts)
		{
			Project = project;
			Version = version;
			_priorityScripts = new List<string>(priorityScripts);
		}

		public IReadOnlyList<string> PriorityScripts
		{
			get { return _priorityScripts; }
		}

		public string Version { get; private set; }

		public string Project { get; private set; }

		private readonly List<string> _priorityScripts;
	}
}
