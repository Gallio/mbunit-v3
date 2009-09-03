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
using System.Diagnostics;
using System.Threading;
using Gallio.Runtime.Logging;
using Gallio.Runtime;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// A remotely accessible host service.
    /// </summary>
    /// <see cref="HostServiceChannelInterop"/>
    public class RemoteHostService : BaseHostService, IRemoteHostService
    {
        private readonly object watchdogLock = new object();
        private readonly int watchdogTimeoutMilliseconds;
        private Timer watchdogTimer;
        private bool watchdogTimerExpired;

        private readonly ManualResetEvent shutdownEvent;

        /// <summary>
        /// Creates a host service.
        /// </summary>
        /// <param name="watchdogTimeout">The maximum amount of time to wait for a ping
        /// before automatically disposing the host service, or null if there should be no timeout.</param>
        public RemoteHostService(TimeSpan? watchdogTimeout)
        {
            if (watchdogTimeout.HasValue)
                watchdogTimeoutMilliseconds = (int)watchdogTimeout.Value.TotalMilliseconds;

            shutdownEvent = new ManualResetEvent(false);

            StartWatchdogTimer();
        }

        /// <summary>
        /// Returns true if the watchdog timer expired.
        /// </summary>
        public bool WatchdogTimerExpired
        {
            get { return watchdogTimerExpired; }
        }

        /// <summary>
        /// Waits until the host service is shutdown or a ping timeout occurs.
        /// </summary>
        public void WaitUntilShutdown()
        {
            shutdownEvent.WaitOne();
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            StopWatchdogTimer();

            shutdownEvent.Set();
        }

        /// <inheritdoc />
        protected override void PingImpl()
        {
#if DEBUG // FIXME: For debugging the remoting starvation issue.  See Google Code issue #147.  Remove when fixed.
            RuntimeAccessor.Logger.Log(LogSeverity.Debug, String.Format("[Pong] {0:o}", DateTime.Now));
#endif

            ResetWatchdogTimer();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Shutdown();
        }

        private void StartWatchdogTimer()
        {
            if (watchdogTimeoutMilliseconds <= 0)
                return;

            lock (watchdogLock)
            {
                watchdogTimer = new Timer(HandleWatchdogTimerExpired, null, watchdogTimeoutMilliseconds, Timeout.Infinite);
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
                    watchdogTimer.Change(watchdogTimeoutMilliseconds, Timeout.Infinite);
            }
        }

        private void HandleWatchdogTimerExpired(object state)
        {
            if (! Debugger.IsAttached)
            {
                watchdogTimerExpired = true;
                Shutdown();
            }
        }
    }
}
