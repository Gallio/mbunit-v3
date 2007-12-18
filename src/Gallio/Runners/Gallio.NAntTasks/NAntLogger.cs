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
using Castle.Core.Logging;

namespace Gallio.NAntTasks
{
    /// <exclude />
    /// <summary>
    /// An ILogger implementation that logs messages to a INAntLogger object.
    /// </summary>
    internal class NAntLogger : LevelFilteredLogger
    {
        private readonly INAntLogger task;

        /// <summary>
        /// Initializes a new instance of the NAntLogger class using
        /// a custom INAntLogger instance.
        /// </summary>
        /// <param name="task">The INAntLogger where the messages will be channeled
        /// to.</param>
        public NAntLogger(INAntLogger task)
        {
            if (task == null)
                throw new ArgumentNullException("task");
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
