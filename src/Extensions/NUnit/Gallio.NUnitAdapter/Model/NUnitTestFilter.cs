using System.Collections.Generic;
using Gallio.Model.Commands;
using NUnit.Core;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Implementation of NUnit's ITestFilter. Controls which tests are run.
    /// </summary>
    internal class NUnitTestFilter : ITestFilter
    {
        private readonly Dictionary<TestName, ITestCommand> testCommandsByTestName;

        public NUnitTestFilter(Dictionary<TestName, ITestCommand> testCommandsByTestName)
        {
            this.testCommandsByTestName = testCommandsByTestName;
        }

        public bool Pass(ITest test)
        {
            return FilterTest(test);
        }

        public bool Match(ITest test)
        {
            return FilterTest(test);
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        private bool FilterTest(ITest test)
        {
            if (!testCommandsByTestName.ContainsKey(test.TestName))
                return false;

            var testCommand = testCommandsByTestName[test.TestName];

            if (test.RunState == RunState.Explicit)
                return testCommand.IsExplicit;

            return true;
        }
    }
}