// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.IO;
using Gallio.Concurrency;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports;
using Gallio.Runtime.Logging;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// <para>
    /// A test run captures the results of a particualr test execution.  Once a
    /// test run completes, its state becomes immutable.  Thus a test run may
    /// be preserved for some time to enable the user to review details of the
    /// prior test run history.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class are safe for use by multiple concurrent threads.
    /// </para>
    /// </remarks>
    public interface ITestRun
    {
        /// <summary>
        /// An event that is fired when the test run starts.
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// An event that is fired when the test run stops.
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        /// An event that is fired when a log message is received during the test run.
        /// </summary>
        event EventHandler<LogMessageEventArgs> LogMessage;

        /// <summary>
        /// Gets an object and enables test run clients to listen to test runner events
        /// during test execution.
        /// </summary>
        ITestRunnerEvents TestRunnerEvents { get; }

        /// <summary>
        /// Gets the unique id of the test run.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the current status of the test run.
        /// </summary>
        TestRunStatus Status { get; }

        /// <summary>
        /// Gets the test report.
        /// </summary>
        /// <remarks>
        /// Since the report may be updated concurrently while a test is in progress, it is
        /// protected by a <see cref="LockBox{T}" />
        /// </remarks>
        /// <returns>The test report</returns>
        LockBox<Report> Report { get; }

        /// <summary>
        /// <para>
        /// Adds a test runner extension that will be registered with the <see cref="ITestRunner" /> when the test starts.
        /// </para>
        /// </summary>
        /// <param name="extension">The test runner extension</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extension"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the test run has already started</exception>
        void AddExtension(ITestRunnerExtension extension);

        /// <summary>
        /// Gets an Html formatted copy of the report.
        /// </summary>
        /// <remarks>
        /// If the report has not changed and has already been formatted, this method may
        /// return the cached copy.
        /// </remarks>
        /// <param name="condensed">True if a condensed HTML report should be produced</param>
        /// <returns>Path information for the HTML file that was generated</returns>
        FileInfo GetHtmlFormattedReport(bool condensed);

        /// <summary>
        /// Asynchronously starts the test run.
        /// </summary>
        void Start();

        /// <summary>
        /// Asynchronously stops the test run, canceling it if not already finished.
        /// </summary>
        void Stop();

        /// <summary>
        /// Waits for the test run to complete.
        /// </summary>
        /// <param name="timeSpan">The timeout or null if none</param>
        /// <returns>True if the test run completed, false if a timeout occurred</returns>
        bool WaitForCompletion(TimeSpan? timeSpan);
    }
}
