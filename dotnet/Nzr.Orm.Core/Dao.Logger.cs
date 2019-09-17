using Microsoft.Extensions.Logging;
using Nzr.Orm.Core.Sql;
using System;
using System.Collections;
using System.Linq;

namespace Nzr.Orm.Core
{
    /// <summary>
    /// Partial class Dao.
    /// Contains all methods related to logging.
    /// </summary>
    public partial class Dao
    {
        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example: "User {User}logged in from {Address}"</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogCritical(string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogCritical(message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogCritical(Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogCritical(exception, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogCritical(EventId eventId, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogCritical(eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a critical log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogCritical(EventId eventId, Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogCritical(eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogDebug(EventId eventId, Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogDebug(eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogDebug(EventId eventId, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogDebug(eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogDebug(Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogDebug(exception, message, args);
        }

        /// <summary>
        /// Formats and writes a debug log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogDebug(string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogDebug(message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogError(string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogError(message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogError(Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogError(exception, message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogError(EventId eventId, Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogError(eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogError(EventId eventId, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogError(eventId, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogInformation(EventId eventId, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogInformation(eventId, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogInformation(Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogInformation(exception, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogInformation(EventId eventId, Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogInformation(eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes an informational log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogInformation(string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogInformation(message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogTrace(string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogTrace(message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogTrace(Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogTrace(exception, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogTrace(EventId eventId, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogTrace(eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogTrace(EventId eventId, Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogTrace(eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogWarning(EventId eventId, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogWarning(eventId, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="eventId">The event id associated with the log.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogWarning(EventId eventId, Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogWarning(eventId, exception, message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogWarning(string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogWarning(message, args);
        }

        /// <summary>
        /// Formats and writes a warning log message.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="message">Format string of the log message in message template format. Example: logged in from {Address}".</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void LogWarning(Exception exception, string message, params object[] args)
        {
            ILogger logger = Options.Logger;

            if (logger == null)
            {
                return;
            }

            logger.LogWarning(exception, message, args);
        }

        private void LogOperation(string sql, Parameters parameters) => LogDebug("Executing...\r\n\tSQL: {sql}\r\n\tParameters: {parameters}", sql, string.Join(", ", parameters.Select(p => $"{p.Item1} = {p.Item2}").ToList()));

        private void LogOperation(string sql, Parameters parameters, dynamic result) => LogInformation("Executed\r\n\tSQL: {sql}\r\n\tParameters: {parameters}\r\n\tResult: {result}", sql, string.Join(", ", parameters.Select(p => $"{p.Item1} = {p.Item2}").ToList()), result);

        private void LogOperation(string sql, Parameters parameters, IList results) => LogInformation("Executed\r\n\tSQL: {sql}\r\n\tParameters: {parameters}\r\n\tResult: {result}", sql, string.Join(", ", parameters.Select(p => $"{p.Item1} = {p.Item2}").ToList()), string.Join(",", results));

        private void LogError(Exception e, string sql, Parameters parameters)
        {
            LogError("Error Executing\r\n\tSQL: {sql}\r\n\tParameters: {parameters}", sql, string.Join(", ", parameters.Select(p => $"{p.Item1} = {p.Item2}").ToList()));
            LogError(e, e.Message);
        }
    }
}