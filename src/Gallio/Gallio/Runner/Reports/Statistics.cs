// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Utilities;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Collects summary statistics about test execution for reporting purposes.
    /// </summary>
    [Serializable]
    [XmlRoot("statistics", Namespace=XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace=XmlSerializationUtils.GallioNamespace)]
    public sealed class Statistics
    {
        private int assertCount;
        private double duration;

        private int runCount;
        private int testCount;
        private int stepCount;

        private int passedCount;
        private int failedCount;
        private int inconclusiveCount;
        private int skippedCount;

        private readonly Dictionary<TestOutcome, int> outcomeCounts;

        /// <summary>
        /// Creates an empty statistics record.
        /// </summary>
        public Statistics()
        {
            outcomeCounts = new Dictionary<TestOutcome, int>();
        }

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
        /// Gets or sets the total duration summarized tests in seconds.
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
        [XmlAttribute("passedCount")]
        public int PassedCount
        {
            get { return passedCount; }
            set { passedCount = value; }
        }

        /// <summary>
        /// Gets or sets the number of test cases that ran and failed.
        /// </summary>
        [XmlAttribute("failedCount")]
        public int FailedCount
        {
            get { return failedCount; }
            set { failedCount = value; }
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
        /// Gets or sets the number of test cases that did not run.
        /// </summary>
        [XmlAttribute("skippedCount")]
        public int SkippedCount
        {
            get { return skippedCount; }
            set { skippedCount = value; }
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
        /// Gets or sets the total number of test steps.
        /// </summary>
        [XmlAttribute("stepCount")]
        public int StepCount
        {
            get { return stepCount; }
            set { stepCount = value; }
        }

        /// <summary>
        /// Gets or sets the test outcome summaries.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        [XmlArray("outcomeSummaries", IsNullable=false)]
        [XmlArrayItem("outcomeSummary", typeof(TestOutcomeSummary), IsNullable=false)]
        public TestOutcomeSummary[] OutcomeSummaries
        {
            get
            {
                return GenericUtils.ConvertAllToArray<KeyValuePair<TestOutcome, int>, TestOutcomeSummary>(outcomeCounts,
                    delegate(KeyValuePair<TestOutcome, int> entry)
                    {
                        TestOutcomeSummary summary = new TestOutcomeSummary();
                        summary.Outcome = entry.Key;
                        summary.Count = entry.Value;
                        return summary;
                    });
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                outcomeCounts.Clear();

                foreach (TestOutcomeSummary summary in value)
                    SetOutcomeCount(summary.Outcome, summary.Count);
            }
        }

        /// <summary>
        /// Gets the number of tests with the specified outcome.
        /// </summary>
        /// <param name="outcome">The outcome</param>
        /// <returns>The number of tests with the specified outcome</returns>
        public int GetOutcomeCount(TestOutcome outcome)
        {
            int count;
            outcomeCounts.TryGetValue(outcome, out count);
            return count;
        }

        /// <summary>
        /// Ssets the number of tests with the specified outcomee.
        /// </summary>
        /// <param name="outcome">The outcome</param>
        /// <param name="count">The count</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than 0</exception>
        public void SetOutcomeCount(TestOutcome outcome, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Count must be non-negative.");

            outcomeCounts[outcome] = count;
        }

        /// <summary>
        /// Formats a single line of text summarizing test case results.
        /// </summary>
        public string FormatTestCaseResultSummary()
        {
            StringBuilder str = new StringBuilder();

            str.Append(runCount).Append(" run, ");

            str.Append(passedCount).Append(" passed");
            AppendCategorizedOutcomeCounts(str, TestStatus.Passed);
            str.Append(", ");

            str.Append(failedCount).Append(" failed");
            AppendCategorizedOutcomeCounts(str, TestStatus.Failed);
            str.Append(", ");

            str.Append(inconclusiveCount).Append(" inconclusive");
            AppendCategorizedOutcomeCounts(str, TestStatus.Inconclusive);
            str.Append(", ");

            str.Append(skippedCount).Append(" skipped");
            AppendCategorizedOutcomeCounts(str, TestStatus.Skipped);

            return str.ToString();
        }

        private void AppendCategorizedOutcomeCounts(StringBuilder str, TestStatus status)
        {
            SortedDictionary<string, int> categoryCounts = new SortedDictionary<string, int>();

            foreach (KeyValuePair<TestOutcome, int> entry in outcomeCounts)
                if (entry.Key.Status == status && entry.Key.Category != null)
                    categoryCounts.Add(entry.Key.Category, entry.Value);

            if (categoryCounts.Count != 0)
            {
                str.Append(" (");

                bool first = true;
                foreach (KeyValuePair<string, int> entry in categoryCounts)
                {
                    if (!first)
                        str.Append(", ");
                    else
                        first = false;

                    str.Append(entry.Value).Append(' ').Append(entry.Key);
                }

                str.Append(')');
            }
        }

        /// <summary>
        /// Merges statistics from a test step run, incrementing the relevant counters.
        /// </summary>
        /// <param name="testStepRun">The test step run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStepRun"/> is null</exception>
        public void MergeStepStatistics(TestStepRun testStepRun)
        {
            if (testStepRun == null)
                throw new ArgumentNullException("testStepRun");

            // The assert count and duration statistics are pre-aggregated in that parent tests include
            // the values for their children when publishing their results.  This implies that the
            // root test will contain the official final tally which will be larger than all previously
            // seen tallies.
            assertCount = Math.Max(assertCount, testStepRun.Result.AssertCount);
            duration = Math.Max(duration, testStepRun.Result.Duration);

            stepCount += 1;

            if (! testStepRun.Step.IsTestCase)
                return;

            testCount += 1;
            AddOutcome(testStepRun.Result.Outcome);
        }

        /// <summary>
        /// Updates the test outcome statistics counters with an the outcome of a test.
        /// </summary>
        /// <remarks>
        /// Does not update the test or step count.
        /// </remarks>
        /// <param name="outcome">The test outcome</param>
        public void AddOutcome(TestOutcome outcome)
        {

            switch (outcome.Status)
            {
                case TestStatus.Skipped:
                    skippedCount += 1;
                    break;

                case TestStatus.Passed:
                    passedCount += 1;
                    runCount += 1;
                    break;

                case TestStatus.Inconclusive:
                    inconclusiveCount += 1;
                    runCount += 1;
                    break;

                case TestStatus.Failed:
                    failedCount += 1;
                    runCount += 1;
                    break;
            }

            SetOutcomeCount(outcome, GetOutcomeCount(outcome) + 1);
        }
    }
}
