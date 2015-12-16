--:: catalog $Catalogs$
--:: need coreTrimBrackets

--:: pre
exec coreCreateProcedure 'coreRefreshModulesUsedByTableType'
go

--:: main
alter procedure coreRefreshModulesUsedByTableType
    @typeName sysname,
	@schema sysname = null,
	@debug bit = 0
as
begin
	declare @Name sysname
	
	set @schema = isnull(@schema, 'dbo')
	
	set @typeName = dbo.coreTrimBrackets(@typeName)

	declare REF_CURSOR cursor for
		select referencing_schema_name + '.' + referencing_entity_name
		from sys.dm_sql_referencing_entities('[' + @schema + '].[' + @typeName + ']', 'TYPE')

	open REF_CURSOR

	fetch next from REF_CURSOR into @Name

	while (@@FETCH_STATUS = 0)
	begin
		if(@debug = 1) begin
			print @Name + ' references ' + @typeName
		end else begin
			exec sys.sp_refreshsqlmodule @name = @Name
		end
		
		fetch next from REF_CURSOR into @Name
	end

	close REF_CURSOR
	deallocate REF_CURSOR
	
	return -1	
end
go
