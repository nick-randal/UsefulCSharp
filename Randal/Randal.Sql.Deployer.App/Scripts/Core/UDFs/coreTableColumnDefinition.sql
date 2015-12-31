--:: need coreDataTypeName
--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreTableColumnDefinition', 'dbo', 'inline'
GO

--:: main
alter function coreTableColumnDefinition
	( @table SYSNAME )
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
		
		FROM sys.Tables t
			JOIN sys.Columns c ON t.object_id = c.object_id
			LEFT OUTER JOIN sys.indexes idx ON idx.object_id = t.object_id AND idx.is_primary_key = 1
			LEFT OUTER JOIN sys.index_columns pk ON pk.object_id = t.object_id AND idx.index_id = pk.index_id AND c.column_id = pk.column_id
			LEFT OUTER JOIN sys.default_constraints  dc ON dc.object_id = c.default_object_id
			
		WHERE t.Name = @table

/*

SELECT * FROM dbo.coreTableColumnDefinition('erp_campaign_detail')

SELECT * FROM dbo.coreTableColumnDefinition('cals')

SELECT * FROM sys.Types


PRINT OBJECT_NAME(165)

select * from sys.columns c

where object_name(c.object_id) = 'erp_campaign_detail'

select col_length('erp_campaign_detail', 'message')

*/

