/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

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
