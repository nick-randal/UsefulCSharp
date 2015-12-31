--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreDropFunction'
GO

--:: main
alter procedure coreDropFunction
    @Function		SYSNAME,
	@debug			bit = 0
as

	declare @cmd varchar(max), @type char(2)

	select @type = type from sys.objects where (type = 'IF' or type = 'FN' or type = 'TF') and name = @Function

	-- Drop stored procedure if it already exists
	if(@type is not null) begin
		
		set @cmd = 'DROP FUNCTION ' + @Function
		
		if(@debug = 1) begin
			print '---------- Drop function : type ' + @type
			print @cmd
		end else begin
			exec(@cmd)
		end
		
	end
GO

/*

EXEC coreDropFunction 'someFunc'

*/