--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreCapitalize', 'dbo', 'scalar'
GO

--:: main
alter function coreCapitalize( @text varchar(500) )
	returns varchar(500)
as begin
	
	declare @len int, @index int
	
	if(@text is null or @text = '')
		return ''
	
	set @text = ltrim(rtrim(lower(@text)))
	set @len = len(@text)
	
	set @text = left(upper(@text), 1) + right(@text, @len - 1)
	
	-- the first time can be collation free
	set @index = PATINDEX('%[ .\"-][a-z]%', @text)
	while @index != 0 begin
		set @text = left(@text, @index) + upper(substring(@text, @index + 1, 1)) + right(@text, @len - @index - 1)
	
		-- this does not work without the collation
		set @index = PATINDEX('%[ .\"-][abcdefghijklmnopqrstuvwxyz]%', @text  collate SQL_Latin1_General_CP1_CS_AS)
	end
	
	return @text
end

GO

/*

PRINT dbo.coreCapitalize('united states of america')
PRINT dbo.coreCapitalize('a.j. smith')
PRINT dbo.coreCapitalize('bill smith-johnson')

SELECT *
FROM fn_helpcollations()


*/