// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
    /// <para>
    /// A test runner provides the basic functionality for loading, exploring and running
    /// tests.  It abstracts away most concerns having to do with the execution of tests
    /// in isolated (possibly remote) domains.
    /// </para>
    /// <para>
    /// The basic usage pattern of a test runner is as follows:
    /// <list type="bullet">
    /// <item>As a precondition, ensure that the runtime environment has been initialized.</item>
    /// <item>Create the test runner.</item>
    /// <item>Add event handlers and register extensions.  <seealso cref="Events"/>, <seealso cref="RegisterExtension"/></item>
    /// <item>Initialize the runner.  <seealso cref="Initialize"/></item>
    /// <item>Load a test package.  <seealso cref="Load"/></item>
    /// <item>Explore the tests.  <seealso cref="Explore"/></item>
    /// <item>Run the tests.  Save or format the report contents as required using the reporting APIs.  <seealso cref="Run"/></item>
    /// <item>Unload the test package.  <seealso cref="Unload"/></item>
    /// <item>Dispose the test runner.  <seealso cref="Dispose"/></item>
    /// </list>
    /// </para>
    /// <para>
    /// Once a test package is loaded, it may be re-explored and/or re-run as often as desired.
    /// The package may also be unloaded to release resources make way for a new package to be
    /// loaded at a later time without discarding the test runner itself.
    /// </para>
    /// </summary>
    public interface ITestRunner
    {
        /// <summary>
        /// Gets the event dispatcher for the test runner.
        /// </summary>
        ITestRunnerEvents Events { get; }

        /// <summary>
        /// Gets the most recent report contents.
        /// The report may be only partially populated depending on the current state of
        /// the test runner.  When no test package has been loaded, the report will be empty
        /// but not null.
        /// </summary>
        Report Report { get; }

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
        /// <param name="extension">The test runner extension to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the test runner has already been initialized</exception>
        void RegisterExtension(ITestRunnerExtension extension);

        /// <summary>
        /// <para>
        /// Initializes the test runner.
        /// </para>
        /// </summary>
        /// <param name="options">The test runner options</param>
        /// <param name="logger">The logger</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options" />,
        /// <paramref name="logger" /> or <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the runner is already initialized</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed</exception>
        void Initialize(TestRunnerOptions options, ILogger logger, IProgressMonitor progressMonitor);

        /// <summary>
        /// <para>
        /// Loads a test package.
        /// </para>
        /// <para>
        /// When this operation completes, the <see cref="Reports.Report.TestPackageConfig" /> property
        /// of <see cref="Report" /> will contain information about the test package.
        /// </para>
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> or <paramref name="testPackageConfig"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if a package is already loaded</exception>
        /// <exception cref="RunnerException">Thrown if the operation failed</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed</exception>
        void Load(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor);

        /// <summary>
        /// <para>
        /// Explores tests within the currently loaded test package.
        /// </para>
        /// <para>
        /// When this operation completes, the <see cref="Reports.Report.TestModel" /> property
        /// of <see cref="Report" /> will contain information about the test package.
        /// </para>
        /// </summary>
        /// <param name="options">The test exploration options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="options"/> or
        /// <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if no package is currently loaded</exception>
        /// <exception cref="RunnerException">Thrown if the operation failed</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed</exception>
        void Explore(TestExplorationOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// <para>
        /// Runs the tests.
        /// </para>
        /// <para>
        /// When this operation completes, the <see cref="Reports.Report.TestPackageRun" /> property
        /// of <see cref="Report" /> will contain test results.
        /// </para>
        /// </summary>
        /// <param name="options">The test execution options</param>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if no package is currently loaded
        /// or if test exploration has not taken place</exception>
        /// <exception cref="RunnerException">Thrown if the operation failed</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed</exception>
        void Run(TestExecutionOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// <para>
        /// Unloads the current test package.  Does nothing if none is currently loaded.
        /// </para>
        /// <para>
        /// When this operation completes, the <see cref="Report" /> will be empty once again.
        /// </para>
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        /// <exception cref="RunnerException">Thrown if the operation failed</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the runner has been disposed</exception>
        void Unload(IProgressMonitor progressMonitor);

        /// <summary>
        /// <para>
        /// Disposes the test runner.  Does nothing if already disposed or if not initialized.
        /// </para>
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        void Dispose(IProgressMonitor progressMonitor);
    }
}