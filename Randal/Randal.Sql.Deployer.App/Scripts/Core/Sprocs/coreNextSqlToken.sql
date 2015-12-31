--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreNextSqlToken'
GO

--:: main
ALTER PROCEDURE coreNextSqlToken
    @sourceText varchar(4000) output,     -- As token text is removed this string becomes shorter
    @tokenText varchar(4000) output,      -- Token text including trailing blanks
    @tokenType varchar(32) = '' output    -- Name, Number, String, Operator, None
AS
BEGIN
    DECLARE @i int,
            @parenCount int,
            @inString varchar(1),
            @ch char(1),
            @endDel char(1)

    set nocount on
    set @sourceText = ltrim(@sourceText)
    
	if @sourceText = ''
    begin
        set @tokenType = 'None'
        set @tokenText = ''
        return
    end
    
	set @ch = substring(@sourceText,1,1)
    set @i = 1
    
	if (@ch >= 'A' and @ch <= 'Z') or @ch = '$' or @ch = '_'
    begin
        set @tokenType = 'Name'
        while @i < len(@sourceText)
        begin
            set @ch = substring(@sourceText,@i+1,1)
            if (@ch >= 'A' and @ch <= 'Z') or (@ch >= '0' and @ch <= '9') or @ch = '$' or @ch = '_'
                set @i = @i + 1
            else
                break
        end
    end
    else if (@ch = '"' or @ch = '[')
    begin
        set @tokenType = 'Name'
        set @endDel = case when @ch = '"' then '"' else ']' end
        set @i = @i + 1
        while @i < len(@sourceText)
        begin
            set @ch = substring(@sourceText, @i+1, 1)
            set @i = @i + 1
            if (@ch = @endDel)
                break
        end
    end
    else if (@ch between '0' and '9') or (@ch = '.' and @i < len(@sourceText) and (substring(@sourceText,@i+1,1) between '0' and '9'))
    begin
        set @tokenType = 'Number'
        while @i < len(@sourceText) and (substring(@sourceText,@i+1,1) between '0' and '9')
            set @i = @i + 1
        if @i < len(@sourceText) and substring(@sourceText,@i+1,1) = '.'
            set @i = @i + 1
        while @i < len(@sourceText) and (substring(@sourceText,@i+1,1) between '0' and '9')
            set @i = @i + 1
    end
    else if (@ch = '''')
    begin
        set @tokenType = 'String'
        while @i < len(@sourceText) and (substring(@sourceText,@i+1,1) <> @ch or (@i+1 < len(@sourceText) and substring(@sourceText,@i+2,1) = @ch))
            set @i = @i + 1
        if @i < len(@sourceText) and substring(@sourceText,@i+1,1) = @ch
            set @i = @i + 1
    end
    else
        set @tokenType = 'Operator'
    
	while @i < len(@sourceText) and substring(@sourceText,@i+1,1) = ''
        set @i = @i + 1
    
	set @tokenText = substring(@sourceText,1,@i)
--    set @tokenText = dbo.coreTrim(substring(@sourceText,1,@i))
    set @sourceText = substring(@sourceText,@i+1,len(@sourceText)-@i)
    
	return
end
GO

/*
DECLARE @sql varchar(max), @token varchar(max), @tokenType varchar(max)
SET @sql = '[AColumn] varchar(max) IDENTITY(1,1) IS NOT NULL DEFAULT(''Default'')'

WHILE @sql <> ''
BEGIN
    EXEC coreNextSqlToken @sql OUTPUT, @token OUTPUT, @tokenType OUTPUT
    PRINT @tokenType + ' = ''' + @token + ''''
END
*/
