namespace Randal.Sql.Deployer.App
{
	public enum RunnerResolution
	{
		Committed		= -1,
		ValidationOnly	= -2,
		RolledBack		= -3,
		ExceptionThrown = -999
	}
}