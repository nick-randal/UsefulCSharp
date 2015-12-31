--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec corecreatefunction 'coreLevenshtein', 'dbo', 'scalar'
GO

--:: main
alter function coreLevenshtein(@left varchar(500), @right varchar(500))
returns int
as
begin
	
	declare @difference int, @lenRight int, @lenLeft int, @leftIndex int, @rightIndex int,   @left_char char(1), @right_char char(1), @compareLength int  
	
	SET @lenLeft = LEN(@left) 
	SET @lenRight = LEN(@right) 
	SET @difference = 0  
	
	if @lenLeft = 0 begin 
	  SET @difference = @lenRight GOTO done 
	END 
	
	if @lenRight = 0 begin 
	  SET @difference = @lenLeft 
	  GOTO done 
	END
	
	GOTO comparison  

	comparison: 
	IF (@lenLeft >= @lenRight) 
	  SET @compareLength = @lenLeft 
	Else 
	  SET @compareLength = @lenRight  
	
	SET @rightIndex = 1 
	SET @leftIndex = 1 
	
	WHILE @leftIndex <= @compareLength BEGIN 
	
	  SET @left_char = substring(@left, @leftIndex, 1)
	  SET @right_char = substring(@right, @rightIndex, 1)
	  IF @left_char <> @right_char BEGIN -- Would an insertion make them re-align? 
	  
		 IF(@left_char = substring(@right, @rightIndex+1, 1))    
			SET @rightIndex = @rightIndex + 1 
		 -- Would an deletion make them re-align? 
		 ELSE 
			IF(substring(@left, @leftIndex+1, 1) = @right_char)
			   SET @leftIndex = @leftIndex + 1
			   SET @difference = @difference + 1 
	  END
	  
	  SET @leftIndex = @leftIndex + 1 
	  SET @rightIndex = @rightIndex + 1 
	END
	GOTO done  

	done:
		RETURN @difference
end

GO

/*

PRINT dbo.coreLevenshtein('', '')
PRINT dbo.coreLevenshtein('Nick', 'Nikc')
PRINT dbo.coreLevenshtein('This is a test', 'this is a tst')

declare @a varchar(100), @b varchar(100)
set @a = replicate('abcdefg', 20) set @b = replicate('gfedcba', 20)
PRINT dbo.coreLevenshtein(@a, @b)

*/