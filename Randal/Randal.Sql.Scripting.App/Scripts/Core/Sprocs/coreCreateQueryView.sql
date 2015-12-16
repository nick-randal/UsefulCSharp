--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreCreateQueryView'
GO

--:: main
ALTER PROCEDURE coreCreateQueryView
    @ViewName		sysname,
	@Query			varchar(max),
	@schemaBinding	bit = 0,
	@debug			bit = 0
AS BEGIN

	DECLARE @cmd varchar(max)

	IF NOT EXISTS (SELECT 1 FROM sys.views where name = @ViewName)
		SET @cmd = 'CREATE '
	ELSE
		SET @cmd = 'ALTER '

	SET @cmd = @cmd + 
		'VIEW [dbo].[' + @ViewName + '] '
	
	if @schemaBinding = 1
		set @cmd = @cmd + 'with schemabinding '
		
	set @cmd = @cmd + ' AS ' + @Query

	if @debug = 1
		print @cmd
	else
		EXEC(@cmd)

END	
GO

/*


*/