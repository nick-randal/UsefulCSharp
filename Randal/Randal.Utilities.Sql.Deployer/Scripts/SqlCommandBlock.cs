using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Randal.Utilities.Sql.Deployer.Scripts
{
	public interface ISqlCommandBlock
	{
		string RequestForExecution();
		bool IsExecuted { get; }
		SqlScriptPhase Phase { get; }
	}

	public sealed class SqlCommandBlock : BaseScriptBlock, ISqlCommandBlock
	{
		public SqlCommandBlock(string keyword, string text, SqlScriptPhase phase) : base(keyword, text)
		{
			_commandText = string.Empty;
			Phase = phase;
			IsExecuted = false;
		}

		public bool IsExecuted { get; private set; }

		public SqlScriptPhase Phase { get; private set; }

		public override IReadOnlyList<string> Parse()
		{
			var sql = FilterGo.Replace(Text, string.Empty);

			_commandText = sql.Trim();
			IsValid = true;

			return new List<string>();
		}

		public string RequestForExecution()
		{
			if(IsValid == false)
				throw new InvalidOperationException("Cannot execute invalid script block.");

			if(IsExecuted)
				throw new InvalidOperationException("Cannot request execution for script block more than once.");

			IsExecuted = true;

			return _commandText;
		}

		private string _commandText;
		private static readonly Regex FilterGo = new Regex(@"^\s*go(\s+|$)",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
	}
}
