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
using System.Diagnostics;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Remoting;

namespace Gallio.Host
{
    internal class HostEndpoint : LongLivedMarshalByRefObject, IDisposable
    {
        private IServerChannel serverChannel;
        private IClientChannel callbackChannel;

        private Process ownerProcess;

        public void Dispose()
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

        public void InitializeIpcChannel(string portName)
        {
            serverChannel = new BinaryIpcServerChannel(portName);
            callbackChannel = new BinaryIpcClientChannel(portName + @".Callback");
        }

        public void InitializeTcpChannel(int portNumber)
        {
            serverChannel = new BinaryTcpServerChannel("localhost", portNumber);
            callbackChannel = new BinaryTcpClientChannel("localhost", portNumber);
        }

        public bool SetOwnerProcess(int processId)
        {
            try
            {
                ownerProcess = Process.GetProcessById(processId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public HostTerminationReason Run(TimeSpan? watchdogTimeout)
        {
            using (RemoteHostService hostService = new RemoteHostService(watchdogTimeout))
            {
                if (ownerProcess != null)
                {
                    ownerProcess.Exited += delegate { hostService.Dispose(); };
                    ownerProcess.EnableRaisingEvents = true;
                }

                if (ownerProcess == null || !ownerProcess.HasExited)
                {
                    HostServiceChannelInterop.RegisterWithChannel(hostService, serverChannel);
                    hostService.WaitUntilShutdown();
                }

                if (hostService.WatchdogTimerExpired)
                    return HostTerminationReason.WatchdogTimeout;

                if (ownerProcess != null && ownerProcess.HasExited)
                    return HostTerminationReason.Disowned;
            }

            return HostTerminationReason.Disposed;
        }
    }
}
