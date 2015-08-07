using System.Collections.Generic;

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
}