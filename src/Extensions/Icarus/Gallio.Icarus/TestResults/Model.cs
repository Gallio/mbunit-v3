using System.Collections.Generic;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Icarus.TestResults
{
    public class Model : IModel {
        private readonly IDictionary<string, IList<TestStepRun>> testStepRuns;

        public Model()
        {
            testStepRuns = new Dictionary<string, IList<TestStepRun>>();
        }

        public void AddTestStepRun(string testId, TestStepRun testStepRun)
        {
            IList<TestStepRun> runs;
            if (!testStepRuns.ContainsKey(testId))
            {
                runs = new List<TestStepRun>();
                testStepRuns.Add(testId, runs);
            }
            else 
            {
                runs = testStepRuns[testId];
            }
            runs.Add(testStepRun);
        }

        public IList<TestStepRun> GetTestStepRuns(string testId)
        {
            if (testStepRuns.ContainsKey(testId)) 
            {
                return testStepRuns[testId];
            }
            return new List<TestStepRun>();
        }
    }
}
