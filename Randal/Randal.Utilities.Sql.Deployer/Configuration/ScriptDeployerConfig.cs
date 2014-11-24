// Useful C#
// Copyright (C) 2014 Nicholas Randal
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

using Newtonsoft.Json;

namespace Randal.Sql.Deployer.Configuration
{
	public interface IScriptDeployerConfig
	{
		string DatabaseLookup { get; }

		ProjectsTableConfig ProjectsTableConfig { get; }
	}

	public sealed class ScriptDeployerConfig : IScriptDeployerConfig
	{
		[JsonProperty(Required = Required.Default)]
		public string DatabaseLookup { get; set; }

		[JsonProperty(Required = Required.Default)]
		public ProjectsTableConfig ProjectsTableConfig { get; set; }

		public static readonly IScriptDeployerConfig Default = new ScriptDeployerConfig
		{
			DatabaseLookup = @"select [name] from master.sys.databases where [name] not in ('master', 'msdb', 'tempdb')",
			ProjectsTableConfig = new ProjectsTableConfig
			{
				Database = "master",
				CreateTable = @"IF NOT EXISTS (select * from sys.tables where name = 'ProjectsDeployed')
BEGIN
	CREATE TABLE ProjectsDeployed (
		Project				VARCHAR(128)	NOT NULL,
		Version				VARCHAR(15)		NOT NULL,
		FromMachine			SYSNAME			NOT NULL,
		FromUser			SYSNAME			NOT NULL,
		InstalledOn			DATETIME		NOT NULL DEFAULT GETDATE(),
		InstalledBy			SYSNAME			NOT NULL DEFAULT SUSER_NAME(),
		PRIMARY KEY CLUSTERED (Project, Version)
	)

	ALTER TABLE [dbo].[ProjectsDeployed]  WITH CHECK ADD  CONSTRAINT [CK_ProjectsDeployed_Version] CHECK  (([Version] like '[0-9][0-9].[0-9][0-9].[0-9][0-9].[0-9][0-9]'))

END",
				Insert = @"INSERT ProjectsDeployed (Project, Version, FromMachine, FromUser) VALUES ('{0}', '{1}', '{2}', '{3}')",
				Read = @"SELECT MAX(Version) FROM ProjectsDeployed WHERE Project = '{0}'"
			}
		};
	}

	public interface IProjectsTableConfig
	{
		string Database { get; }
		string CreateTable { get; }
		string Insert { get; }
		string Read { get; }
	}

	public sealed class ProjectsTableConfig : IProjectsTableConfig
	{
		[JsonProperty(Required = Required.Default)]
		public string Database { get; set; }

		[JsonProperty(Required = Required.Default)]
		public string CreateTable { get; set; }

		[JsonProperty(Required = Required.Default)]
		public string Insert { get; set; }

		[JsonProperty(Required = Required.Default)]
		public string Read { get; set; }
	}

	
}
