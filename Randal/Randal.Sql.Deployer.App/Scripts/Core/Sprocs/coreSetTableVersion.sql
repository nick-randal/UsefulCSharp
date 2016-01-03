--:: need coreSetParameter, coreGetTableVersion
--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreSetTableVersion'
GO

--:: main
alter procedure coreSetTableVersion
	@table			sysname,
	@version		char(11),
	@override		bit = 0

as begin
	declare @current char(11)

	IF(@version NOT LIKE '[0-9][0-9].[0-9][0-9].[0-9][0-9].[0-9][0-9]') begin
		raiserror('coreSetTableVersion: Invalid version %s specified for table %s', 16, 1, @version, @table)
		return 0
	end
	
	set @current = dbo.coreGetTableVersion(@table)

	if(@override = 1 or @current is null or @current < @version)
		exec coreSetParameter 'TableVersion', @table, @version

	return -1
end
GO

/*

-- bad value
exec coreSetTableVersion 'Test', '09.08.26.01', 1

select dbo.coreGetTableVersion('Test')

select * from CoreParameters

*/
