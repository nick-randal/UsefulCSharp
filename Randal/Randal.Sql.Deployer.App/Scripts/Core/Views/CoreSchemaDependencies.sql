--:: catalog $Catalogs$

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreSchemaDependencies',
'
select sed.referenced_entity_name [referencedEntity], vw.name [view], p.name [sproc], 
case 
when p.name is not null then ''exec coreDropProcedure '''''' + p.name + ''''''''
when vw.name is not null then ''exec coreDropView '''''' + vw.name + ''''''''
else '''' end [dropText]
from sys.sql_expression_dependencies sed
left join sys.procedures p on sed.referencing_id = p.object_id
left join sys.views vw on sed.referencing_id = vw.object_id
'


/*

exec coreDropView 'CoreSchemaDependencies'

exec corePrintTableColumns 'CoreSchemaDependencies', NULL

select top 100 * from CoreSchemaDependencies

*/

