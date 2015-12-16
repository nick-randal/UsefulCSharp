--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreTypeSize', 'dbo', 'scalar'
GO

--:: main
alter function coreTypeSize(@userTypeId int, @max_length int, @precision int, @scale int)
	returns varchar(255)
	
as begin
	
	declare @def varchar(255)
	
	select @def = case 
			when t.is_user_defined = 1 then ''
			when t.system_type_id IN (165, 167, 231) and @max_length = -1 then '(max)'
			when t.system_type_id IN (165, 167, 175) THEN '(' + convert(varchar, @max_length) + ')'
			when t.system_type_id IN (231, 239) THEN '(' + convert(varchar, @max_length / 2) + ')'
			when t.system_type_id IN (60, 106, 108, 122) then '(' + convert(varchar, @precision) + ', ' + convert(varchar, @scale) + ')'
		else '' end
	from sys.types t 
	where t.user_type_id = @userTypeId
		
	return @def
end

GO

/*

PRINT dbo.coreTypeSize(231, 100, 0, 0)

select * from sys.types

*/