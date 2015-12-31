--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreToLocalTime', 'dbo', 'scalar'
GO

--:: main
alter function coreToLocalTime(@utc datetime)
	returns datetime
	
as begin
	
	declare @mins int
	
	set @mins = datediff(ss, getutcdate(), getdate())
		
	return dateadd(ss, @mins, @utc)
end

GO

/*

print convert(varchar, dbo.toLocalTime(getutcdate()), 109)

*/