--:: need coreCreateProcedure
--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreCreateScalarFunction'
GO

--:: main
alter procedure coreCreateScalarFunction
	@funcName sysname

as begin
	
	if exists (select 1 from sys.objects where object_id = object_id(@funcName) AND type in (N'IF', N'TF') ) begin
		exec( 'drop function ' + @funcName )
	end

	if not exists (select 1 from sys.objects where object_id = object_id(@funcName) AND type = N'FN' ) begin
		exec('create function ' + @funcName + '() returns bit as begin return 0 end')
	end

	return -1
end

/*

EXEC coreCreateScalarFunction 'testScalarFunc', 'int'

exec coreCreateScalarFunction 'test'

*/