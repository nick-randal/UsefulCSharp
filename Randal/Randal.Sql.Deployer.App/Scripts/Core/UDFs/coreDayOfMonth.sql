--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreDayOfMonth', 'dbo', 'scalar'
GO

--:: main
alter function coreDayOfMonth(@month tinyint, @year smallint, @iteration tinyint, @weekday tinyint)
	returns datetime
	
as begin
	
	declare @addDays tinyint, @date datetime, @startShift smallint
	
	
	set @date = convert(datetime, convert(varchar, @month) + '-1-' + convert(varchar, @year))
	
	set @startShift = @weekday - datepart(weekday, @date)
	if @startShift < 0
		set @startShift = 7 + @startShift
	
	
	set @addDays = (7 * (@iteration - 1)) + @startShift-- - @weekday--  ((8 - @weekday) % 7)
	
	return dateadd(day, @addDays, @date)
end

GO

/*

print dbo.coreDayOfMonth(3, 2010, 2, 1)		-- get 2nd Sunday of March 2010
print dbo.coreDayOfMonth(11, 2010, 1, 1)		-- get 1st Sunday of November 2010

print dbo.coreDayOfMonth(3, 2011, 2, 1)		-- get 2nd Sunday of March 2011
print dbo.coreDayOfMonth(11, 2011, 1, 1)		-- get 1st Sunday of November 2011


print dbo.coreDayOfMonth(12, 2010, 1, 3)		
			

print dbo.coreDayOfMonth(month(getdate()), year(getdate()), 2, 3)	-- get 2nd Tuesday of current month


select @@datefirst, year(getdate()), datepart(weekday, getdate())


declare @addDays tinyint, @date datetime
set @date = convert(datetime, convert(varchar, 11) + '-1-' + convert(varchar, 2010))
set @addDays = (7 * (1 - 1)) + (7 - datepart(weekday, @date))
select @date, @addDays, datepart(weekday, @date)

*/

--select datepart(weekday, convert(datetime, '11-6-2010')), datename(weekday, convert(datetime, '11-6-2010'))