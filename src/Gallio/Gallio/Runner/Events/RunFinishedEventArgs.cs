// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test execution has finished.
    /// </summary>
    public sealed class RunFinishedEventArgs : OperationFinishedEventArgs
    {
        private readonly Report report;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="success">True if test execution was successful</param>
        /// <param name="report">The report, including test package run data on success</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> is null</exception>
        public RunFinishedEventArgs(bool success, Report report)
            : base(success)
        {
            if (report == null)
                throw new ArgumentNullException("report");

            this.report = report;
        }

        /// <summary>
        /// Gets the report, including test package run data on success.
        /// </summary>
        public Report Report
        {
            get { return report; }
        }
    }
}
