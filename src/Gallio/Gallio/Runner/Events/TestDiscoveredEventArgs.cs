// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to signal that a test was discovered and added to the test model.
    /// </summary>
    public sealed class TestDiscoveredEventArgs : EventArgs
    {
        private readonly Report report;
        private readonly TestData test;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="test">The test at the top of the subtree that was added.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report"/> is null.</exception>
        public TestDiscoveredEventArgs(Report report, TestData test)
        {
            if (report == null)
                throw new ArgumentNullException("report");
            if (test == null)
                throw new ArgumentNullException("test");

            this.report = report;
            this.test = test;
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// Gets the test at the top of the subtree that was merged.
        /// </summary>
        public TestData Test
        {
            get { return test; }
        }
    }
}
