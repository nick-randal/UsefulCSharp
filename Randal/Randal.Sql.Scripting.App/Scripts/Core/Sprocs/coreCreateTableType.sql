--:: catalog $Catalogs$
--:: need coreTrimBrackets

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'coreCreateTableType'
go

--:: main
alter procedure coreCreateTableType
    @typeName		sysname,
	@schema			sysname,
	@definition		varchar(max),
	@debug			bit = 0
as
begin
	declare @schemaId int
	declare @cmd varchar(max)
		
	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)

	set @typeName = dbo.coreTrimBrackets(@typeName)	

	if exists (select 1 from sys.types where name = @typeName and schema_id = @schemaId and is_table_type = 1) begin
		return -1;
	end

	set @cmd = 'create type [' + @schema + '].[' + @typeName + '] as table (' + @definition + ');'
		
	if(@debug = 1) begin
		print @cmd
	end else begin
		exec(@cmd)
	end
	
	return -1	
end
go