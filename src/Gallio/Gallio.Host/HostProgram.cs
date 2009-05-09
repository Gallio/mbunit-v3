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
using System.IO;
using Gallio.Common.Diagnostics;
using Gallio.Host.Properties;
using Gallio.Common.Platform;
using Gallio.Common.Reflection;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Hosting;
using Gallio.Runtime.Logging;

namespace Gallio.Host
{
    /// <summary>
    /// The Gallio host process.
    /// </summary>
    public sealed class HostProgram : ConsoleProgram<HostArguments>
    {
        /// <summary>
        /// Creates an instance of the program.
        /// </summary>
        public HostProgram()
        {
            ApplicationName = Resources.ApplicationName;
        }

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

            ILogger logger = new RichConsoleLogger(Console);
            if (Arguments.SeverityPrefix)
                logger = new SeverityPrefixLogger(logger);

            /* Already reported via other means.  The duplication is distracting.
            UnhandledExceptionPolicy.ReportUnhandledException += (sender, e) =>
            {
                if (! e.IsRecursive)
                    logger.Log(LogSeverity.Error, String.Format("Unhandled exception: {0}", e.GetDescription()));
            };
            */

            logger.Log(LogSeverity.Info, String.Format("Host started at {0}.", DateTime.Now));

            bool fatal = false;
            try
            {
                if (Arguments.Debug)
                    RunEndpointWithDebugger(logger);
                else
                    RunEndpoint(logger);
            }
            catch (Exception ex)
            {
                logger.Log(LogSeverity.Error, String.Format("Fatal exception: {0}", ExceptionUtils.SafeToString(ex)));
                fatal = true;
            }

            logger.Log(LogSeverity.Info, String.Format("Host stopped at {0}.", DateTime.Now));

            ForceExit(fatal);
            return 0;
        }

        private static void ForceExit(bool fatal)
        {
            // Force the host to terminate in case there are some recalcitrant foreground
            // threads still kicking around.

            if (DotNetRuntimeSupport.IsUsingMono && fatal)
            {
                // On Mono, we can encounter (unexplained) CannotUnloadAppDomainExceptions
                // which prevent the process from terminating even when we call Exit.
                // So in the interest of robustness, we give it a little shove...
                Process.GetCurrentProcess().Kill();
            }

            Environment.Exit(0);
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

        private void RunEndpointWithDebugger(ILogger logger)
        {
            Process currentProcess = Process.GetCurrentProcess();

            IDebuggerManager debuggerManager = new DefaultDebuggerManager(); // FIXME: Get from IoC
            IDebugger debugger = debuggerManager.GetDefaultDebugger();
            AttachDebuggerResult attachResult = AttachDebuggerResult.CouldNotAttach;
            try
            {
                if (!debugger.IsAttachedToProcess(currentProcess, logger))
                {
                    logger.Log(LogSeverity.Important, "Attaching the debugger to the host.");
                    attachResult = debugger.AttachToProcess(currentProcess, logger);
                    if (attachResult == AttachDebuggerResult.CouldNotAttach)
                        logger.Log(LogSeverity.Warning, "Could not attach debugger to the host.");
                }

                RunEndpoint(logger);
            }
            finally
            {
                if (attachResult == AttachDebuggerResult.Attached)
                {
                    logger.Log(LogSeverity.Important, "Detaching the debugger from the host.");
                    DetachDebuggerResult detachResult = debugger.DetachFromProcess(currentProcess, logger);
                    if (detachResult == DetachDebuggerResult.CouldNotDetach)
                        logger.Log(LogSeverity.Warning, "Could not detach debugger from the host.");
                }
            }
        }

        private void RunEndpoint(ILogger logger)
        {
            AppDomain appDomain = null;
            try
            {
                appDomain = AppDomainUtils.CreateAppDomain(@"Host",
                    Arguments.ApplicationBaseDirectory, Arguments.ConfigurationFile, Arguments.ShadowCopy);

                Type endpointType = typeof(HostEndpoint);
                using (HostEndpoint endpoint = (HostEndpoint)appDomain.CreateInstanceFromAndUnwrap(
                    AssemblyUtils.GetAssemblyLocalPath(endpointType.Assembly), endpointType.FullName))
                {
                    if (Arguments.OwnerProcessId >= 0)
                    {
                        if (!endpoint.SetOwnerProcess(Arguments.OwnerProcessId))
                        {
                            logger.Log(LogSeverity.Warning,
                                String.Format("The owner process with PID {0} does not appear to be running!",
                                    Arguments.OwnerProcessId));
                            return;
                        }
                    }

                    if (Arguments.IpcPortName != null)
                    {
                        logger.Log(LogSeverity.Debug, String.Format("Listening for connections on IPC port: '{0}'",
                            Arguments.IpcPortName));
                        endpoint.InitializeIpcChannel(Arguments.IpcPortName);
                    }
                    else
                    {
                        logger.Log(LogSeverity.Debug, String.Format("Listening for connections on TCP port: '{0}'",
                            Arguments.TcpPortNumber));
                        endpoint.InitializeTcpChannel(Arguments.TcpPortNumber);
                    }

                    TimeSpan? watchdogTimeout = Arguments.TimeoutSeconds <= 0
                        ? (TimeSpan?) null
                        : TimeSpan.FromSeconds(Arguments.TimeoutSeconds);

                    HostTerminationReason reason = endpoint.Run(watchdogTimeout);

                    switch (reason)
                    {
                        case HostTerminationReason.WatchdogTimeout:
                            logger.Log(LogSeverity.Warning, "Watchdog timer expired!");
                            break;

                        case HostTerminationReason.Disowned:
                            logger.Log(LogSeverity.Warning, "Owner process terminated abruptly!");
                            break;

                        case HostTerminationReason.Disposed:
                            break;
                    }
                }
            }
            finally
            {
                // For some reason this is throwing CannotUnloadAppDomainException on Mono 1.9.1.
                // After that happens, the process refuses to shut down normally.
                // -- Jeff.
                if (appDomain != null)
                    AppDomain.Unload(appDomain);
            }
        }

        [STAThread]
        //[LoaderOptimization(LoaderOptimization.MultiDomain)] // Disabled due to bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=95157
        internal static int Main(string[] args)
        {
            return new HostProgram().Run(NativeConsole.Instance, args);
        }
    }
}
