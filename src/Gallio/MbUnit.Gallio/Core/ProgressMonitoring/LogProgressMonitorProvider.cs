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
    /// Runs tasks with a <see cref="LogProgressMonitor" />.
    /// </summary>
    public class LogProgressMonitorProvider : IProgressMonitorProvider
    {
        private readonly ILogger logger;

        /// <summary>
        /// Creates a log progress monitor provider.
        /// </summary>
        /// <param name="logger">The logger to which messages should be written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public LogProgressMonitorProvider(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(@"logger");

            this.logger = logger;
        }

        /// <inheritdoc />
        public void Run(TaskWithProgress task)
        {
            using (LogProgressMonitor progressMonitor = new LogProgressMonitor(logger))
            {
                task(progressMonitor);
            }
        }
    }
}