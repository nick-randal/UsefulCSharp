--:: need coreTrimBrackets
--:: catalog $Catalogs$

--:: pre
exec coreCreateFunction 'coreTableExists', 'dbo', 'scalar'
GO
--:: main
alter function coreTableExists(@table sysname, @schema sysname)
	returns BIT
AS
BEGIN
    
	if exists(select * from sys.tables where [name] = dbo.coreTrimBrackets(@table) and schema_name(schema_id) = dbo.coreTrimBrackets(@schema))
		return 1
	
	return 0
end

go

/*

SELECT dbo.coreTableExists('Stores', 'dbo')
SELECT dbo.coreTableExists('Companies', 'dbo')

*/