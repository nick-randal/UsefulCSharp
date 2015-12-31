--:: catalog $Catalogs$

--:: ignore
use model

--:: main
exec coreCreateQueryView 'CoreSprocStats',
'
select d.name [db], p.name [sproc], eps.last_execution_time [lastExec], eps.execution_count [execCount],
	eps.min_physical_reads [minPhysicalReads], eps.max_physical_reads [maxPhysicalReads], eps.total_physical_reads [totalPhysicalReads],
	eps.min_logical_reads [minLogicalReads], eps.max_logical_reads [maxLogicalReads], eps.total_logical_reads [totalLogicalReads],
	eps.last_elapsed_time [lastElapsedTime], eps.min_elapsed_time [minElapsedTime], eps.max_elapsed_time [maxElapsedTime], eps.total_elapsed_time [totalElapsedTime],
	eps.total_elapsed_time / eps.execution_count AS [avgElapsedTime]
from sys.dm_exec_procedure_stats eps
join sys.databases d on eps.database_id = d.database_id
join sys.procedures p on eps.object_id = p.object_id
'


/*

select top 100 * from CoreSprocStats where sproc not like 'core%' order by avgElapsedTime desc

*/