--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreDropProcedure'
GO

--:: main
alter procedure coreDropProcedure
	@Procedure		SYSNAME,
	@schema			SYSNAME = null,
	@PermsTo		SYSNAME = null
as
	DECLARE @cmd VARCHAR(1000)

	declare @schemaId int

	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)

	-- Drop stored procedure if it already exists
	IF EXISTS ( SELECT * FROM sys.procedures WHERE [name] = @Procedure AND schema_id = @schemaId AND [type] = 'P') BEGIN
		
		IF @PermsTo != null BEGIN
			
			-- Transfer Role-based permissions
			SELECT 
			N'coreGrantPermissions '''
				+CAST(dp.name as nvarchar) COLLATE DATABASE_DEFAULT+N''', '''
				+CAST(state_desc AS nvarchar)+N''', '''
				+CAST(permission_name AS nvarchar)+N''', '''
				+CAST(@PermsTo AS nvarchar)+N''''
			FROM sys.database_permissions p
			INNER JOIN sys.all_objects o ON p.major_id = o.[object_id]
			INNER JOIN sys.database_principals dp ON p.grantee_principal_id = dp.principal_id AND o.[name] = @Procedure
			
			EXEC(@cmd)
		
		END
	
		SET @cmd = 'DROP PROCEDURE ' + @schema + '.'  + @Procedure + ';'
		PRINT 'Drop Procedure : ' + @schema + '.' + @Procedure
		EXEC(@cmd)
	END ELSE BEGIN
		PRINT 'Procedure ' + @schema + '.'  + @Procedure + ' does not exist.'
	END
GO

--SELECT * FROM sys.procedures