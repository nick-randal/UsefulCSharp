--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreIncrStr', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreIncrStr(@input varchar(20))
	RETURNS varchar(20)
AS BEGIN
	
	declare @len int, @pos int, @carry bit, @letter int
	
	set @input = rtrim(ltrim(@input))
	set @len = len(@input)
	set @pos = @len
	
	-- at least go through one time
	set @carry = 1
	set @input = upper(@input)
	
	while(@pos > 0 and @carry = 1) begin
	
		set @letter = ascii(substring(@input, @pos, 1))
		set @letter = @letter + 1
		set @carry = 0
			
		if(@letter between 48 and 58) begin		

			if @letter = 57
				set @letter = 65

		end else if(@letter between 65 and 91) begin
			
			if(@letter = 91) begin
				set @letter = 48
				set @carry = 1
			end 
			
		end else begin
			
			set @letter = 48
			
		end
	
		set @input = left(@input, @pos - 1) + char(@letter) + right(@input, @len - @pos)
	
		set @pos = @pos - 1
	end
	
	if(@carry = 1)
		set @input = 'a' + @input
	
	RETURN @input
END

GO

/*

declare @c int, @val varchar(10)
set @c = 1
set @val = '_'
while @c < 1000000 begin
	set @val = dbo.coreIncrStr(@val)
	print @val
	set @c = @c + 1
end

*/