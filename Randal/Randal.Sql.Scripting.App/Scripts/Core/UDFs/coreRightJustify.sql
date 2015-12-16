--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateFunction 'coreRightJustify', 'dbo', 'scalar'
GO

--:: main
alter function coreRightJustify( @input varchar(max), @char char(1), @newLen smallint )
returns varchar(max)
AS
BEGIN
	declare @len smallint, @pad smallint, @rem smallint, @sub smallint
	
	set @len = len(rtrim(@input))
	set @pad = @newLen - @len
	
	if(@char = char(9)) begin
		set @rem = @newLen % 4    
		set @pad = @newLen / 4
		
		if @rem > 0
			set @pad = @pad + 1
	
		set @sub = @len / 4
		if (@len % 4) = 3
			set @sub = @sub + 1
	
		set @pad = @pad - @sub
	end
	
	if @pad <= 0
		return @input
	
	set @input = @input + replicate(@char, @pad)
	
	return @input
END
GO

/*

declare @char char(1), @len tinyint

set @char = char(9)
set @len = 9

print '[' + dbo.coreRightJustify('1', @char, @len) + ']'
print '[' + dbo.coreRightJustify('12', @char, @len) + ']'
print '[' + dbo.coreRightJustify('123', @char, @len) + ']'
print '[' + dbo.coreRightJustify('1234', @char, @len) + ']'
print '[' + dbo.coreRightJustify('12345', @char, @len) + ']'
print '[' + dbo.coreRightJustify('123456', @char, @len) + ']'
print '[' + dbo.coreRightJustify('1234567', @char, @len) + ']'
print '[' + dbo.coreRightJustify('12345678', @char, @len) + ']'
print '[' + dbo.coreRightJustify('123456789', @char, @len) + ']'

*/