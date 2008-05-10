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
using Gallio.Runtime.Logging;
using NAnt.Core;

namespace Gallio.NAntTasks
{
    /// <exclude />
    /// <summary>
    /// An <see cref="ILogger" /> implementation that logs messages to a <see cref="Task" /> object.
    /// </summary>
    internal class TaskLogger : BaseLogger
    {
        private readonly Task task;

        public TaskLogger(Task task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            this.task = task;
        }

        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            if (exception != null)
                message += "\n" + exception;

            switch (severity)
            {
                case LogSeverity.Error:
                    task.Log(Level.Error, message);
                    break;

                case LogSeverity.Warning:
                    task.Log(Level.Warning, message);
                    break;

                case LogSeverity.Info:
                    task.Log(Level.Info, message);
                    break;

                case LogSeverity.Debug:
                    task.Log(Level.Debug, message);
                    break;
            }
        }
    }
}
