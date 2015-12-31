--:: catalog $Catalogs$
--:: need coreTrimBrackets

--:: pre
exec coreCreateProcedure 'coreRenameTableType'
go

--:: main
alter procedure coreRenameTableType
    @typeName sysname,
	@schema sysname = null,
	@newName sysname
as
begin
	declare @schemaId int
	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)

	set @typeName = dbo.coreTrimBrackets(@typeName)
	
	declare @oldName sysname = '[' + @schema + '].[' + @typeName + ']'
	
	if exists (select 1 from sys.types where name = @typeName and schema_id = @schemaId and is_table_type = 1)
	begin
		exec sys.sp_rename @oldName, @newName
	end

	return -1	
end
go
