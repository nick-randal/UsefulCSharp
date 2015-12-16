--:: need coreDataTypeName
--:: catalog $Catalogs$

--:: ignore
use model

--:: main

EXEC coreCreateQueryView 'CoreTableColumnsView', 
'SELECT t.name [TableName], t.object_id [TableId], t.type [ObjectType], t.is_published [IsPublished], t.is_replicated [IsReplicated],
	c.column_id [ColumnId], c.name [ColumnName], dbo.coreDataTypeName(c.object_id, c.column_id) [TypeName], 
	CASE c.is_nullable 
		WHEN 1 THEN ''NULL''
		ELSE ''NOT NULL'' END [NullableText],
	CASE c.default_object_id
		WHEN 0 THEN ''''
		ELSE ''DEFAULT '' + dc.definition END [DefaultText],
	CASE WHEN pk.key_ordinal IS NOT NULL THEN pk.key_ordinal ELSE 0 END [PKOrdinal],
	c.is_identity [IsIdentity], c.is_computed [IsComputed], c.is_nullable [IsNullable], c.system_type_id [SysTypeId]

FROM sys.Tables t
	JOIN sys.Columns c ON t.object_id = c.object_id
	left outer join sys.indexes idx ON idx.object_id = t.object_id AND idx.is_primary_key = 1
	left outer join sys.index_columns pk ON pk.object_id = t.object_id AND idx.index_id = pk.index_id AND c.column_id = pk.column_id
	left outer join sys.default_constraints  dc ON dc.object_id = c.default_object_id

union all

SELECT t.name [TableName], t.object_id [TableId], t.type [ObjectType], t.is_published [IsPublished], t.is_replicated [IsReplicated], 
	c.column_id [ColumnId], c.name [ColumnName], dbo.coreDataTypeName(c.object_id, c.column_id) [TypeName], 
	CASE c.is_nullable 
		WHEN 1 THEN ''NULL''
		ELSE ''NOT NULL'' END [NullableText],
	CASE c.default_object_id
		WHEN 0 THEN ''''
		ELSE ''DEFAULT '' + dc.definition END [DefaultText],
	CASE WHEN pk.key_ordinal IS NOT NULL THEN pk.key_ordinal ELSE 0 END [PKOrdinal],
	c.is_identity [IsIdentity], c.is_computed [IsComputed], c.is_nullable [IsNullable], c.system_type_id [SysTypeId]

FROM sys.Views t
	JOIN sys.Columns c ON t.object_id = c.object_id
	left outer join sys.indexes idx ON idx.object_id = t.object_id AND idx.is_primary_key = 1
	left outer join sys.index_columns pk ON pk.object_id = t.object_id AND idx.index_id = pk.index_id AND c.column_id = pk.column_id
	left outer join sys.default_constraints  dc ON dc.object_id = c.default_object_id

	where t.Is_MS_shipped = 0
'