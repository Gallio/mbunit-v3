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
using System.IO;
using Gallio.Reflection;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Hosting;
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

            if (! ValidateArguments())
            {
                ShowHelp();
                return 1;
            }

            UnhandledExceptionPolicy.ReportUnhandledException += HandleUnhandledExceptionNotification;
            Console.WriteLine(String.Format("* Host started at {0}.", DateTime.Now));

            try
            {
                RunEndpoint();
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

        private bool ValidateArguments()
        {
            if (Arguments.IpcPortName != null && Arguments.TcpPortNumber >= 0
                || Arguments.IpcPortName == null && Arguments.TcpPortNumber < 0)
            {
                ShowErrorMessage("Either /ipc-port or /tcp-port must be specified, not both.");
                return false;
            }

            if (Arguments.ApplicationBaseDirectory != null && !Directory.Exists(Arguments.ApplicationBaseDirectory))
            {
                ShowErrorMessage("The specified application base directory does not exist.");
                return false;
            }

            if (Arguments.ConfigurationFile != null && !File.Exists(Arguments.ConfigurationFile))
            {
                ShowErrorMessage("The specified configuration file does not exist.");
                return false;
            }

            return true;
        }

        private void RunEndpoint()
        {
            AppDomain appDomain = null;
            try
            {
                appDomain = AppDomainUtils.CreateAppDomain(@"IsolatedProcessHost",
                    Arguments.ApplicationBaseDirectory, Arguments.ConfigurationFile, Arguments.ShadowCopy);

                Type endpointType = typeof(HostEndpoint);
                HostEndpoint endpoint = (HostEndpoint) appDomain.CreateInstanceFromAndUnwrap(
                            AssemblyUtils.GetAssemblyLocalPath(endpointType.Assembly), endpointType.FullName);

                if (Arguments.OwnerProcessId >= 0)
                {
                    if (! endpoint.SetOwnerProcess(Arguments.OwnerProcessId))
                    {
                        Console.WriteLine(String.Format("* The owner process with PID {0} does not appear to be running!", Arguments.OwnerProcessId));
                        return;
                    }
                }

                if (Arguments.IpcPortName != null)
                {
                    Console.WriteLine(String.Format("* Listening for connections on IPC port: '{0}'", Arguments.IpcPortName));
                    endpoint.InitializeIpcChannel(Arguments.IpcPortName);
                }
                else
                {
                    Console.WriteLine(String.Format("* Listening for connections on TCP port: '{0}'", Arguments.TcpPortNumber));
                    endpoint.InitializeTcpChannel(Arguments.TcpPortNumber);
                }

                TimeSpan? watchdogTimeout = Arguments.TimeoutSeconds <= 0
                    ? (TimeSpan?) null
                    : TimeSpan.FromSeconds(Arguments.TimeoutSeconds);

                HostTerminationReason reason = endpoint.Run(watchdogTimeout);

                switch (reason)
                {
                    case HostTerminationReason.WatchdogTimeout:
                        Console.WriteLine("* Watchdog timer expired!");
                        break;

                    case HostTerminationReason.Disowned:
                        Console.WriteLine("* Owner process terminated abruptly!");
                        break;

                    case HostTerminationReason.Disposed:
                        break;
                }
            }
            finally
            {
                if (appDomain != null)
                    AppDomain.Unload(appDomain);
            }
        }

        private void HandleUnhandledExceptionNotification(object sender, CorrelatedExceptionEventArgs e)
        {
            if (e.IsRecursive)
                return;

            Console.WriteLine(String.Format("* Unhandled exception: {0}", e.GetDescription()));
        }

        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        internal static int Main(string[] args)
        {
            return new HostProgram().Run(NativeConsole.Instance, args);
        }
    }
}
