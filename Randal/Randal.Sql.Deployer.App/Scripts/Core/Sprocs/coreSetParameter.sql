
--:: catalog $Catalogs$

--:: pre
EXEC coreCreateProcedure 'coreSetParameter'
GO

--:: main
ALTER PROCEDURE coreSetParameter
	@group		varchar(32),
	@param		varchar(32),
	@value		sql_variant
as
	
	if(@group is null or @param is null)
		return 0

	merge CoreParameters dst
	using 
	( 
		values (@group, @param)
	) as src(groupName, paramName)
	
	on dst.GroupName = src.groupName and dst.ParamName = src.paramName
	
	when matched then
		update
			set ParamValue = @value
			
	when not matched by target then
		insert (GroupName, ParamName, ParamValue) values (@group, @param, @value)
	;


	RETURN 1
GO

/*

helpme CoreParameters

EXEC coreSetParameter 'TestGroup', 'Test', 'Blah Blah'
EXEC coreSetParameter 'TestGroup', 'Test', 'Blah Blah2'

select * from CoreParameters

*/