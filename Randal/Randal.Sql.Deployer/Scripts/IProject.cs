using System.Collections.Generic;
using Randal.Sql.Deployer.Configuration;

namespace Randal.Sql.Deployer.Scripts
{
	public interface IProject
	{
		IProjectConfig Configuration { get; }
		IReadOnlyList<SourceScript> NonPriorityScripts { get; }
		IReadOnlyList<SourceScript> PriorityScripts { get; }

		SourceScript TryGetScript(string scriptName);
	}
}