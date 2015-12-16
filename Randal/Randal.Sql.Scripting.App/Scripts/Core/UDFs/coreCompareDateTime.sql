--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreCompareDateTime', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreCompareDateTime(
	@Date1 DATETIME, 
	@Date2 DATETIME)

	returns INT
AS
BEGIN

	IF(@Date1 IS NULL AND @Date2 IS NULL)
		RETURN 0
	ELSE IF (@Date1 IS NULL)
		RETURN 1
	ELSE IF (@Date2 IS NULL)
		RETURN -1
		
	RETURN DATEDIFF(MINUTE, @Date1, @Date2)
END
GO

/*

print dbo.coreCompareDateTime('1/2/2006', '1/1/2006')	-- < 1
print dbo.coreCompareDateTime('1/2/2006', '1/2/2006')	-- 0
print dbo.coreCompareDateTime('1/1/2006', '1/2/2006')	-- > 1

print dbo.coreCompareDateTime(NULL, '1/3/2006')			-- > 1
print dbo.coreCompareDateTime(NULL, NULL)				-- 0
print dbo.coreCompareDateTime('1/2/2006', NULL)			-- < 1

*/

