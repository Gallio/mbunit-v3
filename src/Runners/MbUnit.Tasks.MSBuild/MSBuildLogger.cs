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
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Castle.Core.Logging;
using ILogger=Castle.Core.Logging.ILogger;

namespace MbUnit.Tasks.MSBuild
{
    /// <summary>
    /// Logs messages to a <see cref="TaskLoggingHelper" /> instance.
    /// </summary>
    public class MSBuildLogger : LevelFilteredLogger
    {
        private readonly TaskLoggingHelper taskLogger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MSBuildLogger" /> class.
        /// </summary>
        /// <param name="taskLogger">The <see cref="TaskLoggingHelper" /> instance to channel
        /// log messages to.</param>
        public MSBuildLogger(TaskLoggingHelper taskLogger)
        {
            if (taskLogger == null)
                throw new ArgumentNullException("taskLogger");
            this.taskLogger = taskLogger;
            Level = LoggerLevel.Debug;
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
            if (exception != null)
                message += "\n" + exception;

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
                    taskLogger.LogMessage(MessageImportance.Normal, message);
                    break;

                case LoggerLevel.Debug:
                    taskLogger.LogMessage(MessageImportance.Low, message);
                    break;
            }
        }

        /// <inheritdoc />
        public override ILogger CreateChildLogger(string name)
        {
            //TODO: Check why are we ignoring the name
            return new MSBuildLogger(taskLogger);
        }
    }
}
