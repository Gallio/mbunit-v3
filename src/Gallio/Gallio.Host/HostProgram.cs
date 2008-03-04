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
using Gallio.Hosting.ConsoleSupport;
using Gallio.Hosting;
using Gallio.Hosting.Channels;
using Gallio.Utilities;

namespace Gallio.Host
{
    /// <summary>
    /// The Gallio host process.
    /// </summary>
    public sealed class HostProgram : ConsoleProgram<HostArguments>
    {
        private readonly TimeSpan WatchdogTimeout = TimeSpan.FromSeconds(30);

        /// <inheritdoc />
        protected override int RunImpl(string[] args)
        {
            if (!ParseArguments(args))
            {
                ShowHelp();
                return -1;
            }

            UnhandledExceptionPolicy.ReportUnhandledException += HandleUnhandledExceptionNotification;
            Console.WriteLine(String.Format("* Host started at {0}.", DateTime.Now));

            try
            {
                using (BinaryIpcServerChannel serverChannel = new BinaryIpcServerChannel(Arguments.IpcPortName))
                {
                    using (new BinaryIpcClientChannel(Arguments.IpcPortName + @".Callback"))
                    {
                        using (RemoteHostService hostService = new RemoteHostService(WatchdogTimeout))
                        {
                            HostServiceChannelInterop.RegisterWithChannel(hostService, serverChannel);

                            Console.WriteLine(String.Format("* Listening for connections on IPC port: '{0}'", Arguments.IpcPortName));
                            hostService.WaitUntilDisposed();

                            if (hostService.WatchdogTimerExpired)
                                Console.WriteLine("* Watchdog timer expired!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("* Fatal exception: {0}", ExceptionUtils.SafeToString(ex)));
            }

            Console.WriteLine(String.Format("* Host stopped at {0}.", DateTime.Now));
            return 0;
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
