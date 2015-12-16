--:: catalog $Catalogs$
--:: need coreTypeDef

--:: ignore
use model

--:: pre
EXEC coreCreateFunction 'coreDataTypeName', 'dbo', 'scalar'
GO

--:: main
ALTER FUNCTION coreDataTypeName(@object_id INT, @column_id INT)

	returns VARCHAR(100)
AS
BEGIN

	DECLARE @DataName VARCHAR(100)

	select 
		@DataName = dbo.coreTypeDef(c.user_type_id, c.max_length, c.precision, c.scale)
		--case 
		--	when t.is_user_defined = 1 then t.Name
		--	when c.system_type_id IN (165, 167, 175, 231, 239) and c.max_length <> -1 THEN t.Name + '(' + convert(varchar, c.max_length) + ')'
		--	when c.system_type_id IN (165, 167, 175, 231, 239) then t.Name + '(max)'
		--	when c.system_type_id IN (60, 106, 108, 122) then t.Name + '(' + convert(varchar, c.precision) + ', ' + convert(varchar, c.scale) + ')'
		--else t.name
		--end

	FROM sys.columns c
		JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.user_type_id = c.user_type_id
	WHERE c.object_id = @object_id AND c.column_id = @column_id

	RETURN @DataName
END
GO

/*

SELECT dbo.coreDataTypeName(object_id, column_id) FROM sys.Columns WHERE dbo.coreDataTypeName(object_id, column_id) like 'num%'

select * from sys.types


*/

