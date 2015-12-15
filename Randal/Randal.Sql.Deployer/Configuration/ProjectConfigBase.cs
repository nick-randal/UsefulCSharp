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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Randal.Sql.Deployer.Configuration
{
	[DataContract]
	public abstract class ProjectConfigBase : IProjectConfig
	{
		protected ProjectConfigBase(string project, string version)
		{
			_project = project ?? "Unknown";
			_version = version ?? "01.01.01.01";
		}

		[DataMember(IsRequired = true), XmlIgnore]
		public virtual string Project
		{
			get { return _project; } 
			set { _project = (value ?? string.Empty).Trim(); }
		}

		[DataMember(IsRequired = true), XmlIgnore]
		public virtual string Version
		{
			get { return _version; }
			set { _version = (value ?? string.Empty).Trim(); }
		}

		public abstract IReadOnlyList<string> PriorityScripts { get; }

		public abstract IReadOnlyDictionary<string, string> Vars { get; }

		public bool Validate(out IList<string> messages)
		{
			messages = (
				from k in Vars.Keys
				where ValidVarRegex.IsMatch(k) == false
				select "Vars has key '" + k + "' which contains invalid characters. Allowed: Aa-Zz 0-9 - _"
			).ToList();

			if (string.IsNullOrWhiteSpace(Project))
				messages.Add("A project name must be specified in the configuration.");

			if (VersionRegex.IsMatch(Version) == false)
				messages.Add("A valid version number in the format YY.MM.DD.II must be provided.");

			return messages.Count == 0;
		}

		protected static readonly Regex
			VersionRegex = new Regex(@"\d{2}(\.\d{2}){3}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture),
			ValidVarRegex = new Regex("^" + ValidVarPattern + "$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Multiline | RegexOptions.Singleline);

		private string _project, _version;

		internal const string ValidVarPattern = @"[\w\d_-]+";
	}
}