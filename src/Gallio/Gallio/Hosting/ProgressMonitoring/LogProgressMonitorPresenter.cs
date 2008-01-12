// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Hosting.ProgressMonitoring
{
    /// <summary>
    /// Displays progress by writing a series of messages to an <see cref="ILogger" /> as the name
    /// of the current task changes.
    /// </summary>
    public class LogProgressMonitorPresenter : BaseProgressMonitorPresenter
    {
        private readonly ILogger logger;
        private string previousTaskName = string.Empty;

        /// <summary>
        /// Create a logged progress monitor presenter.
        /// </summary>
        /// <param name="logger">A logger instance to which messages will be written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public LogProgressMonitorPresenter(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(@"logger");

            this.logger = logger;
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            ProgressMonitor.Changed += HandleChanged;
        }

        private void HandleChanged(object sender, EventArgs e)
        {
            string currentTaskName = ProgressMonitor.TaskName;

            if (previousTaskName.CompareTo(currentTaskName) != 0)
            {
                previousTaskName = currentTaskName;
                logger.Info(currentTaskName);
            }
        }
    }
}