--:: catalog $Catalogs$

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreTableSizes',
'
with TableSizes as
(
	select 
		ps.object_id,
		row_count, 
		convert(numeric(38), ps.reserved_page_count	) * 0.0078125	table_reserved_mb,
		convert(numeric(38), ps.used_page_count		) * 0.0078125	table_used_mb
	from sys.dm_db_partition_stats ps
	where (ps.index_id = 0 or ps.index_id = 1)
),

IndexSizes as
(
	select 
		ps.object_id,
		sum(convert(numeric(38), ps.reserved_page_count	)) * 0.0078125	index_reserved_mb, 
		sum(convert(numeric(38), ps.used_page_count		)) * 0.0078125	index_used_mb
	from sys.dm_db_partition_stats ps
	where  (ps.index_id > 1) -- object_id = object_id(@table) and
	group by ps.object_id
)

select 
	object_name(t.object_id) [TableName], t.row_count [rows], 
	convert(numeric(9, 1), t.table_used_mb) table_used_mb, 
	convert(numeric(9, 1), isnull(ix.index_used_mb, 0.0)) index_used_mb,
	convert(numeric(9, 1), t.table_reserved_mb + isnull(ix.index_reserved_mb, 0.0)) reserved_mb,
	convert(numeric(9, 1), t.table_reserved_mb + isnull(ix.index_reserved_mb, 0.0) - t.table_used_mb - isnull(ix.index_used_mb, 0.0)) unused_mb
from TableSizes t
	join sys.tables st on t.object_id = st.object_id
	left join IndexSizes ix on t.object_id = ix.object_id
where st.type = ''u''
'


/*

helpme TableSizes

q1k TableSizes

*/

