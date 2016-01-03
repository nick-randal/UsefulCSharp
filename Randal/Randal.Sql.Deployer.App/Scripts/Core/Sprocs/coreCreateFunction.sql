--:: need coreCreateProcedure, coreCreateInlineTableFunction, coreCreateMultiStatementFunction, coreCreateScalarFunction
--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreCreateFunction'
GO

--:: main
alter procedure coreCreateFunction
	@funcName	sysname,
	@schema		sysname,
	@funcType	varchar(1000)

as begin

	declare @cmd varchar(max), @schemaId int

	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)


	set @funcName = @schema + '.' + @funcName
	if(object_id(@funcName) is not null) begin
		return 0
	end

	if(@funcType = 'inline') begin
		exec coreCreateInlineTableFunction @funcName
		return -1
	end
	
	if(@funcType = 'table' or @funcType = 'multi') begin
		exec coreCreateMultiStatementFunction @funcName
		return -1
	end
		
	if(@funcType = 'scalar') begin
		exec coreCreateScalarFunction @funcName
		return -1;
	end

	raiserror('Unexpected return type ''%s'' provided for function ''%s'' ', 17, 1, @funcType, @funcName)

	return -1
end
/*

exec coreCreateFunction 'test', 'scalar'

exec coreDropFunction 'test'

exec coreCreateFunction 'boom', 'salad shooter'

*/