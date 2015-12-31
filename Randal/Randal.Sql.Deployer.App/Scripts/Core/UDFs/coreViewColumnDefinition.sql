--:: need coreDataTypeName
--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreViewColumnDefinition', 'dbo', 'inline'
GO

--:: main
alter function coreViewColumnDefinition
	( @view SYSNAME )
	RETURNS TABLE 
AS 
	RETURN
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

		FROM sys.Views t
			JOIN sys.Columns c ON t.object_id = c.object_id
			LEFT OUTER JOIN sys.indexes idx ON idx.object_id = t.object_id AND idx.is_primary_key = 1
			LEFT OUTER JOIN sys.index_columns pk ON pk.object_id = t.object_id AND idx.index_id = pk.index_id AND c.column_id = pk.column_id
			LEFT OUTER JOIN sys.default_constraints  dc ON dc.object_id = c.default_object_id
			
		WHERE t.Name = @view

/*

SELECT * FROM dbo.coreViewColumnDefinition('')

SELECT * FROM sys.Types

*/

