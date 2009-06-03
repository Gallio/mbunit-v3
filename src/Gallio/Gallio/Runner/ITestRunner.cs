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
using Gallio.Model.Execution;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;

namespace Gallio.Runner
{
    /// <summary>
    /// A test runner provides the basic functionality for loading, exploring and running
    /// tests.  It abstracts away most concerns having to do with the execution of tests
    /// in isolated (possibly remote) domains.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The basic usage pattern of a test runner is as follows:
    /// <list type="bullet">
    /// <item>As a precondition, ensure that the runtime environment has been initialized.</item>
    /// <item>Create the test runner.</item>
    /// <item>Add event handlers and register extensions.  <seealso cref="Events"/>, <seealso cref="RegisterExtension"/></item>
    /// <item>Initialize the runner.  <seealso cref="Initialize"/></item>
    /// <item>Explore and/or Run the tests.  <seealso cref="Explore"/> and <seealso cref="Run" /></item>
    /// <item>Dispose the test runner.  <seealso cref="Dispose"/></item>
    /// <item>Save or format the report contents as required using the reporting APIs.</item>
    /// </list>
    /// </para>
    /// <para>
    /// Multiple test runs may be performed between initialization and disposal of the test runner.
    /// </para>
    /// </remarks>
    public interface ITestRunner
    {
        /// <summary>
        /// Gets the event dispatcher for the test runner.
        /// </summary>
        ITestRunnerEvents Events { get; }

        /// <summary>
        /// <para>
        /// Registers a test runner extension.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The extension should already have been configured before this method
        /// is called.  Its <see cref="ITestRunnerExtension.Install" /> method will be
        /// called to register the extension with the test runner as part of the initialization
        /// of the test runner.
        /// </remarks>
        /// <param name="extension">The test runner extension to register.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the test runner has already been initialized.</exception>
        void RegisterExtension(ITestRunnerExtension extension);

        /// <summary>
        /// Initializes the test runner.
        /// </summary>
        /// <param name="testRunnerOptions">The test runner options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testRunnerOptions" />,
        /// <paramref name="logger" /> or <paramref name="progressMonitor"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the runner is already initialized.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed.</exception>
        void Initialize(TestRunnerOptions testRunnerOptions, ILogger logger, IProgressMonitor progressMonitor);

        /// <summary>
        /// Explores tests in a test package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns a report that contains test package and test model information only.
        /// </para>
        /// </remarks>
        /// <param name="testPackageConfig">The test package configuration.</param>
        /// <param name="testExplorationOptions">The test exploration options.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The test report</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/>,
        /// <paramref name="testExplorationOptions"/>, or <paramref name="progressMonitor"/> is null.</exception>
        /// <exception cref="RunnerException">Thrown if the operation failed.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed.</exception>
        Report Explore(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions,
            IProgressMonitor progressMonitor);

        /// <summary>
        /// Explores and runs tests in a test package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns a report that contains test package, test model and test run data.
        /// </para>
        /// </remarks>
        /// <param name="testPackageConfig">The test package configuration.</param>
        /// <param name="testExplorationOptions">The test exploration options.</param>
        /// <param name="testExecutionOptions">The test execution options.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The test report</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/>,
        /// <paramref name="testExplorationOptions"/>, <paramref name="testExecutionOptions"/>, or <paramref name="progressMonitor"/> is null.</exception>
        /// <exception cref="RunnerException">Thrown if the operation failed.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed.</exception>
        Report Run(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions,
            TestExecutionOptions testExecutionOptions, IProgressMonitor progressMonitor);

        /// <summary>
        /// Disposes the test runner.  Does nothing if already disposed or if not initialized.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null.</exception>
        void Dispose(IProgressMonitor progressMonitor);
    }
}