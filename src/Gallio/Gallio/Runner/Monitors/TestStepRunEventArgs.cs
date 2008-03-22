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
using Gallio.Model;
using Gallio.Model.Serialization;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Monitors
{
    /// <summary>
    /// Provides report information for a test step that is beginning or ending
    /// or transitioning between phases.
    /// </summary>
    [Serializable]
    public class TestStepRunEventArgs : EventArgs
    {
        private readonly Report report;
        private readonly TestData test;
        private readonly TestStepRun testStepRun;
        private readonly string lifecyclePhase;

        /// <summary>
        /// Creates event arguments about a step.
        /// </summary>
        /// <param name="report">The report</param>
        /// <param name="test">The test data</param>
        /// <param name="testStepRun">The test step run element</param>
        /// <param name="lifecyclePhase">The test step's lifecycle phase, or an empty string if starting or ending</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="report>"/>, <paramref name="test"/>
        /// or <paramref name="testStepRun"/> is null</exception>
        public TestStepRunEventArgs(Report report, TestData test, TestStepRun testStepRun, string lifecyclePhase)
        {
            if (report == null)
                throw new ArgumentNullException("report");
            if (test == null)
                throw new ArgumentNullException("test");
            if (testStepRun == null)
                throw new ArgumentNullException("testStepRun");
            if (lifecyclePhase == null)
                throw new ArgumentNullException("lifecyclePhase");

            this.report = report;
            this.test = test;
            this.testStepRun = testStepRun;
            this.lifecyclePhase = lifecyclePhase;
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        public Report Report
        {
            get { return report; }
        }

        /// <summary>
        /// Gets the test data of the test that contains the test step.
        /// </summary>
        public TestData Test
        {
            get { return test; }
        }

        /// <summary>
        /// Gets the test step run element.
        /// </summary>
        public TestStepRun TestStepRun
        {
            get { return testStepRun; }
        }

        /// <summary>
        /// Gets the current test step's lifecycle phase, or an empty string if starting or ending.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        public string LifecyclePhase
        {
            get { return lifecyclePhase; }
        }

        /// <summary>
        /// Gets the kind of step described using the <see cref="MetadataKeys.TestKind" /> metadata key.
        /// </summary>
        /// <returns>The step kind</returns>
        public string GetStepKind()
        {
            return TestStepRun.Step.Metadata.GetValue(MetadataKeys.TestKind)
                ?? Test.Metadata.GetValue(MetadataKeys.TestKind)
                ?? TestKinds.Test;
        }
    }
}
