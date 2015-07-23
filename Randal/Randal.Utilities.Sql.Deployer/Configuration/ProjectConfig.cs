// Useful C#
// Copyright (C) 2014-2015 Nicholas Randal
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
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Randal.Sql.Deployer.Configuration
{
	public interface IProjectConfig
	{
		IReadOnlyList<string> PriorityScripts { get; }
		string Version { get; }
		string Project { get; }
		IReadOnlyDictionary<string, string> Vars { get; }

		bool Validate(out IList<string> messages);
	}

	public sealed class ProjectConfig : IProjectConfig
	{
		public ProjectConfig() : this(null, null, null, null)
		{
		}

		public ProjectConfig(string project, string version, IEnumerable<string> priorityScripts, IDictionary<string, string> vars)
		{
			Project = project ?? "Unknown";
			Version = version ?? "01.01.01.01";
			_priorityScripts = priorityScripts == null ? new List<string>() : new List<string>(priorityScripts);
			_vars = vars == null 
				? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) 
				: new Dictionary<string, string>(vars, StringComparer.OrdinalIgnoreCase);
		}

		[JsonIgnore]
		public IReadOnlyList<string> PriorityScripts
		{
			get { return _priorityScripts; }
		}

		[JsonIgnore]
		public IReadOnlyDictionary<string, string> Vars
		{
			get
			{
				if (_vars.Comparer != StringComparer.OrdinalIgnoreCase)
					_vars = new Dictionary<string, string>(_vars, StringComparer.OrdinalIgnoreCase);
				return _vars;
			}
		}

		[JsonProperty(Required = Required.Always)]
		public string Version { get; private set; }

		[JsonProperty(Required = Required.Always)]
		public string Project { get; private set; }

		[JsonProperty(PropertyName = "Vars")] 
		private Dictionary<string, string> _vars;

		[JsonProperty(PropertyName = "PriorityScripts")] 
		private readonly List<string> _priorityScripts;

		public bool Validate(out IList<string> messages)
		{
			messages = (
				from k in _vars.Keys 
				where ValidVarRegex.IsMatch(k) == false
				select "Vars has key '" + k + "' which contains invalid characters. Allowed: Aa-Zz 0-9 - _"
			).ToList();

			if(string.IsNullOrWhiteSpace(Project))
				messages.Add("A project name must be specified in the configuration.");

			if(VersionRegex.IsMatch(Version) == false)
				messages.Add("A valid version number in the format xx.xx.xx.xx must be provided.");

			return messages.Count == 0;
		}

		private static readonly Regex 
			VersionRegex = new Regex(@"\d{2}(\.\d{2}){3}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture),
			ValidVarRegex = new Regex(ValidVarPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline | RegexOptions.Singleline);

		internal const string ValidVarPattern = @"[\w\d_-]+";
	}
}