--:: need coreTrimBrackets
--:: catalog $Catalogs$

--:: pre
exec coreCreateProcedure 'coreCreateView'
GO

--:: main
alter procedure coreCreateView
	@ViewName sysname,
	@schema sysname = 'dbo',
	@debug bit = 0
as
	DECLARE @cmd varchar(max), @schemaId int

	set @schema = isnull(@schema, 'dbo')
	set @schemaId = SCHEMA_ID(@schema)

	IF EXISTS (SELECT 1 FROM sys.views WHERE name = @ViewName and schema_id = @schemaId) begin
		return;
	end
		
	set @ViewName = dbo.coreTrimBrackets(@ViewName)
	SET @cmd = 'CREATE VIEW [' + @schema + '].[' + @ViewName + '] AS SELECT 1 ''PlaceHolder'''
	
	if(@debug = 1) begin
		print @cmd
	end else begin
		exec(@cmd)
	end
GO

/*

exec coreCreateView '[Test]', null, 1

select * from sys.views

*/