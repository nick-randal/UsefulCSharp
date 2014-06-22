using Randal.Core.Testing.UnitTest;
using Randal.Utilities.Sql.Deployer.Configuration;
using Randal.Utilities.Sql.Deployer.Scripts;
using System.Collections.Generic;

namespace Randal.Tests.Utilities.Sql.Deployer.Scripts
{
	public sealed class ProjectBuilder : ITestObjectBuilder<Project>
	{
		public ProjectBuilder()
		{
			_scripts = new List<SourceScript>();
		}
		
		public ProjectBuilder WithConfiguration(string project, string version, params string[] priorityScripts)
		{
			_config = new ProjectConfig(project, version, priorityScripts);
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

	public sealed class ScriptBuilder : ITestObjectBuilder<SourceScript>
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

		public SourceScript Build()
		{
			var script = new SourceScript(_name, _blocks);
			script.Validate();
			return script;
		}

		public static explicit operator SourceScript(ScriptBuilder builder)
		{
			return builder.Build();
		}

		private readonly string _name;
		private readonly List<IScriptBlock> _blocks;
	}
}
