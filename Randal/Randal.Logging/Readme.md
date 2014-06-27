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
- Support for dependency injection
- Works well with IOC containers

Example 1 - file logger
```csharp
var settings = new FileLoggerSettings(@"C:\Logs", "Test", 4194304, true);
var log = new AsyncFileLogger(settings);

log.Add(new LogEntry("Hello log"));   // standard date and time stamping
log.Add(new LogEntryNoTimestamp("an entry without standard timestamping"));

log.Dispose();
```

Example 2 - file logger with string format decorator
```csharp
var settings2 = new FileLoggerSettings(@"C:\__dev\Research\Log", "Test2", 2048, true);
var log2 = new LoggerStringFormatDecorator(new AsyncFileLogger(settings));

log2.AddBlank();
log2.AddEntry("Hello log");
log2.AddEntryNoTimestamp("an entry without standard timestamping");

log2.Dispose();
```
