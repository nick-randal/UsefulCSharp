--:: catalog $Catalogs$

--:: pre
exec coreCreateFunction 'coreTrim', 'dbo', 'scalar'
GO

--:: main
alter function coreTrim( @s nvarchar(max) )
	returns nvarchar(max)

AS BEGIN

	set @s = rtrim(@s)

	while len(@s) > 0 and
		  (ascii(substring(@s,1,1)) <= 32 or ascii(substring(@s,len(@s),1)) <= 32)
	begin
		set @s = ltrim(rtrim(@s))
		if ascii(substring(@s,1,1)) <= 32
			set @s = substring(@s,2,len(@s)-1)
		if ascii(substring(@s,len(@s),1)) <= 32
			set @s = substring(@s,1,len(@s)-1)
		set @s = rtrim(@s)
	end
	return @s 
END

GO

/*
print '[' + dbo.coreTrim(' hello ') + ']'
*/