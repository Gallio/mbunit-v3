// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
