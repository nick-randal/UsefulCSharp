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

namespace Randal.Core.IO.Logging
{
	public static class TextResources
	{
		public const string
			DashedBreak80Wide = "--------------------------------------------------------------------------------",
			Timestamp = "yyMMdd HHmmss",
			NoTimestamp = "             ",
			LeadIn = "    ",
			GroupLeadIn = "  >>>   ",
			GroupLeadOut = "  <<<   ",
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
				InsertProduct = @"INSERT INTO master.dbo.ProductsDeployed (Product, Package, Version, FromMachine, FromUser)" +
				                "VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
				CreateProductsTable =
					@"IF NOT EXISTS (select * from sys.tables where name = 'ProductsDeployed')
BEGIN
	CREATE TABLE ProductsDeployed (
		Product				VARCHAR(64)		NOT NULL,
		Package				VARCHAR(64)		NOT NULL,
		Version				VARCHAR(15)		NOT NULL,
		InstalledOn			DATETIME		NOT NULL DEFAULT GETDATE(),
		InstalledBy			SYSNAME			NOT NULL DEFAULT CURRENT_USER,
		FromMachine			SYSNAME			NOT NULL,
		FromUser			SYSNAME			NOT NULL,

		PRIMARY KEY CLUSTERED (Product, Package, Version)
	)

	ALTER TABLE [dbo].[ProductsDeployed]  WITH CHECK ADD  CONSTRAINT [CK_RpsProducts_Version] CHECK  (([Version] like '[0-9][0-9].[0-9][0-9].[0-9][0-9].[0-9]'))

END",
				ErrSqlConnection = "Failed to open a connection to SQL Server.",
				GetDatabases = "select [name] from master.sys.databases where [name] not in ('msdb', 'tempdb')",
				GetProductVersion = "SELECT MAX(Version) FROM master.dbo.ProductsDeployed WHERE Product = '{0}' AND Package = '{1}'",
				IncludeLogNote = "NOTE !!!    Please include the entire log file when reporting errors.",
				MsgPackageNew = "The package is more recent than the existing database version, proceeding...",
				MsgSqlOpen1 = "Opening a connection to sql server ({0}) using integrated security.",
				MsgSqlOpen2 = "Opening a connection to sql server ({0}) for user ({1}).",
				Timeout = "Timeout : Rollback is taking longer than the connection timeout.  This does not affect the rollback completing."
			;
		}
	}
}