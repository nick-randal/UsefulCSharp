--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'coreDropView'
GO

--:: main
alter procedure coreDropView
	@viewName		sysname
as
begin

	declare @cmd varchar(max)

	set @viewName = dbo.coreTrimBrackets(@viewName)

	set @cmd = 'if exists (select 1 from sys.views where name = ''' + @viewName + ''') drop view [' + @viewName + ']'

	exec (@cmd)

	return -1
end

/*

exec coreDropView 

*/