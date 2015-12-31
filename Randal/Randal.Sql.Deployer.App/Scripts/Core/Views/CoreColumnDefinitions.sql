--:: catalog $Catalogs$
--:: need coreTypeDef, coreTypeSize

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreColumnDefinitions',
'
with MaxColumnLengths as 
(
	select 
		c.object_id, max(len(c.name)) maxColNameLen
	from sys.columns c
	group by c.object_id
),
TablesAndViews as
(
	select
		isnull(v.object_id, t.object_id) object_id, 
		isnull(v.name, t.name) table_view,
		case when v.object_id is not null then ''V''
		else ''T'' end [type], mcl.maxColNameLen
	from sys.views v
		full join sys.tables t on t.object_id = v.object_id
		join MaxColumnLengths mcl on mcl.object_id = v.object_id or mcl.object_id = t.object_id
)
select
	tv.object_id, tv.table_view, tv.type, c.column_id [colOrder], c.name [colName],
	
	dbo.coreTypeDef(c.user_type_id, c.max_length, c.precision, c.scale) typeName,
	dbo.coreTypeDef(c.system_type_id, c.max_length, c.precision, c.scale) sysTypeName,
	
	case c.system_type_id
		when 127 then ''BigInt''
		when 173 then ''Binary''
		when 104 then ''Bit''
		when 175 then ''Char''
		when 40 then ''Date''
		when 61 then ''DateTime''
		when 42 then ''DateTime2''
		when 43 then ''DateTimeOffset''
		when 106 then ''Decimal''
		when 62 then ''Float''
		when 34 then ''Image''
		when 56 then ''Int''
		when 60 then ''Money''
		when 239 then ''NChar''
		when 99 then ''NText''
		when 108 then ''Numeric''
		when 231 then ''NVarChar''
		when 59 then ''Real''
		when 58 then ''SmallDateTime''
		when 52 then ''SmallInt''
		when 122 then ''SmallMoney''
		when 98 then ''Sql_Variant''
		when 35 then ''Text''
		when 41 then ''Time''
		when 189 then ''Timestamp''
		when 48 then ''TinyInt''
		when 36 then ''UniqueIdentifier''
		when 165 then ''VarBinary''
		when 167 then ''VarChar''
		when 241 then ''Xml''
		else sys.name
	end + dbo.coreTypeSize(c.user_type_id, c.max_length, c.precision, c.scale) [xmlName],
	
	sys.name baseType,
	c.max_length [max_byte_length],
	
	CASE c.is_nullable 
		WHEN 1 THEN ''null''
		ELSE ''not null''
	END nullText,
	
	CASE c.default_object_id
		WHEN 0 THEN ''''
		ELSE ''default '' + dc.definition 
	END defText,
	
	tv.maxColNameLen,
	isnull(ic.index_column_id, 0) [pkOrder], c.is_identity isIdentity,
	
	case c.is_identity 
	when 1 then ''identity('' + convert(varchar, id.seed_value) + '', '' + convert(varchar, id.increment_value) + '')''
	else '''' end identityText,
	
	case id.is_not_for_replication
	when 1 then ''not for replication''
	else '''' end notForReplication,
	c.system_type_id, c.user_type_id
	
from TablesAndViews tv
	join sys.Columns c ON tv.object_id = c.object_id
	join sys.Types typ ON typ.system_type_id = c.system_type_id AND typ.user_type_id = c.user_type_id
	join sys.Types sys ON sys.system_type_id = c.system_type_id AND sys.user_type_id = c.system_type_id
	left join sys.identity_columns id on id.object_id = tv.object_id and c.column_id = id.column_id
	left join sys.default_constraints  dc ON dc.object_id = c.default_object_id
	left join sys.indexes idx ON idx.object_id = tv.object_id and idx.is_primary_key = 1
	left join sys.index_columns ic ON ic.object_id = idx.object_id and idx.index_id = ic.index_id and ic.column_id = c.column_id
'


/*


select top 500 * from CoreColumnDefinitions order by table_view

select * from sys.types
select * from sys.identity_columns

select 'when ' + convert(varchar, system_type_id) + ' then ''' + name + ''''
from sys.types
where system_type_id = user_type_id
order by name

*/

