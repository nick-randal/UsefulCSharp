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
using System.Runtime.Serialization;

namespace Randal.Sql.Deployer.Configuration
{
	[DataContract]
	public sealed class ProjectConfigJson : ProjectConfigBase
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public ProjectConfigJson() : this(null, null, null, null)
		{
		}

		/// <summary>
		/// Project with priority scripts and optional vars
		/// </summary>
		/// <param name="project">A project name that uniquely identifies the collection of scripts.</param>
		/// <param name="version">The current version of the project.</param>
		/// <param name="priorityScripts">A list of priority script file names (without extension) or NULL.</param>
		/// <param name="vars">A valid dictionary of replacement variable names and values or NULL.</param>
		public ProjectConfigJson(string project, string version, IEnumerable<string> priorityScripts, IDictionary<string, string> vars = null)
			:base(project, version)
		{
			_priorityScripts = priorityScripts == null ? new List<string>() : new List<string>(priorityScripts);
			_vars = vars == null 
				? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) 
				: new Dictionary<string, string>(vars, StringComparer.OrdinalIgnoreCase);
		}

		[DataMember(Name = "PriorityScripts")]
		private readonly List<string> _priorityScripts;

		[DataMember(Name = "Vars")]
		private Dictionary<string, string> _vars;

		[IgnoreDataMember]
		public override IReadOnlyList<string> PriorityScripts
		{
			get { return _priorityScripts; }
		}

		[IgnoreDataMember]
		public override IReadOnlyDictionary<string, string> Vars
		{
			get
			{
				if (!Equals(_vars.Comparer, StringComparer.OrdinalIgnoreCase))
					_vars = new Dictionary<string, string>(_vars, StringComparer.OrdinalIgnoreCase);
				return _vars;
			}
		}
	}
}