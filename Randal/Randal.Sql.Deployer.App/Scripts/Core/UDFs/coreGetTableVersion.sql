--:: need coreGetParameterString
--:: catalog $Catalogs$

--:: pre
exec coreCreateFunction 'coreGetTableVersion', 'dbo', 'scalar'
GO

--:: main
alter function coreGetTableVersion(@table sysname)
	returns char(11)

as begin
    return isnull(dbo.coreGetParameterString('TableVersion', @table), '')
end


/*

SELECT dbo.coreGetTableVersion('Cities')

*/