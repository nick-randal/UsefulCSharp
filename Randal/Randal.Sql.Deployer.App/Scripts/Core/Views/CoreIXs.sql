--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateQueryView 'CoreIXs',
'
select i.object_id [table_id], i.index_id, object_name(i.object_id) [tableName], i.name [indexName], i.is_primary_key [isPrimaryKey], 
	i.type [clusteringType], lower(i.type_desc) [clusteringText], i.is_unique [isUnique], 
	case i.is_unique when 1 then ''unique'' else '''' end [uniqueText],
	col_name(i.object_id, c.column_id) [columnName], c.is_descending_key [isDescending], 
	case c.is_descending_key
	when 1 then ''desc''
	else '''' end [descendingText], c.key_ordinal [ordinal], c.is_included_column [isIncluded], i.has_filter, isnull(i.filter_definition, '''') filter
from sys.indexes i
join sys.index_columns c 
	on c.object_id = i.object_id and i.index_id = c.index_id
'


/*

exec coreDropView 'CoreIXs'

exec corePrintTableColumns '', NULL

select top 100 * from CoreIXs where isPrimaryKey = 1

*/