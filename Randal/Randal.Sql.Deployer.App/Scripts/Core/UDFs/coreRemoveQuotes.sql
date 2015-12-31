--:: need coreTrim
--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreRemoveQuotes', 'dbo', 'scalar'
GO

--:: main
alter function coreRemoveQuotes( @string nvarchar(max) )
	returns nvarchar(max)

as begin

	-- mc = marker character
	declare @mc nchar(1), @ch1 nchar(1), @ch2 nchar(1), @s nvarchar(max), @len int, @ndx int

	set @s = ''
	set @string = dbo.coreTrim(@string)
	 
	-- get the first character of the string
	set @mc = substring(@string, 1, 1)
	
	-- check for ' or " or first character is the same as the last character
	-- if none of those are true return the string
	if (@mc <> '''' and @mc <> '"') or @mc <> substring(@string, len(@string), 1)
		return @string
		
	-- get the string without the first and last character
	set @string = substring(@string, 2, len(@string)-2)
	set @len = len(@string)
	set @ndx = 1
	
	while (@ndx <= @len) begin
	
		set @ch1 = substring(@string, @ndx, 1)
		
		-- check for doubled marker characters
		if ((@ndx + 1) <= @len) begin 
			set @ch2 = substring(@string, @ndx + 1, 1)
		
			if(@ch1 = @mc and @ch1 = @ch2)
				set @ndx = @ndx + 1
		end
			
		set @s = @s + @ch1
	
		set @ndx = @ndx + 1
	end
	
	/*
	while @string <> '' begin
	
		if substring(@string,1,1) = @ch and len(@string) > 1 and substring(@string,2,1) = @ch
			set @string = substring(@string,2,len(@string)-1)
			
		set @s = @s + substring(@string,1,1)
		set @string = substring(@string,2,len(@string)-1)
	
	end
	*/
	
	return @s
end
GO

/*

print '[' + dbo.coreRemoveQuotes(' ''Hello, there'''' asdf''
 ') + ']'
 
 print dbo.coreRemoveQuotes('"Complete the required information on your home and you will receive a complimentary Computer Analysis, indicating your home''s approximate present value on the market today. You will receive this information quickly, by email, and <span style="text-decoration:underline;">without having to speak with an agent!</span>"')
 
*/


