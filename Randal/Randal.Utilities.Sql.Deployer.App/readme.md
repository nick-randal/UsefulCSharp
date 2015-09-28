#SQL Deployer (Console Application)

The deployer executes all scripts in a project.  The deployment occurs in horizontal phases (pre, main, post) as specified in the scripts using comment tags.  Any scripts specified in the configuration file as a priority are run vertically (ignoring phase tags) to completion before moving to the next script specified.

The entire deployment is run in a transaction to avoid a partially deployed project.  Each project has a version specified and this version will be checked against a central table to avoid regression.

##Arguments
#### b:bypassCheck
Checks the scripts against provided patterns and no scripts will be deployed.  Cannot be used with noTrans.
#### c:checkOnly
Checks the scripts against provided patterns and no scripts will be deployed.  Cannot be used with noTrans.
####l:logFolder             
Directory where the log file will be written.
####n:noTrans               
Do not use a transaction to execute scripts.
####p:projectFolder         
The project folder containg the config.json an all associated SQL files.
####r:rollback              
Rollback the transaction, even if there were no errors.
####s:server                
The SQL Server that the project will be deployed against.

##Projects
Projects are a collection of SQL scripts that need to be executed together against SQL Server.  A project is organized under one parent folder but may have any number of sub-folders in any structure desired.  The single requirement is that a configuration (config.json) file be at the root of the folder structure.

####Configuration file
- Project: name of project to be deployed
- Version: YY.MM.DD.II (year, month, day, iteration)
- Priority Scripts:

```JavaScript
{
	Project: "My Script Project",
	Version: "15.09.25.01",
	PriorityScripts: [ "scriptA", "scriptB" ]
}
```

##Script tags
###Format
	--:: {tag name} {arguments}
Script tags are either immediate commands or blocks that contain SQL statements.  A block starts with the first line after the tag until the next block or end of file.
####catalog
(required)
Specify which database containers to execute against. Database container names are comma separates values (CSV) and can be any combination of literal or wildcard names. The wildcard character is the percent symbol (%).
####need
(optional)
Specify the files that need to be executed before executing the current script.  Each file name should be listed without an extension or path.
####options
(optional)
Options are specified using JSON (JavaScript Object Notation).
#####timeout
The timeout for the the current script command phases when executing against SQL Server.
{ timeout: 60 }
####ignore 
Ignore all following SQL statements until the next block or end of file.

###Phases
*Each of the following phases is optional by itself.  At least one phase is needed to execute commands against SQL Server.*
####pre
(optional)
Phase 1 and earliest possible phase of deployment. Execute all SQL statements.
####main
(optional) 
Phase 2 of deployment.
####post
(optional) 
Phase 3 and last phase of deployment.