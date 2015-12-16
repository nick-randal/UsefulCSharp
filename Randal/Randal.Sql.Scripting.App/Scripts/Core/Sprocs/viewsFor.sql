--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'viewsFor'
GO

--:: main
alter procedure viewsFor
	@tableName		sysname

as begin

	select 
		viewName 
	from dbo.coreGetViewsFor(@tableName)

	return -1
end

/*

exec viewsFor 'Lead'
exec viewsFor 'Agency'

*/