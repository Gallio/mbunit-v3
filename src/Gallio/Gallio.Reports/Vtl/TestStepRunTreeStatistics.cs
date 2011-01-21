using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Model;
using Gallio.Runner.Reports.Schema;
using Gallio.Common;

namespace Gallio.Reports.Vtl
{
    internal class TestStepRunTreeStatistics
    {
        private class TestStatusData
        {
            public Memoizer<int> Count = new Memoizer<int>();
            public Memoizer<IDictionary<string, int>> CountPerCategory = new Memoizer<IDictionary<string, int>>();
        }

        private readonly TestStepRun run;
        private Memoizer<int> runCount = new Memoizer<int>();
        private readonly IDictionary<TestStatus, TestStatusData> map = new Dictionary<TestStatus, TestStatusData>
        {
            { TestStatus.Passed, new TestStatusData() },
            { TestStatus.Failed, new TestStatusData() },
            { TestStatus.Skipped, new TestStatusData() },
            { TestStatus.Inconclusive, new TestStatusData() },
        };

        public int RunCount
        { 
            get { return runCount.Memoize(() => CountImpl(run, null)); } 
        }

        public int PassedCount
        {
            get { return CountPerStatus(TestStatus.Passed); }
        }

        public int FailedCount
        {
            get { return CountPerStatus(TestStatus.Failed); }
        }

        public int SkippedCount
        {
            get { return CountPerStatus(TestStatus.Skipped); }
        }

        public int InconclusiveCount
        {
            get { return CountPerStatus(TestStatus.Inconclusive); }
        }

        private int CountPerStatus(TestStatus status)
        {
            return map[status].Count.Memoize(() => CountImpl(run, status));
        }

        public int SkippedOrInconclusiveCount
        {
            get { return SkippedCount + InconclusiveCount; }
        }

        public string FormatPassedCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Passed);
        }

        public string FormatFailedCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Failed);
        }

        public string FormatSkippedCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Skipped);
        }

        public string FormatInconclusiveCountWithCategories()
        {
            return FormatCountWithCategories(TestStatus.Inconclusive);
        }

        private IDictionary<string, int> CountPerCategory(TestStatus status)
        {
            return map[status].CountPerCategory.Memoize(() =>
            {
                var aggregator = new Dictionary<string, int>();
                CountCategories(run, status, aggregator);
                return aggregator;
            });
        }

        public string FormatCountWithCategories(TestStatus status)
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
                    {
                        output.Append(", ");
                    }

                    output.Append(pair.Value + " " + pair.Key);
                    first = false;
                }

                output.Append(")");
            }

            return output.ToString();
        }

        public TestStepRunTreeStatistics(TestStepRun run)
        {
            this.run = run;
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
