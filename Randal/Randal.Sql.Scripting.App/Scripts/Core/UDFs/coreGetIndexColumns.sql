--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreGetIndexColumns', 'dbo', 'scalar'
GO

--:: main
alter function coreGetIndexColumns(@table_id int, @index_id int, @included bit)
	returns varchar(max)
as begin

	declare @columns varchar(max), @column sysname, @descendingText sysname
	
	declare IX cursor fast_forward for
		select columnName, descendingText
		from IXs 
		where table_id = @table_id and index_id = @index_id and isIncluded = @included
		order by ordinal
	
	open IX
	
	fetch next from IX into @column, @descendingText
	while @@fetch_status = 0 begin
	
		if @columns is null
			set @columns = rtrim(@column + ' ' + @descendingText)
		else
			set @columns = @columns + ', ' + rtrim(@column + ' ' + @descendingText)
	
		fetch next from IX into @column, @descendingText
	end
	
	close IX
	deallocate IX
	
	return @columns
end

GO

/*

PRINT dbo.coreGetIndexColumns(349348409, 1)
PRINT dbo.coreGetIndexColumns(349348409, 2)

select * from IXs

helpme IXs

*/