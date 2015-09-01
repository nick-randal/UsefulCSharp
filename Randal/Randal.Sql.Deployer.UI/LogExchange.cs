using Randal.Sql.Deployer.Shared;

namespace Randal.Sql.Deployer.UI
{
	public sealed class LogExchange : ILogExchange
	{
		public static string LogFilePath;

		public void ReportLogFilePath(string logFilePath)
		{
			LogFilePath = logFilePath;
		}
	}
}
