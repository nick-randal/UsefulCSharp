using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Randal.Sql.Deployer.Configuration
{
	[DataContract]
	public sealed class ProjectConfigJson : ProjectConfigBase
	{
		public ProjectConfigJson() : this(null, null, null, null)
		{
		}

		public ProjectConfigJson(string project, string version, IEnumerable<string> priorityScripts, IDictionary<string, string> vars)
			:base(project, version)
		{
			_priorityScripts = priorityScripts == null ? new List<string>() : new List<string>(priorityScripts);
			_vars = vars == null 
				? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) 
				: new Dictionary<string, string>(vars, StringComparer.OrdinalIgnoreCase);
		}

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

		[DataMember(Name = "Vars")] 
		private Dictionary<string, string> _vars;

		[DataMember(Name = "PriorityScripts")] 
		private readonly List<string> _priorityScripts;
	}
}