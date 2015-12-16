--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreVersionNow', 'dbo', 'scalar'
GO

--:: main
alter function coreVersionNow()
returns BIGINT
AS
BEGIN

	DECLARE @date DATETIME, @Version VARCHAR(12), @value INT

	SET @Date = GETDATE()

	SET @Version = SUBSTRING(CONVERT(VARCHAR, DATEPART(year, @Date)), 3, 2)

	SET @value = DATEPART(dayofyear, @Date)
	IF(@value < 10)
		SET @Version = @Version + '00' + CONVERT(VARCHAR, @value)
	ELSE IF(@value < 100)
		SET @Version = @Version + '0' + CONVERT(VARCHAR, @value)
	ELSE
		SET @Version = @Version + CONVERT(VARCHAR, @value)

	SET @value = DATEPART(hour, @Date)
	IF(@value < 10)
		SET @Version = @Version + '0' + CONVERT(VARCHAR, @value)
	ELSE
		SET @Version = @Version + CONVERT(VARCHAR, @value)

	SET @value = DATEPART(minute, @Date)
	IF(@value < 10)
		SET @Version = @Version + '0' + CONVERT(VARCHAR, @value)
	ELSE
		SET @Version = @Version + CONVERT(VARCHAR, @value)

	SET @value = DATEPART(second, @Date)
	IF(@value < 10)
		SET @Version = @Version + '0' + CONVERT(VARCHAR, @value)
	ELSE
		SET @Version = @Version + CONVERT(VARCHAR, @value)


	RETURN CONVERT(BIGINT, @Version)
END

/*

PRINT dbo.coreVersionNow()

*/