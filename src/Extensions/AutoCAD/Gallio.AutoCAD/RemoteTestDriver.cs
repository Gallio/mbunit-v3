using System;
using System.Globalization;
using System.Threading;
using Gallio.Runner.Drivers;
using Gallio.Runtime;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// <para>
    /// Represents a <see cref="ITestDriver"/> executing in a remote environment.
    /// </para>
    /// <para>
    /// Provides optional support for "keep-alive" style lifecycle management.
    /// </para>
    /// </summary>
    public abstract class RemoteTestDriver : BaseTestDriver, IRemoteTestDriver
    {
        private Watchdog watchdog;

        /// <summary>
        /// Initializes a new <see cref="RemoteTestDriver"/>.
        /// </summary>
        /// <param name="pingTimeout">
        /// The maximum amount of time to wait between <see cref="Ping"/>
        /// calls before <see cref="Shutdown"/> is called automatically,
        /// or <c>null</c> to disable auto-shutdown.
        /// </param>
        protected RemoteTestDriver(TimeSpan? pingTimeout)
        {
            if (pingTimeout.HasValue)
            {
                watchdog = new Watchdog(this, pingTimeout.Value);
                watchdog.Start();
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable temp = watchdog;
                if (temp != null)
                {
                    temp.Dispose();
                    watchdog = null;
                }
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc/>
        public virtual void Ping()
        {
#if DEBUG // FIXME: For debugging the remoting starvation issue.  See Google Code issue #147.  Remove when fixed.
            RuntimeAccessor.Logger.Log(LogSeverity.Debug, String.Format(CultureInfo.CurrentCulture, "[Pong] {0:o}", DateTime.Now));
#endif
            Watchdog temp = watchdog;
            if (temp != null)
            {
                temp.Reset();
            }
        }

        /// <inheritdoc/>
        public virtual void Shutdown()
        {
            ShutdownInternal();
        }

        private void ShutdownInternal()
        {
            Watchdog temp = watchdog;
            if (temp != null)
                temp.Stop();
        }

        private class Watchdog : IDisposable
        {
            private RemoteTestDriver driver;
            private readonly object watchdogLock = new object();
            private readonly int watchdogTimeoutMilliseconds;
            private Timer watchdogTimer;

            public Watchdog(RemoteTestDriver driver, TimeSpan watchdogTimeout)
            {
                this.driver = driver;
                watchdogTimeoutMilliseconds = (int)watchdogTimeout.TotalMilliseconds;
            }

            public void Start()
            {
                if (watchdogTimeoutMilliseconds <= 0)
                    return;

                lock (watchdogLock)
                {
                    if (watchdogTimer == null)
                        watchdogTimer = new Timer(HandleWatchdogTimerExpired, null, watchdogTimeoutMilliseconds, Timeout.Infinite);
                }
            }

            public void Stop()
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

            public void Reset()
            {
                lock (watchdogLock)
                {
                    if (watchdogTimer != null)
                        watchdogTimer.Change(watchdogTimeoutMilliseconds, Timeout.Infinite);
                }
            }

            private void HandleWatchdogTimerExpired(object state)
            {
                driver.Shutdown();
            }

            void IDisposable.Dispose()
            {
                Stop();
                GC.SuppressFinalize(this);
            }
        }
    }
}
