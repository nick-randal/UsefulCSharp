
--:: catalog $Catalogs$

--:: pre
EXEC coreCreateProcedure 'coreGetColumnDefinitions'
GO

--:: main
ALTER PROCEDURE coreGetColumnDefinitions
	@TableName SYSNAME = NULL
AS

	SELECT t.name [TableName], t.object_id [TableId], 
		c.column_id [ColumnId], c.name [ColumnName], 
		dbo.coreDataTypeName(c.object_id, c.column_id) [TypeName], 
		CASE c.is_nullable 
			WHEN 1 THEN 'NULL'
			ELSE 'NOT NULL' END [NullableText],
		CASE c.default_object_id
			WHEN 0 THEN ''
			ELSE 'DEFAULT ' + dc.definition END [DefaultText],
		CASE WHEN pk.key_ordinal IS NOT NULL THEN pk.key_ordinal ELSE 0 END [PKOrdinal],
		c.is_identity [IsIdentity], c.is_computed [IsComputed], c.is_nullable [IsNullable], c.system_type_id [SysTypeId]
	
		FROM sys.Tables t
		JOIN sys.Columns c ON t.object_id = c.object_id
		LEFT OUTER JOIN sys.indexes idx ON idx.object_id = t.object_id AND idx.is_primary_key = 1
		LEFT OUTER JOIN sys.index_columns pk ON pk.object_id = t.object_id AND idx.index_id = pk.index_id AND c.column_id = pk.column_id
		JOIN sys.Types typ ON typ.system_type_id = c.system_type_id AND typ.user_type_id = c.user_type_id
		LEFT OUTER JOIN sys.default_constraints  dc ON dc.object_id = c.default_object_id
		
		WHERE @TableName IS NULL OR t.Name = @TableName
		ORDER BY TableName, ColumnId

	RETURN -1
GO

/*

EXEC coreGetColumnDefinitions

*/