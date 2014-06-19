using System;
using System.Collections.Generic;
using System.Linq;
using Randal.Utilities.Sql.Deployer.Configuration;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class Project
	{
		public Project(IProjectConfig config, IReadOnlyList<SourceScript> scripts)
		{
			if(config == null)
				throw new ArgumentNullException("config");
			if(scripts == null)
				throw new ArgumentNullException("scripts");

			Configuration = config;
			AllScripts = scripts;
			_scriptsLookup = AllScripts.ToDictionary(s => s.Name.ToLower(), s => s, StringComparer.InvariantCultureIgnoreCase);
			_priorityScripts = new List<SourceScript>();
			SetupPriorityScripts();
		}

		private void SetupPriorityScripts()
		{
			foreach (var priorityName in Configuration.PriorityScripts)
			{
				var script = this[priorityName];

				if(script == null)
					throw new InvalidOperationException("Script '" + priorityName + "' listed in the configuration as a priority script was not found.");

				_priorityScripts.Add(script);
			}
		}

		public IProjectConfig Configuration { get; private set; }
		public IReadOnlyList<SourceScript> AllScripts { get; private set; }
		public IReadOnlyList<SourceScript> PriorityScripts { get { return _priorityScripts; } } 
		
		public SourceScript this[string keyName]
		{
			get
			{
				SourceScript script;

				_scriptsLookup.TryGetValue(keyName, out script);

				return script;
			}
		}
		
		private readonly Dictionary<string, SourceScript> _scriptsLookup;
		private readonly List<SourceScript> _priorityScripts;
	}
}