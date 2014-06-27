Asynchronous file logger and other supported logger types.  
Extensible interface to modify funtionality at any stage of the logging process.

- Asynchronous file logger
- Contiguous log file based on settings size, the entire log file will be allocated on creation
- Automatic cutover to new file when size limit reached
- String logger
- Text Writer logger
- Null logger
- Extensible formatting
- Optional verbosity filtering to quickly increase or decrease log output

```csharp
// simple file log example
var settings = new FileLoggerSettings(@"C:\Logs", "Test", 4194304, true);
var log = new AsyncFileLogger(settings);

log.Add(new LogEntry("Hello log"));   // standard date and time stamping
log.Add(new LogEntryNoTimestamp("an entry without standard timestamping"));

log.Dispose();
```
