
--:: catalog $Catalogs$

--:: pre
EXEC coreCreateProcedure 'coreRemoveParameter'
GO

--:: main
ALTER PROCEDURE coreRemoveParameter
	@group		varchar(32),
	@param		varchar(32)
as
	
	IF(@group is null or @param is null)
		RETURN 0

	DELETE CoreParameters WHERE GroupName = @group and ParamName = @param

	RETURN 1
GO

/*

EXEC coreRemoveParameter 'Test'

*/