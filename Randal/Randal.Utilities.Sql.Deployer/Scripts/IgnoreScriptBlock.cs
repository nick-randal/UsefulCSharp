namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public sealed class IgnoreScriptBlock : BaseScriptBlock
	{
		public IgnoreScriptBlock(string text) : base("ignore", text)
		{
			IsValid = true;
		}
	}
}
