--:: catalog $Catalogs$

--:: pre
IF NOT EXISTS (SELECT 1 FROM sys.procedures where name = 'coreCreateProcedure')
	EXEC('create procedure coreCreateProcedure as begin return 0 end')
GO

--:: main
alter procedure coreCreateProcedure
	@procName sysname,
	@schema sysname = null
as begin
	
	declare @schemaId int

	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)

	if not exists (select 1 from sys.procedures where name = @procName and schema_id = @schemaId) begin                
		exec('create procedure [' + @schema + '].[' +  @procName + '] as begin raiserror(''The body of stored procedure ' + @procName + ' has not been implemented!'', 11, 1) return 0 end')
	end

	return -1	
end

/*

exec coreCreateProcedure 'delete_me', 'test'

exec delete_me

exec coreDropProcedure 'delete_me'

*/