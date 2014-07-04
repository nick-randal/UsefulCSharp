using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using Randal.Core.Strings;

namespace Randal.Sql.Scripting
{
	public interface IScriptFormatter
	{
		string Format(StoredProcedure sproc);
	}

	public sealed class ScriptFormatter : IScriptFormatter
	{
		public ScriptFormatter(IServer server)
		{
			_server = server;
		}

		public string Format(StoredProcedure sproc)
		{
			var values = new Dictionary<string, object>
			{
				{"catalog", sproc.Parent.Name},
				{"sproc", sproc.Name},
				{"body", sproc.TextBody}
			};

			var temp = sproc.TextHeader.Replace("create procedure", "alter procedure");
			values.Add("header", temp);

			temp = string.Join(", ", sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = "));
			values.Add("parameters", temp);

			temp = GetNeeds(sproc);
			values.Add("needs", temp ?? string.Empty);

			return SprocScript.Format().With(values); 
		}

		private string GetNeeds(SqlSmoObject sproc)
		{
			var dependencies = _server.GetDependencies(sproc)
				.Where(x => x.Value != sproc.Urn && x.Type != "Table")
				.Select(x => x.GetNameForType(x.Type)).ToList();

			if (dependencies.Count == 0)
				return string.Empty;

			return "--:: need " + string.Join(", ", dependencies) + Environment.NewLine;
		}

		private readonly IServer _server;

		private const string 
			SprocScript =
@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateProcedure '{sproc}'
GO

--:: main
{header}
{body}

/*
	exec {sproc} {args}
*/",
			UserDefinedFunctionScript = 
@"",
			ViewScript = 
@""
		;
	}
}
