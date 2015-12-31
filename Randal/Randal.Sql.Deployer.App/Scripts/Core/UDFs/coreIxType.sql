--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreIxType', 'dbo', 'scalar'
GO

--:: main
alter function coreIxType (@type tinyint, @pk bit, @unique bit, @xml char(1)) 
	returns varchar(24)

as begin

	declare @desc varchar(24)
	
	set @desc = case @type
	when 1 then 'clustered'
	when 2 then 'nonclustered'
	when 3 then 'xml'
	when 4 then 'spatial'
	else ''
	end
	
	if @type = 3 and @xml is null
		set @desc = 'primary ' + @desc
	

	if(@pk = 1) begin
	
		set @desc = 'primary key ' + @desc
		
	end else begin
	
		if(@unique = 1)
			set @desc = 'unique ' + @desc
	end

	return @desc
end

/*

select dbo.coreIxType(0, 0, 0, null)
select dbo.coreIxType(1, 0, 0, null)
select dbo.coreIxType(1, 0, 1, null)
select dbo.coreIxType(1, 1, 1, null)
select dbo.coreIxType(2, 0, 0, null)
select dbo.coreIxType(2, 0, 1, null)
select dbo.coreIxType(2, 1, 1, null)
select dbo.coreIxType(3, 0, 0, null)
select dbo.coreIxType(3, 0, 0, 'P')


select 
	'select dbo.coreIxType(' + convert(varchar, i.type) + ', ' + convert(varchar, i.is_primary_key) + ', ' 
	+ convert(varchar, i.is_unique) + ', ' + isnull(convert(varchar, xi.secondary_type), 'null') + ')'
from sys.indexes i
left join sys.xml_indexes xi on i.object_id = xi.object_id and i.index_id = xi.index_id
group by i.type, i.is_primary_key, i.is_unique, xi.secondary_type
order by i.type, i.is_primary_key, i.is_unique, xi.secondary_type


select *
from sys.indexes i
left join sys.xml_indexes xi on i.object_id = xi.object_id and i.index_id = xi.index_id

select * from sys.indexes where type = 3



select dbo.coreIxType(object_id) from sys.foreign_keys

*/





