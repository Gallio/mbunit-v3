// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Common.Remoting;
using Gallio.Runtime;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.Model.Isolation
{
    /// <summary>
    /// A hosted test isolation context created by <see cref="HostedTestIsolationProvider" />.
    /// </summary>
    public class HostedTestIsolationContext : BaseTestIsolationContext
    {
        private readonly IHostFactory hostFactory;
        private readonly TestIsolationOptions testIsolationOptions;
        private readonly ILogger logger;

        private delegate object RunIsolatedTaskDelegate(object[] args);

        /// <summary>
        /// Creates a hosted test isolation context.
        /// </summary>
        /// <param name="hostFactory">The host factory.</param>
        /// <param name="testIsolationOptions">The test isolation options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostFactory"/>,
        /// <paramref name="testIsolationOptions"/> or <paramref name="logger"/> is null.</exception>
        public HostedTestIsolationContext(IHostFactory hostFactory,
            TestIsolationOptions testIsolationOptions, ILogger logger)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (testIsolationOptions == null)
                throw new ArgumentNullException("testIsolationOptions");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.hostFactory = hostFactory;
            this.testIsolationOptions = testIsolationOptions;
            this.logger = logger;
        }

        /// <summary>
        /// Gets the host factory.
        /// </summary>
        public IHostFactory HostFactory
        {
            get { return hostFactory; }
        }

        /// <summary>
        /// Gets the test isolation options.
        /// </summary>
        public TestIsolationOptions TestIsolationOptions
        {
            get { return testIsolationOptions; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <inheritdoc />
        sealed protected override object RunIsolatedTaskImpl<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
        {
            hostSetup = hostSetup.Copy();
            foreach (var pair in testIsolationOptions.Properties)
                if (! hostSetup.Properties.ContainsKey(pair.Key))
                    hostSetup.AddProperty(pair.Key, pair.Value);

            return RunIsolatedTaskInHost<TIsolatedTask>(hostSetup, statusReporter, args);
        }

        /// <summary>
        /// Runs an isolated task in a host.
        /// </summary>
        /// <typeparam name="TIsolatedTask">The isolated task type.</typeparam>
        /// <param name="hostSetup">The host setup which includes the test isolation option properties copied down, not null.</param>
        /// <param name="statusReporter">The status reporter, not null.</param>
        /// <param name="args">The task arguments.</param>
        /// <returns>The task result.</returns>
        protected virtual object RunIsolatedTaskInHost<TIsolatedTask>(HostSetup hostSetup, StatusReporter statusReporter, object[] args)
            where TIsolatedTask : IsolatedTask
        {
            IHost host = null;
            try
            {
                statusReporter("Creating test host.");
                host = hostFactory.CreateHost(hostSetup, logger);

                RemoteLogger remoteLogger = new RemoteLogger(logger);
                RuntimeSetup runtimeSetup = RuntimeAccessor.Instance.GetRuntimeSetup().Copy();

                Shim shim = HostUtils.CreateInstance<Shim>(host);
                try
                {
                    statusReporter("Initializing the runtime.");
                    shim.Initialize(runtimeSetup, remoteLogger);
                    statusReporter("");

                    var isolatedTask = HostUtils.CreateInstance<TIsolatedTask>(host);

                    ManualResetEvent disconnectedEvent = new ManualResetEvent(false);
                    EventHandler disconnectedEventHandler = (sender, e) => disconnectedEvent.Set();
                    try
                    {
                        host.Disconnected += disconnectedEventHandler;

                        RunIsolatedTaskDelegate isolatedTaskDelegate = isolatedTask.Run;
                        IAsyncResult asyncResult = isolatedTaskDelegate.BeginInvoke(args, null, null);

                        WaitHandle.WaitAny(new[] { disconnectedEvent, asyncResult.AsyncWaitHandle });

                        if (asyncResult.IsCompleted)
                            return isolatedTaskDelegate.EndInvoke(asyncResult);

                        throw new TestIsolationException("The host disconnected or was terminated prematurely while the task was running.");
                    }
                    finally
                    {
                        host.Disconnected -= disconnectedEventHandler;
                    }
                }
                finally
                {
                    statusReporter("Shutting down the runtime.");
                    shim.Shutdown();

                    GC.KeepAlive(remoteLogger);
                }
            }
            finally
            {
                statusReporter("Disposing test host.");

                try
                {
                    if (host != null)
                        host.Dispose();
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("An exception occurred while disposing the test host.", ex);
                }

                statusReporter("");
            }
        }

        private sealed class Shim : LongLivedMarshalByRefObject
        {
            private bool initializedRuntime;

            public void Initialize(RuntimeSetup runtimeSetup, ILogger logger)
            {
                if (!RuntimeAccessor.IsInitialized)
                {
                    RuntimeBootstrap.Initialize(runtimeSetup, logger);
                    initializedRuntime = true;
                }
            }

            public void Shutdown()
            {
                if (initializedRuntime)
                {
                    initializedRuntime = false;
                    RuntimeBootstrap.Shutdown();
                }
            }
        }
    }
}