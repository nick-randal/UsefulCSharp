using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Smo;
using Randal.Core.Strings;

namespace Randal.Sql.Scripting
{
	public interface IScriptFormatter
	{
		string Format(ScriptSchemaObjectBase schemaObject);
		string Format(StoredProcedure sproc);
	}

	public sealed class ScriptFormatter : IScriptFormatter
	{
		public ScriptFormatter(IServer server)
		{
			_server = server;
		}

		public string Format(ScriptSchemaObjectBase schemaObject)
		{
			var sproc = schemaObject as StoredProcedure;
			var udf = schemaObject as UserDefinedFunction;
			var view = schemaObject as View;

			if (sproc != null)
				return Format(sproc);
			
			if (udf != null)
				return Format(udf);
			
			if (view != null)
				return Format(view);

			throw new InvalidOperationException("Not supported.");
		}

		public string Format(View view)
		{
			var values = new Dictionary<string, object>
			{
				{ "catalog", view.Parent.Name },
				{ "schema", view.Schema },
				{ "view", view.Name },
				{ "body", view.TextBody },
				{ "header", view.ScriptHeader(true) },
				{ "needs", GetNeeds(view) ?? string.Empty }
			};

			return ViewScript.Format().With(values); 
		}

		public string Format(UserDefinedFunction udf)
		{
			var values = new Dictionary<string, object>
			{
				{ "catalog", udf.Parent.Name },
				{ "schema", udf.Schema },
				{ "udf", udf.Name },
				{ "body", udf.TextBody },
				{ "header", udf.ScriptHeader(true) },
				{ "needs", GetNeeds(udf) ?? string.Empty }
			};

			switch (udf.FunctionType)
			{
				case UserDefinedFunctionType.Inline:
					values["funcType"] = "inline";
					break;
				case UserDefinedFunctionType.Table:
					values["funcType"] = "multi";
					break;
				default:
					values["funcType"] = "scalar";
					break;
			}

			return UserDefinedFunctionScript.Format().With(values); 
		}

		public string Format(StoredProcedure sproc)
		{
			var values = new Dictionary<string, object>
			{
				{ "catalog", sproc.Parent.Name },
				{ "schema", sproc.Schema },
				{ "sproc", sproc.Name },
				{ "body", NormalizeSprocBody(sproc.TextBody) },
				{ "header", sproc.ScriptHeader(true) },
				{ "needs", GetNeeds(sproc) ?? string.Empty }
			};

			values["parameters"] = string.Join(", ", sproc.Parameters.Cast<StoredProcedureParameter>().ToList().Select(p => p.Name + " = "));

			return SprocScript.Format().With(values); 
		}

		private static string NormalizeSprocBody(string body)
		{
			body = PatternLineEndings.Replace(body.Trim(), Environment.NewLine + '\t');
			if (body.StartsWith("begin", StringComparison.CurrentCultureIgnoreCase) == false)
				body = "begin" + Environment.NewLine + '\t' + body + Environment.NewLine + "end";

			return body;
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
		private static readonly Regex PatternLineEndings = new Regex(@"[\t ]*\r?\n", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

		private const string 
			SprocScript =
@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateProcedure '[{schema}].[{sproc}]'
GO

--:: main
{header}
{body}

/*
	exec [{schema}].[{sproc}] {parameters}
*/",
			UserDefinedFunctionScript =
@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateFunction '[{schema}].[{udf}]', '{funcType}'
GO

--:: main
{header}
{body}

/*
	select {schema}.{udf}()
*/",
			ViewScript =
@"{needs}--:: catalog {catalog}

--:: ignore
use {catalog}

--:: pre
exec coreCreateView '[{view}]', '[{schema}]'
GO

--:: main
{header}
{body}

/*
	select top 100 * from {view}
*/"
		;
	}
}
