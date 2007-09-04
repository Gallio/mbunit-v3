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
using MbUnit.Core.Reporting;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Runner.Monitors
{
    /// <summary>
    /// Provides report information for a test step that is beginning or ending.
    /// </summary>
    [Serializable]
    public class ReportStepEventArgs : EventArgs
    {
        private readonly Report report;
        private readonly TestRun testRun;
        private readonly StepRun stepRun;

        /// <summary>
        /// Creates event arguments about a step.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="testRun">The test run element that contains the step</param>
        /// <param name="stepRun">The step run element</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>,
        /// <paramref name="testRun"/> or <paramref name="stepRun"/> is null</exception>
        public ReportStepEventArgs(Report report, TestRun testRun, StepRun stepRun)
        {
            if (report == null)
                throw new ArgumentNullException(@"report");
            if (testRun == null)
                throw new ArgumentNullException(@"testRun");
            if (stepRun == null)
                throw new ArgumentNullException(@"stepRun");

            this.report = report;
            this.testRun = testRun;
            this.stepRun = stepRun;
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// Gets the test run element that contains the step.
        /// </summary>
        public TestRun TestRun
        {
            get { return testRun; }
        }

        /// <summary>
        /// Gets the step run element.
        /// </summary>
        public StepRun StepRun
        {
            get { return stepRun; }
        }
    }
}
