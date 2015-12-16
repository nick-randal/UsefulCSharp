--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreIxXmlSecondary', 'dbo', 'scalar'
GO

--:: main
alter function coreIxXmlSecondary (@object_id int, @index_id int, @secondary char(1)) 
	returns varchar(256)

as begin

	declare @desc varchar(256)
	
	if(@index_id is null)
		return ''
	
	select @desc = name from sys.xml_indexes where object_id = @object_id and index_id = @index_id
	
	set @desc = 'using xml index [' + @desc + '] FOR '
		+ 
		case @secondary
			when 'P' then 'PATH'
			when 'V' then 'VALUE'
			when 'R' then 'PROPERTY'
			else ''
		end

	return @desc
end

/*

select object_name(256000)


select dbo.coreIxXmlSecondary(980302652, 256000, 'P')



select 
	'select dbo.coreIxXmlSecondary(' + convert(varchar, object_id) 
	+ ', ' + convert(varchar, using_xml_index_id) 
	+ ', ' + isnull(convert(varchar, secondary_type), 'null') + ')' 
from sys.xml_indexes xi 


*/