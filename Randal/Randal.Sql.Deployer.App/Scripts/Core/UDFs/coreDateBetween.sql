--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreDateBetween', 'dbo', 'scalar'
GO

--:: main
alter function coreDateBetween(@date datetime, @start datetime, @end datetime)
	returns bit
as begin

	declare @diffStart int, @diffEnd int
	
	if @date is null
		return 0

	if @start is null and @end is null
		return 1
		
	select @diffStart = datediff(day, @start, @date), @diffEnd = datediff(day, @date, @end)
	
	if @diffStart < 0 or @diffEnd < 0
		return 0
	
	return 1
end

GO

/*

-- test cases

if 0 <> dbo.coreDateBetween('1/6/2010', '1/2/2010', '1/5/2010')	raiserror('failed 1',1,1)
if 0 <> dbo.coreDateBetween('1/1/2010', '1/2/2010', '1/5/2010')	raiserror('failed 2',1,1)
if 0 <> dbo.coreDateBetween(null, '1/2/2010', '1/5/2010')		raiserror('failed 3',1,1)
if 1 <> dbo.coreDateBetween('1/2/2010', '1/2/2010', '1/5/2010')	raiserror('failed 4',1,1)
if 1 <> dbo.coreDateBetween('1/5/2010', '1/2/2010', '1/5/2010')	raiserror('failed 5',1,1)
if 1 <> dbo.coreDateBetween('1/3/2010', '1/2/2010', '1/5/2010')	raiserror('failed 6',1,1)
if 1 <> dbo.coreDateBetween('1/3/2010', null, '1/5/2010')		raiserror('failed 7',1,1)
if 1 <> dbo.coreDateBetween('1/3/2010', '1/2/2010', null)		raiserror('failed 8',1,1)
if 0 <> dbo.coreDateBetween('1/6/2010', null, '1/5/2010')		raiserror('failed 7',1,1)
if 0 <> dbo.coreDateBetween('1/1/2010', '1/2/2010', null)		raiserror('failed 8',1,1)

*/