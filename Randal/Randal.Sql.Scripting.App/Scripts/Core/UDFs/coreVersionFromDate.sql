--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreVersionFromDate', 'dbo', 'scalar'
GO

--:: main
alter function coreVersionFromDate(@date DATETIME, @iteration tinyint)
returns VARCHAR(11)
AS
BEGIN

	DECLARE @Version VARCHAR(11), @value INT

	SET @Version = SUBSTRING(CONVERT(VARCHAR, DATEPART(year, @Date)), 3, 2) + '.'

	SET @value = DATEPART(month, @Date)
	IF(@value < 10)
		SET @Version = @Version + '0' + CONVERT(VARCHAR, @value)
	ELSE
		SET @Version = @Version + CONVERT(VARCHAR, @value)

	SET @Version = @Version + '.'
	
	SET @value = DATEPART(day, @Date)
	IF(@value < 10)
		SET @Version = @Version + '0' + CONVERT(VARCHAR, @value)
	ELSE
		SET @Version = @Version + CONVERT(VARCHAR, @value)

	IF(@iteration < 10)
		SET @Version = @Version + '.0' + CONVERT(VARCHAR, @iteration)
	ELSE
		SET @Version = @Version + '.' + CONVERT(VARCHAR, @iteration)

	RETURN @Version
END

/*

PRINT dbo.coreVersionFromDate(GETDATE(), 1)

PRINT dbo.coreVersionFromDate(GETDATE(), 11)

PRINT dbo.coreVersionFromDate(GETDATE(), 100)

PRINT dbo.coreVersionFromDate('1-2-2003', 3)

*/