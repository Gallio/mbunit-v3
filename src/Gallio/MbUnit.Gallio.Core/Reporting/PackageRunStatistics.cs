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
using System.Xml.Serialization;
using MbUnit.Framework.Kernel.Results;
using MbUnit.Framework.Kernel.Serialization;
using MbUnit.Framework.Kernel.Utilities;

namespace MbUnit.Core.Reporting
{
    /// <summary>
    /// Collects summary statistics about the execution of a test package for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName="statistics", Namespace=SerializationUtils.XmlNamespace)]
    [XmlType(Namespace=SerializationUtils.XmlNamespace)]
    public class PackageRunStatistics
    {
        private int assertCount;
        private double duration;
        private int failureCount;
        private int ignoreCount;
        private int inconclusiveCount;
        private int runCount;
        private int skipCount;
        private int passCount;
        private int testCount;

        /// <summary>
        /// Gets or sets the number of assertions evaluated.
        /// </summary>
        [XmlAttribute("assertCount")]
        public int AssertCount
        {
            get { return assertCount; }
            set { assertCount = value; }
        }

        /// <summary>
        /// Gets or sets the duration of the package run in seconds.
        /// </summary>
        [XmlAttribute("duration")]
        public double Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that were run.
        /// </summary>
        [XmlAttribute("runCount")]
        public int RunCount
        {
            get { return runCount; }
            set { runCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that ran and passed.
        /// </summary>
        [XmlAttribute("passCount")]
        public int PassCount
        {
            get { return passCount; }
            set { passCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that ran and failed.
        /// </summary>
        [XmlAttribute("failureCount")]
        public int FailureCount
        {
            get { return failureCount; }
            set { failureCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that ran and were inconclusive.
        /// </summary>
        [XmlAttribute("inconclusiveCount")]
        public int InconclusiveCount
        {
            get { return inconclusiveCount; }
            set { inconclusiveCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that did not run because they were ignored.
        /// </summary>
        [XmlAttribute("ignoreCount")]
        public int IgnoreCount
        {
            get { return ignoreCount; }
            set { ignoreCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that did not run because they were skipped.
        /// </summary>
        [XmlAttribute("skipCount")]
        public int SkipCount
        {
            get { return skipCount; }
            set { skipCount = value; }
        }

        /// <summary>
        /// Gets or sets the total number of test cases.
        /// </summary>
        [XmlAttribute("testCount")]
        public int TestCount
        {
            get { return testCount; }
            set { testCount = value; }
        }

        /// <summary>
        /// Formats a single line of text summarizing test case results.
        /// </summary>
        public string FormatTestCaseResultSummary()
        {
            return String.Format("Run: {0}, Passed: {1}, Failed: {2}, Inconclusive: {3}, Ignored: {4}, Skipped: {5}.",
                runCount, passCount, failureCount, inconclusiveCount, ignoreCount, skipCount);
        }

        /// <summary>
        /// Merges statistics from a test run, incrementing the relevant counters.
        /// </summary>
        /// <param name="testInfo">The test info</param>
        /// <param name="testRun">The test run</param>
        public void MergeTestRunStatistics(TestInfo testInfo, TestRun testRun)
        {
            if (testRun == null)
                throw new ArgumentNullException("testRun");

            assertCount += testRun.Result.AssertCount;

            if (!testInfo.IsTestCase)
                return;

            testCount += 1;

            // Tally the various test run states.
            switch (testRun.Result.State)
            {
                case TestState.Completed:
                case TestState.Failed:
                case TestState.Canceled:
                    runCount += 1;
                    break;

                case TestState.NotRun:
                    return;

                case TestState.Ignored:
                    ignoreCount += 1;
                    return;

                case TestState.Skipped:
                    skipCount += 1;
                    return;
            }

            // If the test ran, tally the various outcomes.
            switch (testRun.Result.Outcome)
            {
                case TestOutcome.Passed:
                    passCount += 1;
                    break;

                case TestOutcome.Failed:
                    failureCount += 1;
                    break;

                case TestOutcome.Inconclusive:
                    inconclusiveCount += 1;
                    break;
            }
        }
    }
}
