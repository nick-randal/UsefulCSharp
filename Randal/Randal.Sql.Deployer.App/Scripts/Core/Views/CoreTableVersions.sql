--:: catalog $Catalogs$
--:: need CoreParameters

--:: ignore
use model

--:: main

EXEC coreCreateQueryView 'CoreTableVersions', 
'
SELECT ParamName [Table], ParamValue [Version] 
FROM dbo.CoreParameters where GroupName = ''TableVersion''
'

/*

SELECT * FROM CoreTableVersions

*/