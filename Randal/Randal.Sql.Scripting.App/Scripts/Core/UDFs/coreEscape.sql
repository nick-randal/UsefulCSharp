--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreEscape', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreEscape(@input varchar(max))
	RETURNS varchar(max)
AS
BEGIN
	
	set @input = replace(replace(@input, '''', ''''''), '"', '""')

	RETURN @input
END
GO

/*

select dbo.coreEscape(1), dbo.coreEscape('test'), dbo.coreEscape(null), dbo.coreEscape('"bob"'), dbo.coreEscape('test''s')

*/