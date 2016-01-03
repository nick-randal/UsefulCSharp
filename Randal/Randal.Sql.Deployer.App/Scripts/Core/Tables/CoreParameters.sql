--:: catalog $Catalogs$

--:: pre

declare @table_id int, @cmd varchar(max)

set @table_id = object_id('CoreParameters')

if not exists (select 1 from sys.columns where object_id = @table_id and name = 'ParamValue') begin

	-- drop the table if it already exists
	if(@table_id is not null) 
		drop table CoreParameters

	CREATE TABLE CoreParameters (
		GroupName			varchar(32)		not null,
		ParamName			varchar(32)		not null,
		ParamValue			sql_variant		null,
		primary key clustered (GroupName, ParamName)
	)
	
end


/*

print dbo.coreGetParameterString('TableVersion', '')

select * from sys.columns where object_id = object_id('CoreParameters') and (name = 'ParamValue')

select * from CoreParameters
*/
