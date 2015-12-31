--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'coreSpaceDetails'
GO

--:: main
alter procedure coreSpaceDetails
	@tableName			sysname

as begin

	declare @db_id smallint;
	declare @object_id int;
	
	set @db_id = DB_ID();
	set @object_id = OBJECT_ID(@tableName);
	
	select i.name, index_type_desc, alloc_unit_type_desc, 
		   page_count, avg_page_space_used_in_percent, record_count, 
		   min_record_size_in_bytes, max_record_size_in_bytes, avg_record_size_in_bytes
	from sys.dm_db_index_physical_stats(@db_id, @object_id, NULL, NULL , 'Detailed') ps
	left join sys.indexes i on ps.object_id = i.object_id and ps.index_id = i.index_id;
	
	return -1
end

/*

exec coreSpaceDetails 'agency'

exec coreDropProcedure 'coreSpaceDetails'

*/

