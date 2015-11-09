using System.Collections.Generic;

namespace Randal.Sql.Deployer.Scripts
{
	public interface IScriptCheckerConsumer
	{
		ScriptCheck Validate(string input, IList<string> messages);
	}
}