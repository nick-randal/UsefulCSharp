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
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Randal.Sql.Deployer.Configuration
{
	[XmlRoot(ElementName = "ProjectConfig")]
	public sealed class ProjectConfigXml : ProjectConfigBase
	{
		public ProjectConfigXml() : base(null, null)
		{
			_priorityScripts = new List<string>();
			_vars = new List<Var>();
		}

		[XmlElement]
		public override string Project { get; set; }

		[XmlElement]
		public override string Version { get; set; }

		[XmlArray(ElementName = "PriorityScripts"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XmlArrayItem("Script")]
		public List<string> PriorityScriptsInternal
		{
			get { return _priorityScripts; }
			set { _priorityScripts = value; }
		}

		[XmlArray(ElementName = "Vars"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public List<Var> VarsInternal
		{
			get { return _vars; }
			set { _vars = value; }
		}

		[XmlIgnore]
		public override IReadOnlyList<string> PriorityScripts { get { return _priorityScripts; } }

		[XmlIgnore]
		public override IReadOnlyDictionary<string, string> Vars
		{
			get { return _vars.ToDictionary(v => v.Name, v => v.Value, StringComparer.OrdinalIgnoreCase); }
		}

		private List<string> _priorityScripts;
		private List<Var> _vars;
	}
}