// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using NAnt.Core;

namespace Gallio.NAntTasks
{
    /// <exclude />
    /// <summary>
    /// An <see cref="ILogger" /> implementation that logs messages to a <see cref="Task" /> object.
    /// </summary>
    internal class TaskLogger : LevelFilteredLogger
    {
        private readonly Task task;

        public TaskLogger(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this.task = task;
            Level = LoggerLevel.Debug;
        }

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

        public override ILogger CreateChildLogger(string name)
        {
            return new TaskLogger(task);
        }
    }
}
