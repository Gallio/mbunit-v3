using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// <para>
    /// The test run history object manages a historical record of test runs.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class are safe for use by multiple concurrent threads.
    /// </para>
    /// </remarks>
    public interface ITestRunHistory
    {
        /// <summary>
        /// Gets all test runs in the history.
        /// </summary>
        /// <returns>An immutable list of all historical test runs</returns>
        IList<ITestRun> GetAllTestRuns();

        /// <summary>
        /// Purges the test run history.
        /// </summary>
        void Purge();
    }
}
