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
using Newtonsoft.Json;

namespace Randal.Sql.Deployer.Configuration
{
	public interface IProjectConfig
	{
		IReadOnlyList<string> PriorityScripts { get; }
		string Version { get; }
		string Project { get; }
	}

	public sealed class ProjectConfig : IProjectConfig
	{
		public ProjectConfig() : this(null, null, null)
		{
		}

		public ProjectConfig(string project, string version, IEnumerable<string> priorityScripts)
		{
			Project = project ?? "Unknown";
			Version = version ?? "01.01.01.01";
			_priorityScripts = priorityScripts == null ? new List<string>() : new List<string>(priorityScripts);
		}

		public IReadOnlyList<string> PriorityScripts
		{
			get { return _priorityScripts; }
		}

		[JsonProperty(Required = Required.Default)]
		public string Version { get; private set; }

		[JsonProperty(Required = Required.Default)]
		public string Project { get; private set; }

		[JsonProperty(Required = Required.Default)] 
		private readonly List<string> _priorityScripts;
	}
}