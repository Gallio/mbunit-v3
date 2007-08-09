using System;
using Microsoft.Build.Utilities;
using Castle.Core.Logging;

namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// Logs messages to a TaskLoggingHelper instance.
    /// </summary>
    public class MSBuildLogger : ConsoleLogger
    {
        private readonly TaskLoggingHelper taskLogger;
        
        /// <summary>
        /// Initializes a new instance of the MSBuildLogger class.
        /// </summary>
        /// <param name="taskLogger">The TaskLoggingHelper instance to channel
        /// log messages to.</param>
        public MSBuildLogger(TaskLoggingHelper taskLogger)
        {
            this.taskLogger = taskLogger;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="name">Not used.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception to log (it can be null).</param>
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            switch (level)
            {
                case LoggerLevel.Fatal:
                case LoggerLevel.Error:
                    taskLogger.LogError(message);
                    break;

                case LoggerLevel.Warn:
                    taskLogger.LogWarning(message);
                    break;

                case LoggerLevel.Info:
                case LoggerLevel.Debug:
                    taskLogger.LogMessage(message);
                    break;
            }

            if (exception != null)
                taskLogger.LogError(exception.ToString());
        }
    }
}
