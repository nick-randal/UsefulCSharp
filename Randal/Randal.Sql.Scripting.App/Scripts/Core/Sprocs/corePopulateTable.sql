--:: need coreNextArg, corePivotDelimitedList, coreTrimBrackets, coreRemoveQuotes
--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'corePopulateTable'
GO

--:: main
alter procedure corePopulateTable
    @database		sysname,
    @table			sysname,
    @keyColumns		varchar(max),
    @otherColumns	varchar(max),
    @rows			nvarchar(max),
    @debug			bit = 0

as begin

	set nocount on
	
	declare	@cmd			nvarchar(max),
			@tmp			nvarchar(max),
			@sep			nvarchar(10),
			@row			nvarchar(max), 
			@value			nvarchar(max),
			@data			nvarchar(max)
	
	declare @keys table (col sysname, ordinal int)
	declare @nonKeys table(col sysname, ordinal int)
	
	insert @keys	(col, ordinal) select '[' + value + ']', ordinal from dbo.corePivotDelimitedList(@keyColumns, ',')
	insert @nonKeys (col, ordinal) select '[' + value + ']', ordinal from dbo.corePivotDelimitedList(@otherColumns, ',')


    if(isnull(@database, '') = '')
        set @database = db_name()
	
	--------------------- Merge Destination Table
	set @cmd = 'merge [' + @database + '].dbo.[' + dbo.coreTrimBrackets(@table) + '] dst
using
(
'
	
	--------------------- Using Data
	set @data = N''
	while @rows <> N'' begin

		set @data = @data + char(9) + N'select '
        exec coreNextArg @rows output, @row output, N';'
        
        while @row != N'' begin
			exec coreNextArg @row output, @value output
			set @data = @data + N'''' + dbo.coreRemoveQuotes(replace(@value, N'''', N'''''')) + N''', '
		end
		
		set @data = substring(@data, 1, len(@data) - 1) + N' union all' + char(10)
		
	end
	
	set @data = substring(@data, 1, len(@data) - 11)
	set @cmd = @cmd + @data
	
	--------------------- Source Column Aliases
	set @tmp = ''
	select @tmp = coalesce(@tmp + ', k', '') + convert(varchar(3), ordinal) from @keys order by ordinal
	select @tmp = coalesce(@tmp + ', c', '') + convert(varchar(3), ordinal) from @nonKeys order by ordinal
	set @tmp = substring(@tmp, 3, len(@tmp))
	
	set @cmd = @cmd + '
) as src ( ' + @tmp + ' )
	
on '
	
	--------------------- On Condition
	set @tmp = ''
	select @tmp = coalesce(@tmp + ' and dst.', '') + col + ' = src.k' + convert(varchar(3), ordinal) from @keys order by ordinal
	set @tmp = substring(@tmp, 6, len(@tmp))
	
	set @cmd = @cmd + @tmp + '
	
when matched then
	update set
		'
	--------------------- Update Set Assignments
	set @tmp = ''
	set @sep = ',' + char(10) + char(9) + char(9) + char(9)
	select @tmp = coalesce(@tmp + @sep, '') + col + ' = src.c' + convert(varchar(3), ordinal) from @nonKeys order by ordinal
	set @tmp = substring(@tmp, 6, len(@tmp))
	
	set @cmd = @cmd + @tmp
	
	--------------------- Insert Columns
	set @tmp = ''
	select @tmp = coalesce(@tmp + ', ', '') + col from @keys order by ordinal
	select @tmp = coalesce(@tmp + ', ', '') + col from @nonKeys order by ordinal
	set @tmp = substring(@tmp, 3, len(@tmp))
	
	set @cmd = @cmd +
	'
	
when not matched then
	insert
	( '
	+ @tmp +
	' ) 
	values 
	( '
	
	--------------------- Insert Values
	set @tmp = ''
	select @tmp = coalesce(@tmp + ', src.k', '') + convert(varchar(3), ordinal) from @keys order by ordinal
	select @tmp = coalesce(@tmp + ', src.c', '') + convert(varchar(3), ordinal) from @nonKeys order by ordinal
	set @tmp = substring(@tmp, 3, len(@tmp))
		
	set @cmd = @cmd + @tmp + ' )'
	
	--------------------------------------------------------------------------------------------------
	-- All done formatting, lets try it out
	
	set @cmd = 'set nocount on
	
' + @cmd + '
;

set nocount off'
        
    --set @debug = 1
    set nocount off
    
    if(@debug = 1) begin
		print @cmd
		
		return -1
	end
	
	
	begin try
		exec(@cmd)
	end try
	begin catch
		print 'The following command generated an error!'
		print ''
		print @cmd
		print ''
		print 'Error (' + convert(varchar, ERROR_NUMBER()) + ') : ' + ERROR_MESSAGE()
	end catch
	
	return -1
	
end