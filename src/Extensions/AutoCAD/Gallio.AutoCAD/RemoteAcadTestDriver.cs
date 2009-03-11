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
using System.ComponentModel;
using System.Threading;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Messages;
using Gallio.Model.Serialization;
using Gallio.Runner.Drivers;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Implementation of <see cref="ITestDriver"/> that runs within an AutoCAD process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// All <see cref="ITestDriver"/> method calls block until they can be processed by
    /// the main AutoCAD thread. Multiple calls received simultaneously will be queued
    /// and processed in arbitrary order.
    /// </para>
    /// <para>
    /// <see cref="IRemoteTestDriver.Ping"/> and <see cref="IRemoteTestDriver.Shutdown"/>
    /// are processed immediately.
    /// </para>
    /// <para>
    /// Each call is serviced by the thread running in the <see cref="WaitForShutdown"/>
    /// method. If <c>WaitForShutdown</c> isn't running any calls to <c>ITestDriver</c>
    /// methods will throw <c>InvalidOperationException</c>.
    /// </para>
    /// </remarks>
    /// <seealso cref="Gallio.AutoCAD.AcadEndpoint"/>
    public class RemoteAcadTestDriver : RemoteTestDriver, ISynchronizeInvoke
    {
        private static class State
        {
            public const int Starting = 0;
            public const int Running = 1;
            public const int Shutdown = 2;
        }

        ///<summary>The name of the .NET remoting service.</summary>
        public const string ServiceName = "AcadTestDriver";

        private Runner.Drivers.RemoteTestDriver remoteTestDriver;
        private AcadThread acadThread;
        private int state;

        /// <summary>
        /// Creates a new <see cref="RemoteAcadTestDriver"/>.
        /// </summary>
        /// <param name="pingTimeout">
        /// The maximum amount of time to wait between <see cref="RemoteTestDriver.Ping"/>
        /// calls before <see cref="RemoteTestDriver.Shutdown"/> is called automatically.
        /// Specify <c>null</c> to disable auto-shutdown.
        /// </param>
        /// <param name="testDriver">
        /// All <see cref="ITestDriver"/> calls will be delegated to this instance.
        /// </param>
        public RemoteAcadTestDriver(TimeSpan? pingTimeout, ITestDriver testDriver)
            : base(pingTimeout)
        {
            if (testDriver == null)
                throw new ArgumentNullException("testDriver");

            state = State.Starting;
            acadThread = new AcadThread();
            remoteTestDriver = new Runner.Drivers.RemoteTestDriver(testDriver);
        }

        /// <inheritdoc />
        protected override void InitializeImpl(RuntimeSetup runtimeSetup, ILogger logger)
        {
            base.InitializeImpl(runtimeSetup, logger);

            remoteTestDriver.Initialize(runtimeSetup, logger);
        }

        /// <inheritdoc />
        protected override void ExploreImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor)
        {
            Sync.Invoke(this, () => remoteTestDriver.Explore(testPackageConfig, testExplorationOptions, testExplorationListener, progressMonitor));
        }

        /// <inheritdoc />
        protected override void RunImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor)
        {
            Sync.Invoke(this, () => remoteTestDriver.Run(testPackageConfig, testExplorationOptions, testExplorationListener, testExecutionOptions, testExecutionListener, progressMonitor));
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Shutdown();

                if (remoteTestDriver != null)
                {
                    remoteTestDriver.Dispose();
                    remoteTestDriver = null;
                }

                if (acadThread != null)
                {
                    acadThread.Dispose();
                    acadThread = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override void Shutdown()
        {
            if (state != State.Shutdown)
            {
                if (Interlocked.Exchange(ref state, State.Shutdown) != State.Shutdown)
                {
                    acadThread.Shutdown();
                }
            }
            base.Shutdown();
        }

        /// <summary>
        /// <para>
        /// Blocks the calling thread until <see cref="Shutdown"/> is called.
        /// </para>
        /// <para>
        /// All incoming calls to the <see cref="ITestDriver"/> interface will
        /// be serviced on the thread that calls <c>WaitForShutdown</c>.
        /// </para>
        /// </summary>
        public void WaitForShutdown()
        {
            switch (Interlocked.CompareExchange(ref state, State.Running, State.Starting))
            {
                case State.Starting:
                    acadThread.Run();
                    break;
                case State.Running:
                    throw new InvalidOperationException("Currently waiting on a different thread.");
                case State.Shutdown:
                    return;
            }
        }

        /// <inheritdoc/>
        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            return BeginInvokeImpl(method, args);
        }

        /// <summary>
        /// Throws not implemented exception by default.
        /// </summary>
        protected virtual IAsyncResult BeginInvokeImpl(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            return EndInvokeImpl(result);
        }

        /// <summary>
        /// Throws not implemented exception by default.
        /// </summary>
        protected virtual object EndInvokeImpl(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public object Invoke(Delegate method, object[] args)
        {
            return acadThread.Invoke(method, args);
        }

        /// <inheritdoc/>
        public bool InvokeRequired
        {
            get { return true; }
        }
    }
}
