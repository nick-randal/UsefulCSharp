--:: catalog $Catalogs$
--:: need coreIxType, coreIxColumns, coreIxXmlSecondary

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreIndexes',
'
select
	i.name, 
	''ALTER TABLE ['' + object_name(i.object_id)  + ''] DROP CONSTRAINT ['' + i.name + '']'' [definition], 
	case
	when is_primary_key = 1 then ''Index - Primary Key: DROP''
	else ''Index - Unique Constraint: DROP''
	end [description],
	case
	when is_primary_key = 1 then 9
	else 5
	end [phase]
from sys.indexes i
	left join sys.tables t on i.object_id = t.object_id
	left join sys.views v on i.object_id = v.object_id
	
where
	i.type in (1, 2) and (i.is_primary_key = 1 or i.is_unique_constraint = 1)
	and
	(t.object_id is not null and t.type = ''U'')
	or
	(v.object_id is not null and v.type = ''U'')
	
union all

select
	i.name, 
	''drop index  ['' + i.name + ''] on ['' + object_name(i.object_id) + '']'' [definition], ''Index: DROP'' [description], 7 [phase]
from sys.indexes i
	left join sys.tables t on i.object_id = t.object_id
	left join sys.views v on i.object_id = v.object_id
	
where
	i.type in (1, 2, 3, 4) and i.is_primary_key = 0 and i.is_unique_constraint = 0
	and
	(t.object_id is not null and t.type = ''U'')
	or
	(v.object_id is not null and v.type = ''V'')
	
union all

select
	i.name, 
	''alter table ['' + object_name(i.object_id)  + ''] add constraint ['' + i.name + ''] '' 
	+ dbo.coreIxType(i.type, is_primary_key, is_unique, null) + dbo.coreIxColumns(i.object_id, i.index_id, is_primary_key, 0) [definition], 
	''Index - Primary Key: CREATE'',
	105
from sys.indexes i
	left join sys.tables t on i.object_id = t.object_id
	left join sys.views v on i.object_id = v.object_id
	
where
	i.type in (1, 2) and i.is_primary_key = 1
	and
	(t.object_id is not null and t.type = ''U'')
	or
	(v.object_id is not null and v.type = ''U'')
	
union all

select
	i.name, 
	''create '' + dbo.coreIxType(i.type, i.is_primary_key, i.is_unique, xi.secondary_type) + '' index ['' + i.name 
	+ ''] on ['' + object_name(i.object_id) + ''] ('' + c.name + '') '' + dbo.coreIxXmlSecondary(i.object_id, xi.using_xml_index_id, xi.secondary_type) [definition], 
	case
	when xi.using_xml_index_id is null then ''Index - XML Primary: CREATE''
	else ''Index - XML secondary: CREATE''
	end [description],
	case
	when xi.using_xml_index_id is null then 106
	else 108
	end [phase]
from sys.indexes i
	join sys.xml_indexes xi on i.object_id = xi.object_id and i.index_id = xi.index_id
	join sys.index_columns ic on ic.object_id = xi.object_id and ic.index_id = xi.index_id
	join sys.columns c on xi.object_id = c.object_id and c.column_id = ic.column_id
	left join sys.tables t on i.object_id = t.object_id
	left join sys.views v on i.object_id = v.object_id
	
where
	i.type = 3
	and
	(t.object_id is not null and t.type = ''U'')
	or
	(v.object_id is not null and v.type = ''U'')
	
union all

select
	i.name, 
	''create '' + dbo.coreIxType(i.type, is_primary_key, is_unique, null) + '' index ['' + i.name + ''] '' 
	+ dbo.coreIxColumns(i.object_id, i.index_id, is_primary_key, 0)
	+ dbo.coreIxColumns(i.object_id, i.index_id, is_primary_key, 1)
	+ case 
		when filter_definition is not null then '' where '' + filter_definition
		else '''' 
	end  [definition], 
	
	''Index: CREATE'' ,
	107
	
from sys.indexes i
	left join sys.tables t on i.object_id = t.object_id
	left join sys.views v on i.object_id = v.object_id
	
where
	i.type in (1, 2) and i.is_primary_key = 0
	and
	(t.object_id is not null and t.type = ''U'')
	or
	(v.object_id is not null and v.type = ''U'')
'