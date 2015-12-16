--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreFkReferenceColumns', 'dbo', 'scalar'
GO

--:: main
alter function coreFkReferenceColumns (@fk_id int) 
	returns varchar(1024)

as begin

	declare @columns varchar(1024)

	set @columns =
	(
		select 
			'[' + col_name(referenced_object_id, referenced_column_id) + '], '
		from sys.foreign_key_columns
		where constraint_object_id = @fk_id
		order by constraint_column_id
		for xml path ('')
	)

	return left(@columns, len(@columns) - 1)
end

/*

select dbo.coreFkReferenceColumns(object_id) from sys.foreign_keys

*/






