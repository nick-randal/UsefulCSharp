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

using System.Collections.Generic;
using Randal.Core.Testing.UnitTest;
using Randal.Sql.Deployer.Configuration;
using Randal.Sql.Deployer.Scripts;
using Randal.Sql.Deployer.Scripts.Blocks;

namespace Randal.Tests.Sql.Deployer.Scripts
{
	public sealed class ProjectBuilder : ITestObjectBuilder<Project>
	{
		public ProjectBuilder()
		{
			_scripts = new List<SourceScript>();
		}

		public ProjectBuilder WithConfiguration(string project, string version, params string[] priorityScripts)
		{
			_config = new ProjectConfigJson(project, version, priorityScripts, null);
			return this;
		}

		public ProjectBuilder WithScript(SourceScript script)
		{
			_scripts.Add(script);
			return this;
		}

		public Project Build()
		{
			var project = new Project(_config, _scripts);
			return project;
		}

		public static explicit operator Project(ProjectBuilder builder)
		{
			return builder.Build();
		}

		private IProjectConfig _config;
		private readonly List<SourceScript> _scripts;
	}

	public sealed class ScriptBuilder
	{
		public ScriptBuilder(string name)
		{
			_name = name;
			_blocks = new List<IScriptBlock>();
		}

		public ScriptBuilder WithCatalogs(string catalogs)
		{
			var block = new CatalogBlock(catalogs);
			block.Parse();
			_blocks.Add(block);
			return this;
		}

		public ScriptBuilder WithMainBlock(string sql)
		{
			_blocks.Add(new SqlCommandBlock("main", sql, SqlScriptPhase.Main));
			return this;
		}

		public SourceScript Build(IList<string> validationMessages)
		{
			var script = new SourceScript(_name, _blocks);
			script.Validate(validationMessages);
			return script;
		}

		private readonly string _name;
		private readonly List<IScriptBlock> _blocks;
	}
}