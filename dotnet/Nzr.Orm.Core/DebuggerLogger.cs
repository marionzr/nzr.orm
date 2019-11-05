using Microsoft.Extensions.Logging;
using System;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Dummy implementation to log messages on output window.
    /// </summary>
    public class DebuggerLogger : ILogger, IDisposable
    {
        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state) => this;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the connection resource.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Cleanup
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DebuggerLogger()
        {
            Dispose(false);
        }

        /// <summary>
        /// Checks if the given logLevel is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked</param>
        /// <returns>True if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel) =>
#if DEBUG
            true;
#endif
#if RELEASE
            false;
#endif

        /// <summary>
        /// Writes a log entry in the output Console.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) => System.Diagnostics.Debug.WriteLine(state);
    }
}
