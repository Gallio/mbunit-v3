// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Runtime.Logging;

namespace Gallio.MSBuildTasks
{
    /// <exclude />
    /// <summary>
    /// Logs messages to a <see cref="TaskLoggingHelper" /> instance.
    /// </summary>
    internal class TaskLogger : BaseLogger
    {
        private readonly TaskLoggingHelper taskLoggingHelper;
        
        public TaskLogger(TaskLoggingHelper taskLoggingHelper)
        {
            if (taskLoggingHelper == null)
                throw new ArgumentNullException("taskLoggingHelper");

            this.taskLoggingHelper = taskLoggingHelper;
        }

        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            if (exception != null)
                message += "\n" + exception;

            switch (severity)
            {
                case LogSeverity.Error:
                    taskLoggingHelper.LogError(message);
                    break;

                case LogSeverity.Warning:
                    taskLoggingHelper.LogWarning(message);
                    break;

                case LogSeverity.Important:
                    taskLoggingHelper.LogMessage(MessageImportance.High, message);
                    break;

                case LogSeverity.Info:
                    taskLoggingHelper.LogMessage(MessageImportance.Normal, message);
                    break;

                case LogSeverity.Debug:
                    taskLoggingHelper.LogMessage(MessageImportance.Low, message);
                    break;
            }
        }
    }
}
