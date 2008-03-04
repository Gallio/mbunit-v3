// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Runtime.Remoting;
using System.Threading;
using Castle.Core.Logging;

namespace Gallio.Hosting
{
    /// <summary>
    /// A host service is a <see cref="MarshalByRefObject" /> implementation of
    /// <see cref="IHostService" /> suitable for access across a remoting channel.
    /// </summary>
    /// <see cref="HostServiceChannelInterop"/>
    public class RemoteHostService : LongLivedMarshalByRefObject, IHostService
    {
        private readonly object watchdogLock = new object();
        private readonly TimeSpan? watchdogTimeout;
        private Timer watchdogTimer;
        private bool watchdogTimerExpired;

        private readonly ManualResetEvent shutdownEvent;

        /// <summary>
        /// Creates a host service.
        /// </summary>
        /// <param name="watchdogTimeout">The maximum amount of time to wait for a ping
        /// before automatically disposing the host service, or null if there should be no timeout</param>
        public RemoteHostService(TimeSpan? watchdogTimeout)
        {
            this.watchdogTimeout = watchdogTimeout;

            shutdownEvent = new ManualResetEvent(false);

            StartWatchdogTimer();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            StopWatchdogTimer();

            shutdownEvent.Set();
        }

        /// <summary>
        /// Returns true if the watchdog timer expired.
        /// </summary>
        public bool WatchdogTimerExpired
        {
            get { return watchdogTimerExpired = true; }
        }

        /// <summary>
        /// Waits until the host service is disposed or a ping timeout occurs.
        /// </summary>
        public void WaitUntilDisposed()
        {
            shutdownEvent.WaitOne();
        }

        /// <inheritdoc />
        public void Ping()
        {
            ResetWatchdogTimer();
        }

        /// <inheritdoc />
        public void DoCallback(CrossAppDomainDelegate callback)
        {
            callback();
        }

        /// <inheritdoc />
        public ObjectHandle CreateInstance(string assemblyName, string typeName)
        {
            return Activator.CreateInstance(assemblyName, typeName);
        }

        /// <inheritdoc />
        public ObjectHandle CreateInstanceFrom(string assemblyPath, string typeName)
        {
            return Activator.CreateInstanceFrom(assemblyPath, typeName);
        }

        /// <inheritdoc />
        public void InitializeRuntime(RuntimeSetup runtimeSetup, ILogger logger)
        {
            Runtime.Initialize(runtimeSetup, logger);
        }

        /// <inheritdoc />
        public void ShutdownRuntime()
        {
            Runtime.Shutdown();
        }

        private void StartWatchdogTimer()
        {
            if (!watchdogTimeout.HasValue)
                return;

            lock (watchdogLock)
            {
                watchdogTimer = new Timer(HandleWatchdogTimerExpired, null, watchdogTimeout.Value, TimeSpan.FromMilliseconds(-1));
            }
        }

        private void StopWatchdogTimer()
        {
            lock (watchdogLock)
            {
                if (watchdogTimer != null)
                {
                    watchdogTimer.Dispose();
                    watchdogTimer = null;
                }
            }
        }

        private void ResetWatchdogTimer()
        {
            lock (watchdogLock)
            {
                if (watchdogTimer != null)
                    watchdogTimer.Change(watchdogTimeout.Value, TimeSpan.FromMilliseconds(-1));
            }
        }

        private void HandleWatchdogTimerExpired(object state)
        {
            watchdogTimerExpired = true;
            Dispose();
        }
    }
}
