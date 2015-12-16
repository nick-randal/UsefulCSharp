--:: catalog $Catalogs$

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreComputedColumns',
'
with P as
(
	select 1 id, ''persisted'' s
)
, NN as
(
	select 1 id, ''not null'' s
)
select 
	name, 
	''alter table [dbo].['' + object_name(object_id) + ''] add ['' 
	+ name + ''] as '' + [definition] + '' '' + isnull(p.s, '''') + '' '' + isnull(n.s, '''') [definition], ''Computed Column: CREATE'' [description], 101  [phase]
from sys.computed_columns cc
left join P p on cc.is_persisted = p.id
left join NN n on cc.is_nullable = n.id

union all

select 
	name, 
	''alter table [dbo].['' + object_name(object_id) + ''] drop column ['' + name + '']'', ''Computed Column: DROP'', 11
from sys.computed_columns
'


/*

select * from CoreComputedColumns order by phase

*/