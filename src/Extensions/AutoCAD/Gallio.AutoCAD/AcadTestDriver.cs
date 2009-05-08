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
using System.Globalization;
using System.Runtime.Remoting;
using System.Threading;
using Gallio.Common.Policies;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Messages;
using Gallio.Model.Serialization;
using Gallio.Runner;
using Gallio.Runner.Drivers;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// The <c>AcadTestDriver</c> communicates with a <seealso cref="RemoteAcadTestDriver"/>
    /// running in the <c>acad.exe</c> process.
    /// </summary>
    public class AcadTestDriver : BaseTestDriver
    {
        private IAcadProcess process;
        private Runner.Drivers.RemoteTestDriver remoteTestDriver;
        private KeepAlive keepAlive;

        /// <summary>
        /// Initializes a new <see cref="AcadTestDriver"/> instance.
        /// </summary>
        /// <param name="process">An <see cref="IAcadProcess"/> instance.</param>
        /// <param name="remoteDriver">
        /// The <see cref="IRemoteTestDriver"/> instance from the specified <paramref name="process"/>.
        /// </param>
        public AcadTestDriver(IAcadProcess process, IRemoteTestDriver remoteDriver)
        {
            if (process == null)
                throw new ArgumentNullException("process");
            if (remoteDriver == null)
                throw new ArgumentNullException("remoteDriver");
            this.process = process;
            this.remoteTestDriver = new Runner.Drivers.RemoteTestDriver(remoteDriver);
            this.keepAlive = new KeepAlive(remoteDriver);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (keepAlive != null)
                {
                    keepAlive.Dispose();
                    keepAlive = null;
                }
                if (remoteTestDriver != null)
                {
                    remoteTestDriver.Dispose();
                    remoteTestDriver = null;
                }
                if (process != null)
                {
                    process.Dispose();
                    process = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        protected override void InitializeImpl(RuntimeSetup runtimeSetup, TestRunnerOptions testRunnerOptions, ILogger logger)
        {
            base.InitializeImpl(runtimeSetup, testRunnerOptions, logger);
            remoteTestDriver.Initialize(runtimeSetup, testRunnerOptions, logger);
        }

        /// <inheritdoc/>
        protected override void ExploreImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor)
        {
            remoteTestDriver.Explore(testPackageConfig, testExplorationOptions, testExplorationListener, progressMonitor);
        }

        /// <inheritdoc/>
        protected override void RunImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor)
        {
            // AutoCAD uses fibers.  Inform the test harness not to switch threads.
            testExecutionOptions = testExecutionOptions.Copy();
            testExecutionOptions.SingleThreaded = true;

            remoteTestDriver.Run(testPackageConfig, testExplorationOptions, testExplorationListener, testExecutionOptions, testExecutionListener, progressMonitor);
        }

        private class KeepAlive : IDisposable
        {
            private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5);

            private readonly object pingLock = new object();
            private Timer pingTimer;
            private bool lastPingFailed;
            private bool pingInProgress;
            private IRemoteTestDriver remoteDriver;

            public KeepAlive(IRemoteTestDriver remoteDriver)
            {
                this.remoteDriver = remoteDriver;
                StartPingTimer();
            }

            private void StartPingTimer()
            {
                lock (pingLock)
                {
                    pingTimer = new Timer(PingTimerElapsed, null, PingInterval, PingInterval);
                }
            }

            private void StopPingTimer()
            {
                lock (pingLock)
                {
                    if (pingTimer != null)
                    {
                        pingTimer.Dispose();
                        pingTimer = null;
                    }
                }
            }

            private void PingTimerElapsed(object state)
            {
                bool pinged = false;
                try
                {
                    lock (pingLock)
                    {
                        if (pingInProgress || pingTimer == null)
                            return;

                        pinged = true;
                        pingInProgress = true;
                    }

#if DEBUG // FIXME: For debugging the remoting starvation issue.  See Google Code issue #147.  Remove when fixed.
                    RuntimeAccessor.Logger.Log(LogSeverity.Debug, String.Format(CultureInfo.CurrentCulture, "[Ping] {0:o}", DateTime.Now));
#endif
                    var driver = remoteDriver;
                    if (driver != null)
                        driver.Ping();

                    lastPingFailed = false;
                }
                catch (RemotingException ex)
                {
                    if (!lastPingFailed)
                    {
                        UnhandledExceptionPolicy.Report("Could not send Ping message to the remote driver.", ex);
                        lastPingFailed = true;
                    }
                }
                finally
                {
                    if (pinged)
                        pingInProgress = false;
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                StopPingTimer();
                GC.SuppressFinalize(this);
            }
        }
    }
}
