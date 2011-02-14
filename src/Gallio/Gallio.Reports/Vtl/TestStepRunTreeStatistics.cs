using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Common;

namespace Gallio.Reports.Vtl
{
    /// <summary>
    /// Aggregates statistics for an entire tree branch.
    /// </summary>
    public class TestStepRunTreeStatistics
    {
        private class TestStatusData
        {
            public Memoizer<int> Count = new Memoizer<int>();
            public Memoizer<IDictionary<string, int>> CountPerCategory = new Memoizer<IDictionary<string, int>>();
        }

        private readonly TestStepRun root;
        private Memoizer<int> runCount = new Memoizer<int>();
        private readonly IDictionary<TestStatus, TestStatusData> map = new Dictionary<TestStatus, TestStatusData>
        {
            { TestStatus.Passed, new TestStatusData() },
            { TestStatus.Failed, new TestStatusData() },
            { TestStatus.Skipped, new TestStatusData() },
            { TestStatus.Inconclusive, new TestStatusData() },
        };

        /// <summary>
        /// Constructs statistics for the branch identified by the specified root element.
        /// </summary>
        /// <param name="root">The root element of the branch</param>
        public TestStepRunTreeStatistics(TestStepRun root)
        {
            this.root = root;
        }

        /// <summary>
        /// Gets the number of runs in the branch.
        /// </summary>
        public int RunCount
        {
            get { return runCount.Memoize(() => CountImpl(root, null)); } 
        }

        /// <summary>
        /// Gets the number of passed steps in the branch.
        /// </summary>
        public int PassedCount
        {
            get { return CountPerStatus(TestStatus.Passed); }
        }

        /// <summary>
        /// Gets the number of failed steps in the branch.
        /// </summary>
        public int FailedCount
        {
            get { return CountPerStatus(TestStatus.Failed); }
        }

        /// <summary>
        /// Gets the number of skipped steps in the branch.
        /// </summary>
        public int SkippedCount
        {
            get { return CountPerStatus(TestStatus.Skipped); }
        }

        /// <summary>
        /// Gets the number of inconclusive steps in the branch.
        /// </summary>
        public int InconclusiveCount
        {
            get { return CountPerStatus(TestStatus.Inconclusive); }
        }

        private int CountPerStatus(TestStatus status)
        {
            return map[status].Count.Memoize(() => CountImpl(root, status));
        }

        /// <summary>
        /// Gets the number of skipped and inconclusive steps in the branch.
        /// </summary>
        public int SkippedOrInconclusiveCount
        {
            get { return SkippedCount + InconclusiveCount; }
        }

        /// <summary>
        /// Formats the number of passed steps in the branch with details about the inner categories.
        /// </summary>
        /// <example>
        /// "8 passed"
        /// </example>
        /// <returns>A comprehensive description of the step count.</returns>
        public string FormatPassedCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Passed);
        }

        /// <summary>
        /// Formats the number of failed steps in the branch with details about the inner categories.
        /// </summary>
        /// <example>
        /// "5 failed (2 error, 1 timeout)"
        /// </example>
        /// <returns>A comprehensive description of the step count.</returns>
        public string FormatFailedCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Failed);
        }

        /// <summary>
        /// Formats the number of skipped steps in the branch with details about the inner categories.
        /// </summary>
        /// <example>
        /// "6 skipped (2 pending, 1 ignored)"
        /// </example>
        /// <returns>A comprehensive description of the step count.</returns>
        public string FormatSkippedCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Skipped);
        }

        /// <summary>
        /// Formats the number of inconclusive steps in the branch with details about the inner categories.
        /// </summary>
        /// <example>
        /// "4 inconclusive (1 canceled)"
        /// </example>
        /// <returns>A comprehensive description of the step count.</returns>
        public string FormatInconclusiveCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Inconclusive);
        }

        private IDictionary<string, int> CountPerCategory(TestStatus status)
        {
            return map[status].CountPerCategory.Memoize(() =>
            {
                var aggregator = new Dictionary<string, int>();
                CountCategories(root, status, aggregator);
                return aggregator;
            });
        }

        private string FormatCountWithCategories(TestStatus status)
        {
            var output = new StringBuilder();
            output.Append(CountPerStatus(status) + " " + status.ToString().ToLower());
            var categories = CountPerCategory(status);
            
            if (categories.Count > 0)
            {
                output.Append(" (");
                bool first = true;

                foreach (var pair in categories)
                {
                    if (!first)
                        output.Append(", ");

                    output.Append(pair.Value + " " + pair.Key);
                    first = false;
                }

                output.Append(")");
            }

            return output.ToString();
        }

        private static int CountImpl(TestStepRun run, TestStatus? status)
        {
            if (run.Children.Count == 0)
                return (!status.HasValue || (run.Result.Outcome.Status == status)) ? 1 : 0;

            int count = 0;

            foreach (TestStepRun child in run.Children)
                count += CountImpl(child, status);

            return count;
        }

        private static void CountCategories(TestStepRun run, TestStatus status, IDictionary<string, int> aggregator)
        {
            if (run.Children.Count == 0)
            {
                if (run.Result.Outcome.Status == status && !String.IsNullOrEmpty(run.Result.Outcome.Category))
                {
                    string category = run.Result.Outcome.Category;
                    int count;
                    aggregator[category] = 1 + (aggregator.TryGetValue(category, out count) ? count : 0);
                }

                return;
            }

            foreach (TestStepRun child in run.Children)
                CountCategories(child, status, aggregator);
        }
    }
}
