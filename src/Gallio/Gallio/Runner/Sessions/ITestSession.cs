using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// <para>
    /// A test session describes a test run that is either pending, in progress
    /// or completed.  It provides access to intermediate and final test results
    /// and fires events to describe progress and state changes.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class are safe for use by multiple concurrent threads.
    /// </para>
    /// </remarks>
    public interface ITestSession
    {
        /// <summary>
        /// An event that is fired whenever the current test run is changed.
        /// </summary>
        event EventHandler<TestRunEventArgs> TestRunChanged;

        /// <summary>
        /// Gets the session manager that owns this session.
        /// </summary>
        ITestSessionManager Manager { get; }

        /// <summary>
        /// Gets the session unique id.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Returns true if the session is currently open.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets the current test run, or null if there is none.
        /// </summary>
        ITestRun CurrentTestRun { get; }

        /// <summary>
        /// Gets the test run history manager for this session.
        /// </summary>
        ITestRunHistory TestRunHistory { get; }

        /// <summary>
        /// Creates a new test run and changes the current one.
        /// </summary>
        /// <param name="packageConfig">The test package configuration</param>
        /// <param name="runnerOptions">The test runner options</param>
        /// <param name="explorationOptions">The test exploration options</param>
        /// <param name="executionOptions">The test execution options</param>
        /// <returns>The new test run</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="packageConfig"/>,
        /// <paramref name="runnerOptions"/>, <paramref name="explorationOptions"/>,
        /// <paramref name="executionOptions"/> is null</exception>
        ITestRun CreateRun(TestPackageConfig packageConfig, TestRunnerOptions runnerOptions,
            TestExplorationOptions explorationOptions, TestExecutionOptions executionOptions);
    }
}
