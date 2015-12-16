--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreLiteral', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreLiteral(@dataitem SQL_VARIANT)
	RETURNS varchar(max)
AS
BEGIN
	DECLARE @p varchar(32),
			@s varchar(max)

	-- validate @dataitem
	IF( @dataitem IS NULL)
		RETURN 'NULL'

	SET @p = convert(varchar(32), SQL_VARIANT_PROPERTY(@dataitem,'BaseType'))
	SET @s = convert(varchar(max), @dataitem)
	
	IF( @p = 'varchar')
		SET @s = '''' + replace(@s,'''','''''') + ''''
	ELSE
		SET @s = convert(varchar(max),@dataitem)

	RETURN @s        
END
GO

/*

select dbo.coreLiteral(1), dbo.coreLiteral('test'), dbo.coreLiteral(null), dbo.coreLiteral('"'), dbo.coreLiteral('test''s')

*/