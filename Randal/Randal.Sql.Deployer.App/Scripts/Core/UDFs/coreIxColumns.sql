--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreIxColumns', 'dbo', 'scalar'
GO

--:: main
alter function coreIxColumns (@object_id int, @index_id int, @isPk bit, @included bit) 
	returns varchar(512)

as begin

	declare @desc varchar(512)
	
	set @desc =
	(
		select '[' + c.name + ']' + case is_descending_key when 1 then ' desc, ' else ', ' end
		from sys.index_columns ic
			join sys.columns c on ic.object_id = c.object_id and ic.column_id = c.column_id
		where ic.object_id = @object_id and ic.index_id = @index_id and ic.is_included_column = @included
		for xml path('')
	)
	
	if(@desc is null)
		return ''

	if(@isPk = 1)
		return ' ( ' + left(@desc, len(@desc) - 1)  + ' )'

	if(@included = 1)
		return ' include ( ' + left(@desc, len(@desc) - 1)  + ' )'
	
	return ' on [' + object_name(@object_id) + '] ( ' + left(@desc, len(@desc) - 1) + ' )'
	
end

/*

select dbo.coreIxColumns(578205210, 4, 0, 0)
select dbo.coreIxColumns(578205210, 4, 0, 1)

select name, object_id, index_id, filter_definition from sys.indexes
select * from sys.index_columns

*/