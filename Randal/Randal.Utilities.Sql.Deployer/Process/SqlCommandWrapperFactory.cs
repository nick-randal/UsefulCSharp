using System.Data.SqlClient;

namespace Randal.Utilities.Sql.Deployer.Process
{
	public interface ISqlCommandWrapperFactory
	{
		ISqlCommandWrapper CreateCommand(SqlConnection connection, SqlTransaction transaction, string commandText, params object[] values);
	}

	public sealed class SqlCommandWrapperFactory : ISqlCommandWrapperFactory
	{
		public ISqlCommandWrapper CreateCommand(SqlConnection connection, SqlTransaction transaction, string commandText,
			params object[] values)
		{
			return new SqlCommandWrapper(connection, transaction, commandText, values);
		}
	}
}