using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randal.Tests.Sql.Scripting.Support
{
	public static class ServerSetup
	{
		public static void Go()
		{
			var server = new Server(".");
			Database database;

			if (server.Databases.Contains("Test_Randal_Sql") == false)
			{
				database = new Database(server, "Test_Randal_Sql");
				database.Create();
			}
			else
				database = server.Databases["Test_Randal_Sql"];

			if(database.StoredProcedures.Contains("mySp") == false)
			{
				var sp = new StoredProcedure(database, "mySp");
				sp.TextMode = false;
				sp.AnsiNullsStatus = false;
				sp.QuotedIdentifierStatus = false;
				sp.TextBody = "return -1";
				sp.Create();
			}

			if (database.UserDefinedFunctions.Contains("myFunc") == false)
			{
				var func = new UserDefinedFunction(database, "myFunc");
				func.TextMode = false;
				func.ExecutionContext = ExecutionContext.Caller;
				func.FunctionType = UserDefinedFunctionType.Scalar;
				func.ImplementationType = ImplementationType.TransactSql;
				func.DataType = DataType.Int;
				func.TextBody = "begin return(-1); end";
				func.Create();
			}

			if(database.Views.Contains("myView") == false)
			{
				var view = new View(database, "myView");

				view.TextMode = false;
				view.AnsiNullsStatus = false;
				view.TextBody = "select 42 [AnswerToEverything]";
				view.Create();
			}
		}
	}
}
