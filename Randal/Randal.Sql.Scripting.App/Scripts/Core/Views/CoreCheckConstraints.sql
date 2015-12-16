--:: catalog $Catalogs$

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreCheckConstraints',
'
select 
	name, 
	''alter table [dbo].['' + object_name(parent_object_id) + ''] DROP CONSTRAINT ['' + name + '']'' [definition], ''Check Constraint: DROP'' [description], 1 [phase]
from sys.check_constraints

union all

select 
	name, 
	''alter table [dbo].['' + object_name(parent_object_id) + ''] WITH NOCHECK ADD  CONSTRAINT ['' + name + ''] CHECK '' + definition, ''Check Constraint: CREATE'', 105
from sys.check_constraints

union all

select 
	name, 
	''alter table [dbo].['' + object_name(parent_object_id) + ''] CHECK CONSTRAINT ['' + name + '']'', ''Check Constraint: SET'', 115
from sys.check_constraints
'

/*

select * from CoreCheckConstraints order by phase

*/