using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Sql.Deployer.Configuration
{
	public interface IScriptDeployerConfig
	{
		string DatabaseLookup { get; }
	}

	public sealed class ScriptDeployerConfig : IScriptDeployerConfig
	{
		public string DatabaseLookup { get; set; }
	}

	public interface IProjectsTableConfig
	{
		string Database { get; }
		string CreateTable { get; }
		string Insert { get; }
		string Read { get; }
	}

	public sealed class ProjectsTableConfig : IProjectsTableConfig
	{
		public string Database { get; set; }

		public string CreateTable { get; set; }

		public string Insert { get; set; }

		public string Read { get; set; }
	}
}
