--:: need coreTrim
--:: catalog $Catalogs$

--:: pre
exec coreCreateFunction 'coreTrimBrackets', 'dbo', 'scalar'
GO

--:: main
alter function coreTrimBrackets( @string varchar(max) )
	returns varchar(max)
as
begin
	declare @ch smallint
	
	if(@string is null)
		return ''
	
	set @string = dbo.coreTrim(@string)
	
	set @ch = ascii(left(@string, 1))
	if(@ch = 91 or @ch = 60 or @ch = 40 or @ch = 123)
		set @string = right(@string, len(@string) - 1)
		
	set @ch = ascii(right(@string, 1))
	if(@ch = 93 or @ch = 62 or @ch = 41 or @ch = 125)
		set @string = left(@string, len(@string) - 1)
		
	return @string
end
GO


/*

print '[' + dbo.coreTrimBrackets(' [dbo.test] ') + ']'
print '[' + dbo.coreTrimBrackets(' (dbo.test) ') + ']'
print '[' + dbo.coreTrimBrackets(' {dbo.test} ') + ']'
print '[' + dbo.coreTrimBrackets(' <dbo.test> ') + ']'
print '[' + dbo.coreTrimBrackets(null) + ']'

*/



