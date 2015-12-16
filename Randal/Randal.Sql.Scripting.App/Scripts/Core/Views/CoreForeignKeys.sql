--:: catalog $Catalogs$

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreForeignKeys',
'
select
	name, 
	''alter table ['' + object_name(parent_object_id) + ''] drop constraint ['' + name + '']'' [definition], ''Foreign Key: DROP'' [description], 3 [phase]
from sys.foreign_keys

union all

select
	name, 
	''alter table ['' + object_name(parent_object_id) + ''] WITH CHECK ADD  CONSTRAINT  ['' + name 
	+ ''] FOREIGN KEY ('' + dbo.coreFkParentColumns(object_id) + '') REFERENCES ['' 
	+ object_name(referenced_object_id) + ''] ('' + dbo.coreFkReferenceColumns(object_id) + '')'' + dbo.coreFkCascading(update_referential_action, delete_referential_action)
	[definition], 
	''Foreign Key: CREATE'' [description], 107 [phase]
from sys.foreign_keys

union all

select
	name, 
	''alter table ['' + object_name(parent_object_id) + ''] CHECK CONSTRAINT  ['' + name + '']'', ''Foreign Key: SET'', 117
from sys.foreign_keys
'