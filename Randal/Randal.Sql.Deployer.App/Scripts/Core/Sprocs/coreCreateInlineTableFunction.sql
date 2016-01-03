--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreCreateInlineTableFunction'
GO

--:: main
alter procedure coreCreateInlineTableFunction
    @funcName sysname

as begin

    if exists (select * from sys.objects where object_id = object_id(@funcName) AND type in (N'FN', N'TF') ) begin
        exec( 'drop function ' + @funcName )
	end

    if not exists (select * from sys.objects where object_id = object_id(@funcName) AND type = N'IF' ) begin
        exec('create function ' + @funcName + '() returns table as return ( select 0 x )')
	end

	return -1
end