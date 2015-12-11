// Useful C#
// Copyright (C) 2014-2016 Nicholas Randal
// 
// Useful C# is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Randal.Sql.Deployer.Scripts.Blocks
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
			if (IsValid == false)
				throw new InvalidOperationException("Cannot execute invalid script block.");

			if (IsExecuted)
				throw new InvalidOperationException("Cannot request execution for script block more than once.");

			IsExecuted = true;

			return _commandText;
		}

		private string _commandText;

		private static readonly Regex FilterGo = new Regex(@"^\s*go(\s+|$)",
			RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
	}
}