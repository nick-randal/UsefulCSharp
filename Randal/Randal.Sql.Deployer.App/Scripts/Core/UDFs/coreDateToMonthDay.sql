--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreDateToMonthDay', 'dbo', 'scalar'
GO

--:: main
alter function coreDateToMonthDay(@date datetime)
	returns varchar(5)
as begin
	
	return convert(varchar, month(@date)) + '/' + convert(varchar, day(@date))
end

GO

/*

PRINT dbo.coreDateToMonthDay()

*/