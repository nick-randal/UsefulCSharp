--:: catalog $Catalogs$
--:: need coreTrimBrackets

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'coreDropTableType'
go

--:: main
alter procedure coreDropTableType
    @typeName sysname,
	@schema sysname = null,
	@debug bit = 0
as
begin
	declare @schemaId int
	declare @cmd varchar(max)
		
	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)

	set @typeName = dbo.coreTrimBrackets(@typeName)	

	if exists (select 1 from sys.types where name = @typeName and schema_id = @schemaId and is_table_type = 1)
	begin
		set @cmd = 'drop type [' + @schema + '].[' + @typeName + '];'
		
		if(@debug = 1) begin
			print @cmd
		end else begin
			exec(@cmd)
		end
	end
	
	return -1	
end
go
