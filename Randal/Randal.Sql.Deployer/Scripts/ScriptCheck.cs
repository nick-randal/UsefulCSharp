using System;

namespace Randal.Sql.Deployer.Scripts
{
	[Flags]
	public enum ScriptCheck
	{
		Passed = 0,
		Warning = 1,
		Failed = 2,
		Fatal = 4
	}
}