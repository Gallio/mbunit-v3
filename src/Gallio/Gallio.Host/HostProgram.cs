// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Remoting;
using Gallio.Runtime;
using Gallio.Utilities;

namespace Gallio.Host
{
    /// <summary>
    /// The Gallio host process.
    /// </summary>
    public sealed class HostProgram : ConsoleProgram<HostArguments>
    {
        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return 1;
            }

            if (Arguments.Help)
            {
                ShowHelp();
                return 0;
            }

            if (Arguments.IpcPortName != null && Arguments.TcpPortNumber >= 0
                || Arguments.IpcPortName == null && Arguments.TcpPortNumber < 0)
            {
                ShowErrorMessage("Either /ipc-port or /tcp-port must be specified, not both.");
                return 1;
            }

            UnhandledExceptionPolicy.ReportUnhandledException += HandleUnhandledExceptionNotification;
            Console.WriteLine(String.Format("* Host started at {0}.", DateTime.Now));

            try
            {
                InitializeAndRunHost();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("* Fatal exception: {0}", ExceptionUtils.SafeToString(ex)));
            }

            Console.WriteLine(String.Format("* Host stopped at {0}.", DateTime.Now));

            // Force the host to terminate in case there are some recalcitrant foreground
            // threads still kicking around.
            Environment.Exit(0);
            return 0;
        }

        private void InitializeAndRunHost()
        {
            IServerChannel serverChannel;
            IClientChannel callbackChannel;
            if (Arguments.IpcPortName != null)
            {
                Console.WriteLine(String.Format("* Listening for connections on IPC port: '{0}'", Arguments.IpcPortName));

                serverChannel = new BinaryIpcServerChannel(Arguments.IpcPortName);
                callbackChannel = new BinaryIpcClientChannel(Arguments.IpcPortName + @".Callback");
            }
            else
            {
                Console.WriteLine(String.Format("* Listening for connections on TCP port: '{0}'", Arguments.TcpPortNumber));

                serverChannel = new BinaryTcpServerChannel("localhost", Arguments.TcpPortNumber);
                callbackChannel = new BinaryTcpClientChannel("localhost", Arguments.TcpPortNumber);
            }

            Process ownerProcess = null;
            try
            {
                if (Arguments.OwnerProcessId >= 0)
                    ownerProcess = Process.GetProcessById(Arguments.OwnerProcessId);
            }
            catch (Exception)
            {
                Console.WriteLine(String.Format("* The owner process with PID {0} does not appear to be running!",
                    Arguments.OwnerProcessId));
                return;
            }

            TimeSpan? watchdogTimeout = Arguments.TimeoutSeconds <= 0 ? (TimeSpan?)null : TimeSpan.FromSeconds(Arguments.TimeoutSeconds);

            using (serverChannel)
            {
                using (callbackChannel)
                {
                    RunHost(serverChannel, watchdogTimeout, ownerProcess);
                }
            }
        }

        private void RunHost(IServerChannel serverChannel, TimeSpan? watchdogTimeout, Process ownerProcess)
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
                    hostService.WaitUntilDisposed();
                }

                if (hostService.WatchdogTimerExpired)
                    Console.WriteLine("* Watchdog timer expired!");

                if (ownerProcess != null && ownerProcess.HasExited)
                    Console.WriteLine("* Owner process terminated abruptly!");
            }
        }

        private void HandleUnhandledExceptionNotification(object sender, CorrelatedExceptionEventArgs e)
        {
            if (e.IsRecursive)
                return;

            Console.WriteLine("* Unhandled exception: " + e.GetDescription());
        }

        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        internal static int Main(string[] args)
        {
            return new HostProgram().Run(NativeConsole.Instance, args);
        }
    }
}
