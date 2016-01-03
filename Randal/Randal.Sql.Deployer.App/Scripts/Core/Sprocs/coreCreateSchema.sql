--:: need coreTrimBrackets
--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'coreCreateSchema'
GO

--:: main
alter procedure coreCreateSchema
	@schemaName		sysname = 'dbo',
	@debug			bit = 0
as
	DECLARE @cmd varchar(max)

	IF EXISTS (SELECT 1 FROM sys.schemas WHERE name = @schemaName) begin
		return;
	end
		
	set @schemaName = dbo.coreTrimBrackets(@schemaName)
	SET @cmd = 'CREATE SCHEMA [' + @schemaName + ']'
	
	if(@debug = 1) begin
		print @cmd
	end else begin
		exec(@cmd)
	end
GO

/*

exec coreCreateSchema 'Test', 1

select * from sys.schemas

*/