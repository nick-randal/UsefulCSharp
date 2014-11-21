--:: catalog %

--:: main

EXEC coreCreateQueryView 'TableVersions', 
'
SELECT ParamName [Table], ParamValue [Version] 
FROM CoreParameters where GroupName = ''TableVersion''
'

/*

SELECT * FROM TableVersions

*/