using System;
using NAnt.Core;
using Castle.Core.Logging;

namespace MbUnit.Tasks.NAnt
{
    internal class NantLogger : ConsoleLogger
    {
        private readonly Task task;

        public NantLogger(Task task)
        {
            this.task = task;
        }

        protected override void Log(LoggerLevel level, string name, string message, Exception exception)
        {
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

            if (exception != null)
                task.Log(global::NAnt.Core.Level.Error, exception.ToString());
        }
    }
}
