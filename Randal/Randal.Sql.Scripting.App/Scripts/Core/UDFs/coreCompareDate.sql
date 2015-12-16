--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreCompareDate', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreCompareDate(
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
		
	RETURN DATEDIFF(Day, @Date1, @Date2)
END
GO

/*

print dbo.coreCompareDate('1/2/2006', '1/1/2006')	-- < 1
print dbo.coreCompareDate('1/2/2006', '1/2/2006')	-- 0
print dbo.coreCompareDate('1/1/2006', '1/2/2006')	-- > 1

print dbo.coreCompareDate(NULL, '1/3/2006')			-- > 1
print dbo.coreCompareDate(NULL, NULL)				-- 0
print dbo.coreCompareDate('1/2/2006', NULL)			-- < 1

*/

