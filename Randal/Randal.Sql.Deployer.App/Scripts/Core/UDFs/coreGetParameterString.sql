--:: catalog $Catalogs$

--:: pre
EXEC coreCreateFunction 'coreGetParameterString', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreGetParameterString(@group VARCHAR(32), @param VARCHAR(32))
	returns varchar(max)
AS
BEGIN

	DECLARE @value varchar(max)
	
	set @value = null
	
	SELECT @Value = convert(varchar(max), ParamValue) 
		FROM CoreParameters 
		WHERE GroupName = @group and ParamName = @param

	RETURN @Value

END
GO

/*
	PRINT dbo.coreGetParameterString('tables', 'Test')
*/

