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

		[XmlIgnore]
		public override IReadOnlyList<string> PriorityScripts { get { return _priorityScripts; } }

		[XmlArray(ElementName = "PriorityScripts"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[XmlArrayItem("Script")]
		public List<string> PriorityScriptsInternal
		{
			get { return _priorityScripts; }
			set { _priorityScripts = value; }
		}

		[XmlElement]
		public override string Version { get; set; }

		[XmlElement]
		public override string Project { get; set; }

		[XmlIgnore]
		public override IReadOnlyDictionary<string, string> Vars
		{
			get { return _vars.ToDictionary(v => v.Name, v => v.Value, StringComparer.OrdinalIgnoreCase); }
		}

		[XmlArray(ElementName = "Vars"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public List<Var> VarsInternal
		{
			get { return _vars; }
			set { _vars = value; }
		}

		private List<string> _priorityScripts;
		private List<Var> _vars;
	}
}