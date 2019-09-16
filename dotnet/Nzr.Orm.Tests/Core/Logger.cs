using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Nzr.Orm.Tests.Core
{
    public class Logger : ILogger
    {
        public string Context { get; }

        public Logger(string context) => Context = context;


        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => Debug.WriteLine(state);
    }
}
