--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreNextArg'
GO

--:: main
alter procedure coreNextArg
    @sourceList		nvarchar(max) output,
    @arg			nvarchar(max) output,
    @delimiter		nchar(1) = N','

as begin

    declare @i int,
            @parenCount int,
            @inString nvarchar(1),
            @ch nchar(1)

    set nocount on
    
    set @parenCount = 0
    set @inString = ''
    set @i = 0
    set @ch = ' '
    set @sourceList = dbo.coreTrim(@sourceList)

    while @i < len(@sourceList)
    begin
		-- increment the index and get the next character
        set @i = @i + 1
        set @ch = substring(@sourceList,@i,1)

        if (@inString <> N'')
        begin
            if @ch <> @inString continue
            if @i+1 <= len(@sourceList) and substring(@sourceList, @i + 1, 1) = @inString
                set @i = @i + 1
            else
                set @inString = ''
            continue
        end

        if @ch = @delimiter and @parenCount <= 0
        begin
            set @arg = dbo.coreTrim(substring(@sourceList, 1, @i - 1))
            set @sourceList = dbo.coreTrim(substring(@sourceList, @i + 1, len(@sourceList) - @i))
            return 1
        end

        --if @ch = '''' or @ch = '"'
        if @ch = N'"'
            set @inString = @ch
        else if @ch = N'('
            set @parenCount = @parenCount + 1
        else if @ch = N')'
            set @parenCount = @parenCount - 1
    end

    set @arg = @sourceList
    set @sourceList = N''
    
    return 0
end

/*
DECLARE @sql varchar(max), @token varchar(max)
SET @sql = '[AColumn] varchar(max) IDENTITY(1,1) IS NOT NULL DEFAULT(''Default'')'

WHILE @sql <> ''
BEGIN
    EXEC coreNextArg @sql OUTPUT, @token OUTPUT
    PRINT @token + ' = ''' + @token + ''''
END
*/