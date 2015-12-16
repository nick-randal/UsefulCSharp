/*
 Helps format a query statement using place holders for the values in the form of $01, $02... $30.
 All double quotes will be converted to single quotes.

 Optional commands can be passed into the @formatted output variable.
 !exec		- executes the statement after formatting
 !print		- prints the statement after formatting
 !rollback	- executes the statement in a transaction and performs a rollback
 */

--:: catalog $Catalogs$

--:: ignore
use model

--:: pre
exec coreCreateProcedure 'coreFormat'
GO

--:: main
alter procedure coreFormat
	@template		nvarchar(max),
	@formatted		nvarchar(max) output,
	@v1				sql_variant = null,
	@v2				sql_variant = null,
	@v3				sql_variant = null,
	@v4				sql_variant = null,
	@v5				sql_variant = null,
	@v6				sql_variant = null,
	@v7				sql_variant = null,
	@v8				sql_variant = null,
	@v9				sql_variant = null,
	@v10			sql_variant = null,
	@v11			sql_variant = null,
	@v12			sql_variant = null,
	@v13			sql_variant = null,
	@v14			sql_variant = null,
	@v15			sql_variant = null,
	@v16			sql_variant = null,
	@v17			sql_variant = null,
	@v18			sql_variant = null,
	@v19			sql_variant = null,
	@v20			sql_variant = null,
	@v21			sql_variant = null,
	@v22			sql_variant = null,
	@v23			sql_variant = null,
	@v24			sql_variant = null,
	@v25			sql_variant = null,
	@v26			sql_variant = null,
	@v27			sql_variant = null,
	@v28			sql_variant = null,
	@v29			sql_variant = null,
	@v30			sql_variant = null
	
as begin

	declare @isPrint bit, @isExec bit, @isRollback bit, @null char(6)
	
	set @null = '{null}'
	set @isPrint = 0
	set @isExec = 0
	set @isRollback = 0
	
	-- if the @formatted variable is not null then evaluate the option commands that it might contain
	if @formatted is not null and left(@formatted, 1) = '!' begin
		if charindex('print', @formatted) > 0 set @isPrint = 1
		if charindex('exec', @formatted) > 0 set @isExec = 1
		if charindex('rollback', @formatted) > 0 set @isRollback = 1
	end
	
	--print @isPrint
	--print @isExec
	--print @isRollback

	set @template = replace(@template, '$01', convert( varchar(max), isnull(@v1, @null) ) )
	set @template = replace(@template, '$02', convert( varchar(max), isnull(@v2, @null) ) )
	set @template = replace(@template, '$03', convert( varchar(max), isnull(@v3, @null) ) )
	set @template = replace(@template, '$04', convert( varchar(max), isnull(@v4, @null) ) )
	set @template = replace(@template, '$05', convert( varchar(max), isnull(@v5, @null) ) )
	set @template = replace(@template, '$06', convert( varchar(max), isnull(@v6, @null) ) )
	set @template = replace(@template, '$07', convert( varchar(max), isnull(@v7, @null) ) )
	set @template = replace(@template, '$08', convert( varchar(max), isnull(@v8, @null) ) )
	set @template = replace(@template, '$09', convert( varchar(max), isnull(@v9, @null) ) )
	set @template = replace(@template, '$10', convert( varchar(max), isnull(@v10, @null) ) )
	set @template = replace(@template, '$11', convert( varchar(max), isnull(@v11, @null) ) )
	set @template = replace(@template, '$12', convert( varchar(max), isnull(@v12, @null) ) )
	set @template = replace(@template, '$13', convert( varchar(max), isnull(@v13, @null) ) )
	set @template = replace(@template, '$14', convert( varchar(max), isnull(@v14, @null) ) )
	set @template = replace(@template, '$15', convert( varchar(max), isnull(@v15, @null) ) )
	set @template = replace(@template, '$16', convert( varchar(max), isnull(@v16, @null) ) )
	set @template = replace(@template, '$17', convert( varchar(max), isnull(@v17, @null) ) )
	set @template = replace(@template, '$18', convert( varchar(max), isnull(@v18, @null) ) )
	set @template = replace(@template, '$19', convert( varchar(max), isnull(@v19, @null) ) )
	set @template = replace(@template, '$20', convert( varchar(max), isnull(@v20, @null) ) )
	set @template = replace(@template, '$21', convert( varchar(max), isnull(@v21, @null) ) )
	set @template = replace(@template, '$22', convert( varchar(max), isnull(@v22, @null) ) )
	set @template = replace(@template, '$23', convert( varchar(max), isnull(@v23, @null) ) )
	set @template = replace(@template, '$24', convert( varchar(max), isnull(@v24, @null) ) )
	set @template = replace(@template, '$25', convert( varchar(max), isnull(@v25, @null) ) )
	set @template = replace(@template, '$26', convert( varchar(max), isnull(@v26, @null) ) )
	set @template = replace(@template, '$27', convert( varchar(max), isnull(@v27, @null) ) )
	set @template = replace(@template, '$28', convert( varchar(max), isnull(@v28, @null) ) )
	set @template = replace(@template, '$29', convert( varchar(max), isnull(@v29, @null) ) )
	set @template = replace(@template, '$30', convert( varchar(max), isnull(@v30, @null) ) )
																						    
	set @formatted = replace(@template, '"', '''')										    
																						    
	print @formatted

	return -1
end

/*

exec coreFormat 'select * from $01 where $02 not in (select $02 from $03)', 
	'print exec', 
	'lead', 'id_agency', 
	'agency'


exec coreFormat 'select top 100 * from $01',
	'print exec', 
	'lead'

exec coreFormat 'update $01 set $02 = "$03" where $04 = $05',
	'print exec', 
	'lead', 'last_name', 'randal', 'id_lead', 1052169
	
select * from lead where id_lead = 1052169


declare @t varchar(1000) = 'test1'

exec coreFormat '$01 $02', @t output, @t, 'test2'
exec coreFormat '$01 $02', @t output, @t, 'test3'

print @t


exec coreFormat '[ $01 $02 $03 $04 $05 $06 $07 $08 $09 $10 $11 $12 $13 $14 $15 $16 $17 $18 $19 $20 $21 $22 $23 $24 $25 $26 $27 $28 $29 $30 ]', '!print'

*/