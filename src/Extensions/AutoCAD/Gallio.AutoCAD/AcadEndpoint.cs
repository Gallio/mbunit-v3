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
