using System;
using Randal.Core.Enums;
using Randal.Sql.Deployer.Configuration;

namespace Randal.Sql.Deployer.Process
{
	public interface IScriptDeployer : IDisposable
	{
		bool CanUpgrade();
		Returned DeployScripts();
		IScriptDeployerConfig DeployerConfig { get; }
	}
}