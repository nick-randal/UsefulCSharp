--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreDropIndex'
GO

--:: main
alter procedure coreDropIndex
    @TableName			SYSNAME,
	@IndexName			SYSNAME,
	@FailOnNotExist		BIT = 1,
	@Debug				BIT = 0

AS BEGIN
	set nocount on

	declare @cmd varchar(1000)

	set @IndexName = dbo.coreTrimBrackets(@IndexName)
	set @TableName = dbo.coreTrimBrackets(@TableName)

	if(@TableName IS NULL OR @IndexName IS NULL) begin
		raiserror('Must specify a table name and a constraint name when trying to drop a constraint.', 16, 1)
		return 0
	end
	

	if not exists (select [name] from sys.Indexes where object_id = object_id(@TableName) and [name] = @IndexName) begin
	
		if(@FailOnNotExist = 1) begin
			raiserror('The index ''%s'' attempting to be dropped, does not exist.', 16, 1, @IndexName)
			return 0
		end else begin
			return -1
		end

	end
	
	IF(@Debug = 1)
		set @cmd = '!exec !print !rollback'
	else
		set @cmd = '!exec'
	
	exec coreFormat 'drop index [$01] on [$02]', @cmd, @IndexName, @TableName

	set nocount off

	return -1
END

/*

EXEC coreDropIndex 'SettlementBatches', 'IX_SettlementBatches_CompanyStore', DEFAULT, 1

select object_name(object_id) from sys.indexes

*/