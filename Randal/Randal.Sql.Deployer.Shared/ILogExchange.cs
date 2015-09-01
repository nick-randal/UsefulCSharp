using System.ServiceModel;


namespace Randal.Sql.Deployer.Shared
{
	[ServiceContract]
	public interface ILogExchange
	{
		[OperationContract]
		void ReportLogFilePath(string logFilePath);
	}
}
