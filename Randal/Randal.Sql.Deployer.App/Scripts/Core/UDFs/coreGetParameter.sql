--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreGetParameter', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreGetParameter(@group VARCHAR(32), @param VARCHAR(32))
	returns sql_variant
AS
BEGIN

	DECLARE @Value sql_variant
	
	set @value = null
	
	SELECT @Value = ParamValue 
		FROM CoreParameters 
		WHERE GroupName = @group and ParamName = @param

	RETURN @Value

END
GO

/*

	PRINT convert(varchar, dbo.coreGetParameter('tables', 'Test'))

	select * from CoreParameters

*/


