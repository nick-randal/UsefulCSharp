--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreSetTableIndex'
GO

--:: main
alter procedure coreSetTableIndex
    @table			varchar(128),
    @name			varchar(128),
    @index			varchar(1000) = null,
    @clustering		varchar(128) = 'NONCLUSTERED',   -- [UNIQUE] CLUSTERED or NONCLUSTERED
	@include		varchar(1000) = null,
	@where			varchar(1000) = null

as begin
	set nocount on

	declare @indexname sysname, @id int, @cmd varchar(max)

	set @indexname = 'ix_' + @table + '_' + @name

	select @id = [object_id] from sys.indexes where [name] = @indexname
	if(@id is not null)
		exec('drop index [' + @indexname + '] on [' + @table + ']')

	if(@index is null or @clustering is null) begin
		raiserror('an index name and clustering must be specified.', 10, 1)
		return -1
	end

	set @cmd = 'create ' + @clustering 
		+ ' index [' + @indexname + '] on [' + @table + '] (' + @index + ')'

	if (@include is not null) begin
		set @cmd = @cmd + ' include ( ' + @include + ' )'
	end
	
	if @where is not null begin
	
		set @cmd = @cmd + ' where ' + @where
	
	end

	set @cmd = @cmd + ';'
	--print @cmd
	
	begin try
		exec (@cmd)
	end try
	begin catch
	
		raiserror('coreSetTableIndex failed : %s', 17, 1, @cmd)
	end catch

	return -1
end

/*

select * from sys.indexes where [name] LIKE 'IX%'

EXEC coreSetTableIndex 'Companies', 'State', 'CompanyState'

EXEC coreSetTableIndex 'Companies', 'State', 'CompanyState', DEFAULT, 1

*/
