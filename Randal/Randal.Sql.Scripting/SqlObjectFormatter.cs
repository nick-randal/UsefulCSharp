using System;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace Randal.Sql.Scripting
{
	public interface ISqlObjectFormatter
	{
		string Format(StoredProcedure sproc);
		string Format(View view);
		string Format(UserDefinedFunction udf);
	}

	public sealed class SqlObjectFormatter : ISqlObjectFormatter
	{
		public string Format(StoredProcedure sproc)
		{
			var header = sproc.TextHeader.Replace("create procedure", "alter procedure");
			var parameters = sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = ");

			return string.Format(SprocScript,
				sproc.Parent.Name,
				sproc.Name,
				header,
				sproc.TextBody,
				string.Join(", ", parameters)
			);
		}

		public string Format(View view)
		{
			throw new NotImplementedException();
		}

		public string Format(UserDefinedFunction udf)
		{
			throw new NotImplementedException();
		}

		private const string SprocScript =
			@"--:: catalog {0}

			--:: ignore
			use {0}

			--:: pre
			exec coreCreateProcedure '{1}'
			GO

			--:: main
			{2}
			{3}

			/*
				exec {1} {4}
			*/";
	}
}
