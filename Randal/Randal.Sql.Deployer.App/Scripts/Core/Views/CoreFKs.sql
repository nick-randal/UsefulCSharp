--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateQueryView 'CoreFKs',
'
select object_name(fk.parent_object_id) [tableName], fk.parent_object_id [table_id], fk.object_id [fk_id], fk.name [fkName], 
	fk.update_referential_action, fk.update_referential_action_desc collate SQL_Latin1_General_CP1_CI_AS [cascadingUpdate], 
	fk.delete_referential_action, fk.delete_referential_action_desc collate SQL_Latin1_General_CP1_CI_AS [cascadingDelete],
	object_name(fk.parent_object_id) [childTable], col_name(fk.parent_object_id, c.parent_column_id) [childColumn],
	object_name(fk.referenced_object_id) [parentTable], col_name(fk.referenced_object_id, c.referenced_column_id) [parentColumn], 
	c.constraint_column_id [columnOrder]
from sys.foreign_keys fk
join sys.foreign_key_columns c on c.constraint_object_id = fk.object_id
'


/*

exec corePrintTableColumns 'CoreFKs', NULL

select top 100 * from CoreFKs

SELECT *
FROM fn_helpcollations()

*/