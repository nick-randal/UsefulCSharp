--:: catalog $Catalogs$

--:: ignore
use model


--:: pre
exec coreCreateFunction 'coreGetViewsFor', 'dbo', 'table'
GO

--:: main
alter function coreGetViewsFor(@table sysname)

	returns @t table 
	( 
		viewName sysname
	)
	
begin  
	

	insert @t
		select view_name
		from information_schema.view_table_usage
		where table_name = @table
	
	return 
end 

/*

select * from dbo.coreGetViewsFor('agency_site')

*/