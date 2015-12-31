--:: catalog $Catalogs$

--:: ignore
use model


--:: pre
exec coreCreateFunction 'corePivotDelimitedList', 'dbo', 'table'
GO

--:: main
alter function corePivotDelimitedList(@ids nvarchar(max), @delim nchar(1))

	returns @t table 
	( 
		value		nvarchar(128)	not null,
		ordinal		int				not null
		primary key (value) 
	)
	
begin  
	declare @index 	smallint, @str 	varchar(64), @ordinal int
	 
	set @ordinal = 1
	while @ids <> '' begin  
	
		set @index = charindex(@delim, @ids)
		
		if @index > 0 begin
			set @str = left(@ids, @index - 1)  
			set @ids = right(@ids, len(@ids) - @index)  
		end else begin  
			set @str = @ids  
			set @ids = ''  
		end

		insert @t select ltrim(rtrim(@str)), @ordinal
		set @ordinal = @ordinal + 1
	end
	
	return 
end 

/*

select * from dbo.corePivotDelimitedList('{9B746215-E92D-490A-8A11-0024E4200A5F}|{B1A763CF-5155-4CCF-9A8C-04E897E8FE73}|{E2BE46E5-5575-48B6-BBAC-385E8B1BD5C0}|{1C50F3BF-61AC-496D-AE4A-46D94BD8F434}|{0472797B-D59B-4CBD-987D-F21A1284B69D}|{F0C88028-EC33-4C12-9C56-71EA105BCC51}|{5A1BE8D7-E5F2-4F44-BF8A-AE72CD22566D}', '|')

*/