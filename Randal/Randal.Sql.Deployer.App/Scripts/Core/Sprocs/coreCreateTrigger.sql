--:: catalog $Catalogs$

--:: pre
EXEC coreCreateProcedure 'coreCreateTrigger'
GO

--:: main
ALTER PROCEDURE coreCreateTrigger
	@TableName		sysname,
	@SchemaName		sysname,
	@TriggerName	sysname,
	@TriggerType	sysname,
	@Trigger		nvarchar(max),
	@debug			bit = 0

as begin

	declare @FQName varchar(256), @cmd nvarchar(max)

	set @SchemaName = dbo.coreTrimBrackets(@SchemaName)
	set @TableName = dbo.coreTrimBrackets(@TableName)
	set @TriggerName = dbo.coreTrimBrackets(@TriggerName)

	set @FQName = '[' + @SchemaName + '].[TRG_' + @TableName + '_' + @TriggerName + ']'
	
	SET @cmd = 
	'IF OBJECT_ID (''' + @FQName + ''',''TR'') IS NOT NULL
	   DROP TRIGGER ' + @FQName 
	   
	if @debug = 1 begin
		print @cmd
	end
	else begin
		exec(@cmd)
	end

	SET @cmd = 'CREATE TRIGGER ' + @FQName + ' ON [' + @SchemaName + '].[' + @TableName + '] ' + @TriggerType + ' AS ' + @Trigger
	
	if @debug = 1 begin
		PRINT @cmd	
	end 
	else begin
		exec(@cmd)
	end

	return -1
end



/*
DECLARE @return INT
EXEC @return = coreCreateTrigger 'TestCodes', 'Update', 'AFTER INSERT, UPDATE',
	'SET NOCOUNT ON
	IF(UPDATE(Code))
	BEGIN
	IF EXISTS(SELECT * FROM deleted)
		ROLLBACK
	ELSE IF EXISTS(SELECT * FROM inserted)
		UPDATE TestCodes SET Code = UPPER(LTRIM(RTRIM(inserted.Code))) FROM inserted WHERE inserted.Code = TestCodes.Code
	END
	SET NOCOUNT OFF'
PRINT @return

EXEC @return = coreCreateTrigger 'TestCodes', 'Delete', 'INSTEAD OF DELETE',
	'	UPDATE TestCodes SET Deprecated = 1 FROM deleted WHERE deleted.Code = TestCodes.Code
	'
PRINT @return
*/