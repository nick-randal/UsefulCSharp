--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreFkCascading', 'dbo', 'scalar'
GO

--:: main
alter function coreFkCascading (@update tinyint, @delete tinyint) 
	returns varchar(48)

as begin

	declare @cascades varchar(48)
	
	set @cascades = ' on delete ' + 
		case @delete
		when 1 then 'cascade'
		when 2 then 'set null'
		when 3 then 'set default'
		else 'no action'
		end

	set @cascades = @cascades + ' on update ' + 
		case @update
		when 1 then 'cascade'
		when 2 then 'set null'
		when 3 then 'set default'
		else 'no action'
		end
		
	return @cascades
end

/*

select dbo.coreFkCascading(update_referential_action, delete_referential_action) from sys.foreign_keys

*/

