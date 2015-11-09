namespace Randal.Sql.Deployer.Scripts
{
	public interface IScriptChecker : IScriptCheckerConsumer
	{
		void AddValidationPattern(string pattern, ScriptCheck shouldIssue);
	}
}