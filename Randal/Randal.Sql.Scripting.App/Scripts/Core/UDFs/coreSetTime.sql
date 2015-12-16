--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreSetTime', 'dbo', 'scalar'
GO

--:: main
alter function coreSetTime(@dt datetime, @hours int, @minutes int, @seconds int)
	returns datetime
	
as begin
	
	set @hours = (-1 * datepart(hh, @dt) + @hours) * 3600
	set @minutes = (-1 * datepart(mi, @dt) + @minutes) * 60
	set @seconds = -1 * datepart(ss, @dt) + @seconds
	
	return dateadd(ss, @hours + @minutes + @seconds, @dt)
end

GO

/*

declare @dt datetime = getdate()

PRINT convert(varchar, dbo.coreSetTime(@dt, 4, 12, 13), 9)

PRINT convert(varchar, dbo.coreSetTime(@dt, 17, 0, 0), 9)

PRINT convert(varchar, dbo.coreSetTime(@dt, -5, 0, 0), 9)

*/