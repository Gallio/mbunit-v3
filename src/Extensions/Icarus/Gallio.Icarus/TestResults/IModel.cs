using System.Collections.Generic;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Icarus.TestResults {
    public interface IModel {
        void AddTestStepRun(string testId, TestStepRun testStepRun);
        IList<TestStepRun> GetTestStepRuns(string testId);
    }
}