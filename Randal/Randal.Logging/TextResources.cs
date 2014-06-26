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

namespace Randal.Logging
{
	public static class TextResources
	{
		public const string
			DashedBreak80Wide = "--------------------------------------------------------------------------------",
			Timestamp = "yyMMdd HHmmss",
			NoTimestamp = "                 ",
			Prepend = "    ",
			AttentionRepeatingLines = "ATTENTION: The previous line was repeated {0} times.",
			ApplicationEventLog = "Application"
			;

		public static class Errors
		{
			public const string
				ErrorInfo = "Error Info",
				NoExceptionProvided = "An error occurred but no instance of Exception provided."
				;
		}

		public static class Sql
		{
			public static class Database
			{
				public const string
					Master = "master",
					TempDb = "tempdb"
					;
			}

			public const string
				InsertProduct = @"INSERT ProjectsDeployed (Project, Version, FromMachine, FromUser)" +
				                "VALUES ('{0}', '{1}', '{2}', '{3}')",
				CreateProductsTable =
					@"IF NOT EXISTS (select * from sys.tables where name = 'ProjectsDeployed')
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
				GetDatabases = "select [name] from master.sys.databases where [name] not in ('msdb', 'tempdb')",
				GetProductVersion = "SELECT MAX(Version) FROM master.dbo.ProjectsDeployed WHERE Project = '{0}'"
				;
		}
	}
}