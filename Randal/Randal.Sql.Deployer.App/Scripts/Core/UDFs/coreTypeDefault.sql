--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreTypeDefault', 'dbo', 'scalar'
GO

--:: main
alter function coreTypeDefault(@systemTypeId int)
	returns varchar(36)
	
as begin
	
	declare @value sql_variant
	
	if @systemTypeId in (99, 35, 165, 167, 175, 231, 239)
		set @value = ''''''
	else if @systemTypeId in (59, 60, 62, 106, 108, 122)
		set @value = 0.0
	else if @systemTypeId in (48, 52, 56, 104, 127)
		set @value = 0
	else if @systemTypeId = 40
		set  @value = quotename(convert(date, getutcdate()), '''')
	else if @systemTypeId = 41
		set  @value = quotename(convert(time, getutcdate()), '''')
	else if @systemTypeId = 42
		set  @value = quotename(convert(datetime2, getutcdate()), '''')
	else if @systemTypeId = 43
		set  @value = quotename(convert(datetimeoffset, getutcdate()), '''')
	else if @systemTypeId = 58
		set  @value = quotename(convert(smalldatetime, getutcdate()), '''')
	else if @systemTypeId = 61
		set  @value = quotename(convert(datetime, getutcdate()), '''')
	else if @systemTypeId = 241
		set @value = '<a />'
	else
		set @value = 'null'
	
	return convert(varchar(36), (@value))
end

GO

/*

select dbo.coreTypeDefault(user_type_id), user_type_id, name
from sys.types

*/