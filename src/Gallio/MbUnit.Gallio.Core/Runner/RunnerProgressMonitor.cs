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

using Castle.Core.Logging;
using MbUnit.Core.Runner;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Implementation of a IProgressMonitor that logs messages to a ConsoleLogger
    /// instance.
    /// </summary>
    public class RunnerProgressMonitor : TextualProgressMonitor
    {
        private readonly ILogger tddLogger = null;

        /// <summary>
        /// Initializes a new instance of the RunnerProgressMonitor class.
        /// </summary>
        /// <param name="logger">A logger instance where log messages will be
        /// channeled to.</param>
        public RunnerProgressMonitor(ILogger logger)
        {
            tddLogger = logger;
        }

        private string previousTaskName = string.Empty;

        /// <inheritdoc />
        protected override void UpdateDisplay()
        {
            // We can't show progress in a convenient way when running 
            // within Visual Studio, so just inform when a new task
            // has begun.
            if (previousTaskName.CompareTo(TaskName) != 0)
            {
                previousTaskName = TaskName;
                tddLogger.Info(TaskName);
            }
        }
    }
}
