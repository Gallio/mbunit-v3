using System;
using NAnt.Core;
using Castle.Core.Logging;

namespace MbUnit.Tasks.NAnt
{
    /// <summary>
    /// Logs messages to a Task instance.
    /// </summary>
    public class NAntLogger : LevelFilteredLogger
    {
        private readonly Task task;

        /// <summary>
        /// Creates a NAnt logger for a task.
        /// </summary>
        /// <param name="task">The NAnt task</param>
        public NAntLogger(Task task)
        {
            this.task = task;
            Level = LoggerLevel.Debug;
        }

        /// <inheritdoc />
        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
            if (exception != null)
                message += "\n" + exception;

            switch (level)
            {
                case LoggerLevel.Fatal:
                case LoggerLevel.Error:
                    task.Log(global::NAnt.Core.Level.Error, message);
                    break;

                case LoggerLevel.Warn:
                    task.Log(global::NAnt.Core.Level.Warning, message);
                    break;

                case LoggerLevel.Info:
                    task.Log(global::NAnt.Core.Level.Info, message);
                    break;

                case LoggerLevel.Debug:
                    task.Log(global::NAnt.Core.Level.Debug, message);
                    break;
            }
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            return new NAntLogger(task);
        }
    }
}
