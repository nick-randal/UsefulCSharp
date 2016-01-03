--:: need coreCreateProcedure
--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreCreateMultiStatementFunction'
GO

--:: main
alter procedure coreCreateMultiStatementFunction
    @funcName sysname

as begin
	
    if exists (select * from sys.objects where object_id = object_id(@funcName) AND type in (N'FN', N'IF') ) begin
        exec( 'drop function ' + @funcName )
	end

    if not exists (select * from sys.objects where object_id = object_id(@funcName) AND type = N'TF' ) begin
        exec('create function ' + @funcName + '() RETURNS @t table (x int null) as begin insert @t values(1) return end')
	end

	return -1
end

/*

EXEC coreCreateMultiStatementFunction 'testScalarFunc'

*/