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
using System.Linq;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Sql.Deployer.Scripts
{
	public sealed class SourceScript
	{
		private readonly List<IScriptBlock> _scriptBlocks;

		public SourceScript(string name, IEnumerable<IScriptBlock> blocks)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			IsValid = false;
			Name = name;
			_scriptBlocks = new List<IScriptBlock>(blocks);
		}

		public string Name
		{
			get { return _name; }

			private set
			{
				_name = value.Trim();

				if (_name.EndsWith(ScriptConstants.SqlExtension, StringComparison.OrdinalIgnoreCase))
					_name = _name.Remove(_name.Length - 4);
			}
		}

		public IReadOnlyList<IScriptBlock> ScriptBlocks
		{
			get { return _scriptBlocks.AsReadOnly(); }
		}

		public bool IsValid { get; private set; }

		public bool Validate(IList<string> messages)
		{
			foreach (var msg in _scriptBlocks.SelectMany(block => block.Parse()))
			{
				messages.Add(msg);
			}

			if (HasBlockOfType<OptionsBlock>() == false)
				_scriptBlocks.Add(new OptionsBlock());

			if (HasBlockOfType<SqlCommandBlock>() && HasBlockOfType<CatalogBlock>() == false)
				messages.Add(
					"Sql Text Blocks have been defined but no Catalog Block defined to specify which databases to execute against.");

			return IsValid = messages.Count == 0;
		}

		public bool HasSqlScriptPhase(SqlScriptPhase requestedPhase)
		{
			return GetSqlCommandBlock(requestedPhase) != null;
		}

		public bool HasPhaseExecuted(SqlScriptPhase requestedPhase)
		{
			var scriptBlock = GetSqlCommandBlock(requestedPhase);

			return scriptBlock.IsExecuted;
		}

		public string RequestSqlScriptPhase(SqlScriptPhase requestedPhase)
		{
			var scriptBlock = GetSqlCommandBlock(requestedPhase);

			if (scriptBlock == null)
				throw new InvalidOperationException("No script block available for requested phase " + requestedPhase);

			return scriptBlock.RequestForExecution();
		}

		public IReadOnlyList<string> GetCatalogPatterns()
		{
			var catalogBlock = (CatalogBlock) _scriptBlocks.FirstOrDefault(sb => sb is CatalogBlock && sb.IsValid);

			var catalogs = new List<string>();
			if (catalogBlock != null)
				catalogs.AddRange(catalogBlock.CatalogPatterns);

			return catalogs.AsReadOnly();
		}

		public IReadOnlyList<string> GetNeeds()
		{
			var needBlock = (NeedBlock) ScriptBlocks.FirstOrDefault(sb => sb is NeedBlock && sb.IsValid);

			var needed = new List<string>();
			if (needBlock != null)
				needed.AddRange(needBlock.Files);

			return needed.AsReadOnly();
		}

		public OptionsBlock GetConfiguration()
		{
			return (OptionsBlock) ScriptBlocks.FirstOrDefault(sb => sb is OptionsBlock && sb.IsValid);
		}

		private SqlCommandBlock GetSqlCommandBlock(SqlScriptPhase requestedPhase)
		{
			return (SqlCommandBlock) _scriptBlocks
				.FirstOrDefault(sb => sb is SqlCommandBlock && ((SqlCommandBlock) sb).Phase == requestedPhase);
		}

		private bool HasBlockOfType<TBlock>()
			where TBlock : IScriptBlock
		{
			return _scriptBlocks.Exists(sb => sb is TBlock);
		}

		private string _name;
	}
}