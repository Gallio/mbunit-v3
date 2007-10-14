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
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.IO;
using MbUnit.Core.ProgressMonitoring;

namespace MbUnit.Core.ProgressMonitoring
{
    /// <summary>
    /// Runs tasks with a <see cref="RichConsoleProgressMonitor" />.
    /// </summary>
    public class RichConsoleProgressMonitorProvider : IProgressMonitorProvider
    {
        private readonly IRichConsole console;

        /// <summary>
        /// Creates a rich console progress monitor provider.
        /// </summary>
        /// <param name="console">The console to which messages should be written</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="console"/> is null</exception>
        public RichConsoleProgressMonitorProvider(IRichConsole console)
        {
            if (console == null)
                throw new ArgumentNullException(@"console");

            this.console = console;
        }

        /// <inheritdoc />
        public void Run(TaskWithProgress task)
        {
            using (RichConsoleProgressMonitor progressMonitor = new RichConsoleProgressMonitor(console))
            {
                progressMonitor.ThrowIfCanceled();
                task(progressMonitor);
                progressMonitor.ThrowIfCanceled();
            }
        }
    }
}