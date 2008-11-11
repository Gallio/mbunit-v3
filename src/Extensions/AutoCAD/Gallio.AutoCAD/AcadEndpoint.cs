using System;
using System.Diagnostics;
using Gallio.Runner.Drivers;
using Gallio.Runtime.Remoting;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Supports creating a remoting endpoint and providing a
    /// <see cref="IRemoteTestDriver"/> service from within an AutoCAD process.
    /// </summary>
    public class AcadEndpoint : IDisposable
    {
        private IServerChannel serverChannel;
        private IClientChannel callbackChannel;
        private Process ownerProcess;

        /// <summary>
        /// Initializes a new <see cref="AcadEndpoint"/> object.
        /// </summary>
        /// <param name="ipcPortName">The name of the IPC port to create.</param>
        /// <param name="ownerProcess">The process that spawned AutoCAD.</param>
        public AcadEndpoint(string ipcPortName, Process ownerProcess)
        {
            if (ipcPortName == null)
                throw new ArgumentNullException("ipcPortName");
            if (ownerProcess == null)
                throw new ArgumentNullException("ownerProcess");

            this.ownerProcess = ownerProcess;
            serverChannel = new BinaryIpcServerChannel(ipcPortName);
            callbackChannel = new BinaryIpcClientChannel(ipcPortName + @".Callback");
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="AcadEndpoint"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (serverChannel != null)
                {
                    serverChannel.Dispose();
                    serverChannel = null;
                }

                if (callbackChannel != null)
                {
                    callbackChannel.Dispose();
                    callbackChannel = null;
                }
            }
        }

        /// <summary>
        /// Registers a <see cref="IRemoteTestDriver"/> service and blocks
        /// the current thread until <see cref="IRemoteTestDriver.Shutdown"/>
        /// is called.
        /// </summary>
        /// <param name="pingTimeout">
        /// The amount of time to wait between pings from Gallio before it's
        /// considered unresponsive.
        /// </param>
        public void Run(TimeSpan pingTimeout)
        {
            using (RemoteAcadTestDriver driver = new RemoteAcadTestDriver(pingTimeout, new LocalTestDriver()))
            {
                if (ownerProcess != null)
                {
                    ownerProcess.Exited += delegate { driver.Shutdown(); };
                    ownerProcess.EnableRaisingEvents = true;
                }

                if (ownerProcess == null || !ownerProcess.HasExited)
                {
                    serverChannel.RegisterService(RemoteAcadTestDriver.ServiceName, driver);
                    driver.WaitForShutdown();
                }
            }
        }

    }
}
