using System;

namespace Gallio.Runner
{
    /// <summary>
    /// Provides options that control the operation of the test runner.
    /// </summary>
    [Serializable]
    public class TestRunnerOptions
    {
        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy</returns>
        public TestRunnerOptions Copy()
        {
            TestRunnerOptions copy = new TestRunnerOptions();

            return copy;
        }
    }
}
