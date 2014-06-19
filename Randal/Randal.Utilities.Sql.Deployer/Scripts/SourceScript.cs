/*
Useful C#
Copyright (C) 2014  Nicholas Randal

Useful C# is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Randal.Utilities.Sql.Deployer.Scripts
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

				if (_name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
					_name = _name.Remove(_name.Length - 4);
			}
		}

		public IReadOnlyList<IScriptBlock> ScriptBlocks { get { return _scriptBlocks.AsReadOnly(); } }

		public bool IsValid { get; private set; }

		public IReadOnlyList<string> Validate()
		{
			var messages = new List<string>();
			foreach (var block in _scriptBlocks)
			{
				messages.AddRange(block.Parse());
			}

			if(HasBlockOfType<ConfigurationBlock>() == false)
				_scriptBlocks.Add(new ConfigurationBlock());

			if(HasBlockOfType<SqlCommandBlock>() && HasBlockOfType<CatalogBlock>() == false)
				messages.Add("Sql Text Blocks have been defined but no Catalog Block defined to specify which databases to execute against.");

			IsValid = messages.Count == 0;

			return messages;
		}

		private bool HasBlockOfType<TBlock>() 
			where TBlock : IScriptBlock
		{
			return _scriptBlocks.Exists(sb => sb is TBlock);
		}

		public bool HasSqlScriptPhase(SqlScriptPhase requestedPhase)
		{
			return RequestSqlScriptPhase(requestedPhase) != null;
		}

		public string RequestSqlScriptPhase(SqlScriptPhase requestedPhase)
		{
			var scriptBlock = (SqlCommandBlock)_scriptBlocks
				.FirstOrDefault(sb => sb is SqlCommandBlock && ((SqlCommandBlock)sb).Phase == requestedPhase);

			return scriptBlock == null ? null : scriptBlock.RequestForExecution();
		}

		public IReadOnlyList<string> GetCatalogPatterns()
		{
			var catalogBlock = (CatalogBlock)_scriptBlocks.FirstOrDefault(sb => sb is CatalogBlock && sb.IsValid);

			return catalogBlock == null ? null : catalogBlock.CatalogPatterns;
		}

		public IReadOnlyList<string> GetNeeds()
		{
			var needBlock = (NeedBlock) ScriptBlocks.FirstOrDefault(sb => sb is NeedBlock && sb.IsValid);

			return needBlock == null ? null : needBlock.Files;
		}

		public ConfigurationBlock GetConfiguration()
		{
			return (ConfigurationBlock) ScriptBlocks.FirstOrDefault(sb => sb is ConfigurationBlock && sb.IsValid);
		}

		private string _name;
	}
}