Asynchronous file logger and other supported logger types.  
Extensible interface to modify funtionality at any stage of the logging process.

- Asynchronous file logger
- Contiguous log file based on initialized size
- Automatic cutover to new file when size limit reached
- String logger
- Text Writer logger
- Extensible formatting
- Optional verbosity filtering to quickly increase or decrease log output

'''csharp
// simple file log example
var settings = new FileLoggerSettings(@"C:\Logs", "Test", 4194304, true);
var log = new AsyncFileLogger(settings);

log.Add(new LogEntry("Hello log"));

log.Dispose();
'''
