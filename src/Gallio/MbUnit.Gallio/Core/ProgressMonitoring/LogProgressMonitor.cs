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

namespace MbUnit.Core.ProgressMonitoring
{
    /// <summary>
    /// Implementation of <see cref="IProgressMonitor" />
    /// that logs messages to an <see cref="ILogger" />.
    /// </summary>
    public class LogProgressMonitor : TextualProgressMonitor
    {
        private readonly ILogger logger;
        private string previousTaskName = string.Empty;

        /// <summary>
        /// Create a logged progress monitor.
        /// </summary>
        /// <param name="logger">A logger instance to which messages will be written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public LogProgressMonitor(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(@"logger");

            this.logger = logger;
        }

        /// <inheritdoc />
        protected override void UpdateDisplay()
        {
            if (previousTaskName.CompareTo(TaskName) != 0)
            {
                previousTaskName = TaskName;
                logger.Info(TaskName);
            }
        }
    }
}