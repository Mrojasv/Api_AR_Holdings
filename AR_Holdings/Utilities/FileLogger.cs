
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AR_Holdings.Utilities
{
    public class FileLogger : ILogger
    {
        private string filePath;
        private static object _lock = new object();
        public FileLogger(string path)
        {
            filePath = path;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //return logLevel == LogLevel.Trace;
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter != null)
            {
                lock (_lock)
                {
                    string fullFilePath = Path.Combine(filePath, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
                    var n = Environment.NewLine;
                    string exc = "";
                    if (exception != null) exc = exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                    File.AppendAllText(fullFilePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
                }
            }
        }
    }
}
