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
#define USE_IPC

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Common.IO;
using Gallio.Common.Platform;
using Gallio.Runtime.Logging;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Remoting;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// <para>
    /// An isolated process host is a <see cref="IHost" /> that runs code within a
    /// new external process.  Communication with the external process occurs over
    /// an inter-process communication channel.
    /// </para>
    /// <para>
    /// The host application is copied to a unique temporary folder and configured
    /// in place according to the <see cref="HostSetup" />.  Then it is launched
    /// and connected to with inter-process communication.  The process is pinged
    /// periodically by the <see cref="BinaryIpcClientChannel" />.  Therefore it
    /// can be configured to self-terminate when it looks like the connection
    /// has been severed.
    /// </para>
    /// </summary>
    public class IsolatedProcessHost : RemoteHost
    {
        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan JoinBeforeAbortTimeout = TimeSpan.FromMinutes(10); // This timeout is purposely long to allow code coverage tools to finish up.
        private static readonly TimeSpan JoinBeforeAbortWarningTimeout = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan JoinAfterAbortTimeout = TimeSpan.FromSeconds(15);

        // FIXME: Large timeout to workaround the remoting starvation issue.  See Google Code issue #147.  Reduce value when fixed.
        private static readonly TimeSpan WatchdogTimeout = TimeSpan.FromSeconds(120);

        private readonly string runtimePath;

        private readonly string uniqueId;
        private string temporaryConfigurationFilePath;
        private ProcessTask processTask;
        private IClientChannel clientChannel;
        private IServerChannel callbackChannel;
        private SeverityPrefixParser severityPrefixParser;

        private const int logConsoleOutputBufferTimeoutMilliseconds = 100;
        private readonly Timer logConsoleOutputBufferTimer;
        private LogSeverity logConsoleOutputBufferedMessageSeverity;
        private string logConsoleOutputBufferedMessage;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <param name="runtimePath">The runtime path where the hosting executable will be found</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> 
        /// <paramref name="logger"/>, or <paramref name="runtimePath"/> is null</exception>
        public IsolatedProcessHost(HostSetup hostSetup, ILogger logger, string runtimePath)
            : base(hostSetup, logger, PingInterval)
        {
            if (runtimePath == null)
                throw new ArgumentNullException("runtimePath");

            this.runtimePath = runtimePath;
            uniqueId = Hash64.CreateUniqueHash().ToString();

            logConsoleOutputBufferTimer = new Timer(LogConsoleOutputBufferTimeoutExpired);
        }

        /// <inheritdoc />
        protected override IRemoteHostService AcquireRemoteHostService()
        {
            try
            {
                string hostConnectionArguments;
                Func<IClientChannel> clientChannelFactory;
                Func<IServerChannel> callbackChannelFactory;
                PrepareConnection(uniqueId, out hostConnectionArguments, out clientChannelFactory, out callbackChannelFactory);

                StartProcess(hostConnectionArguments);
                EnsureProcessIsRunning();

                clientChannel = clientChannelFactory();
                callbackChannel = callbackChannelFactory();

                IRemoteHostService hostService = HostServiceChannelInterop.GetRemoteHostService(clientChannel);
                WaitUntilReady(hostService);
                return hostService;
            }
            catch (Exception ex)
            {
                FreeResources(true);
                throw new HostException("Error attaching to the host process.", ex);
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                FreeResources(false);
        }

        /// <summary>
        /// Creates the process task to start the process.
        /// </summary>
        /// <remarks>
        /// This method can be overridden to change how the process is started.
        /// </remarks>
        /// <param name="executablePath">The executable path</param>
        /// <param name="arguments">The command-line arguments</param>
        /// <param name="workingDirectory">The working directory</param>
        /// <returns>The process task</returns>
        protected virtual ProcessTask CreateProcessTask(string executablePath, string arguments, string workingDirectory)
        {
            return new ProcessTask(executablePath, arguments, workingDirectory);
        }

        /// <summary>
        /// Prepares the parameters for the remote connection.
        /// </summary>
        /// <param name="uniqueId">The unique id of the host</param>
        /// <param name="hostConnectionArguments">Set to the host application arguments used to configure its server channel</param>
        /// <param name="clientChannelFactory">Set to a factory used to create the local client channel</param>
        /// <param name="callbackChannelFactory">Set to a factory used to create the local server channel to allow the remote host to call back to this one</param>
        protected virtual void PrepareConnection(string uniqueId, out string hostConnectionArguments,
            out Func<IClientChannel> clientChannelFactory, out Func<IServerChannel> callbackChannelFactory)
        {
#if USE_IPC
            string portName = @"IsolatedProcessHost." + uniqueId;

            hostConnectionArguments = "/ipc-port:" + portName;
            clientChannelFactory = delegate { return new BinaryIpcClientChannel(portName); };
            callbackChannelFactory = delegate { return new BinaryIpcServerChannel(portName + ".Callback"); };
#else
            // The TCP channel implementation needs some work to become useful.
            // I implemented it partially as part of an effort to isolate the source of some
            // remoting timeouts that were occurring.  In due time the whole channel-based
            // remoting infrastructure will probably need to be overhauled to use truly
            // bidirectional channels.  -- Jeff.
            hostConnectionArguments = "/tcp-port:63217";
            clientChannelFactory = delegate { return new BinaryTcpClientChannel("localhost", 63217); };
            callbackChannelFactory = delegate { return new BinaryTcpServerChannel("localhost", 63218); };
#endif
        }

        private void StartProcess(string hostConnectionArguments)
        {
            CreateTemporaryConfigurationFile();

            StringBuilder hostArguments = new StringBuilder();
            hostArguments.Append(hostConnectionArguments);
            hostArguments.Append(@" /timeout:").Append((int)WatchdogTimeout.TotalSeconds);
            hostArguments.Append(@" /owner-process:").Append(Process.GetCurrentProcess().Id);
            if (HostSetup.ApplicationBaseDirectory != null)
                hostArguments.Append(@" /application-base-directory:""").Append(
                    FileUtils.StripTrailingBackslash(HostSetup.ApplicationBaseDirectory)).Append('"');
            hostArguments.Append(@" /configuration-file:""").Append(temporaryConfigurationFilePath).Append('"');
            if (HostSetup.ShadowCopy)
                hostArguments.Append(@" /shadow-copy");
            if (HostSetup.Debug)
                hostArguments.Append(@" /debug");
            hostArguments.Append(" /severity-prefix");

            severityPrefixParser = new SeverityPrefixParser();

            processTask = CreateProcessTask(GetInstalledHostProcessPath(), hostArguments.ToString(), HostSetup.WorkingDirectory ?? Environment.CurrentDirectory);
            processTask.CaptureConsoleOutput = true;
            processTask.CaptureConsoleError = true;
            processTask.ConsoleOutputDataReceived += LogConsoleOutput;
            processTask.ConsoleErrorDataReceived += LogConsoleError;
            processTask.Terminated += LogExitCode;

            // Force CLR runtime version.
            string runtimeVersion = HostSetup.RuntimeVersion;
            if (runtimeVersion == null)
                runtimeVersion = DotNetRuntimeSupport.MostRecentInstalledDotNetRuntimeVersion;

            if (!runtimeVersion.StartsWith("v"))
                runtimeVersion = "v" + runtimeVersion; // just in case, this is a common user error

            processTask.SetEnvironmentVariable("COMPLUS_Version", runtimeVersion);

            processTask.Start();
        }

        private void CreateTemporaryConfigurationFile()
        {
            try
            {
                HostSetup patchedSetup = HostSetup.Copy();
                patchedSetup.Configuration.AddAssemblyBinding(typeof(IsolatedProcessHost).Assembly, false);

                temporaryConfigurationFilePath = patchedSetup.WriteTemporaryConfigurationFile();
            }
            catch (Exception ex)
            {
                throw new HostException("Could not write the temporary configuration file.", ex);
            }
        }

        private void LogConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            lock (logConsoleOutputBufferTimer)
            {
                if (e.Data != null)
                {
                    LogSeverity severity;
                    string message;

                    if (severityPrefixParser.ParseLine(e.Data, out severity, out message))
                        LogConsoleOutputWriteBufferedMessageSync();
                    message = message.TrimEnd();

                    logConsoleOutputBufferedMessageSeverity = severity;
                    if (logConsoleOutputBufferedMessage == null)
                        logConsoleOutputBufferedMessage = message;
                    else
                        logConsoleOutputBufferedMessage = string.Concat(logConsoleOutputBufferedMessage, "\n", message);

                    logConsoleOutputBufferTimer.Change(logConsoleOutputBufferTimeoutMilliseconds, Timeout.Infinite);
                }
                else
                {
                    LogConsoleOutputWriteBufferedMessageSync();
                }
            }
        }

        private void LogConsoleOutputBufferTimeoutExpired(object dummy)
        {
            lock (logConsoleOutputBufferTimer)
            {
                LogConsoleOutputWriteBufferedMessageSync();
            }
        }

        private void LogConsoleOutputWriteBufferedMessageSync()
        {
            if (logConsoleOutputBufferedMessage != null)
            {
                Logger.Log(logConsoleOutputBufferedMessageSeverity, logConsoleOutputBufferedMessage);
                logConsoleOutputBufferedMessage = null;
            }
        }

        private void LogConsoleError(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Logger.Log(LogSeverity.Error, e.Data.TrimEnd());
            }
        }

        private void LogExitCode(object sender, TaskEventArgs e)
        {
            var processTask = (ProcessTask)e.Task;

            var exception = processTask.Result.Exception;
            if (exception != null)
            {
                Logger.Log(LogSeverity.Error, "Host process encountered an exception.", exception);
            }
            else
            {
                var diagnostics = new StringBuilder();
                int exitCode = processTask.ExitCode;
                diagnostics.AppendFormat("Host process exited with code: {0}", exitCode);

                string exitCodeDescription = processTask.ExitCodeDescription;
                if (exitCodeDescription != null)
                    diagnostics.Append(" (").Append(exitCodeDescription).Append(")");

                Logger.Log(exitCode != 0 ? LogSeverity.Error : LogSeverity.Info, diagnostics.ToString());
            }
        }

        private void WaitUntilReady(IHostService hostService)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (; ; )
            {
                EnsureProcessIsRunning();

                try
                {
                    hostService.Ping();
                    return;
                }
                catch (Exception)
                {
                    if (stopwatch.Elapsed >= ReadyTimeout)
                        throw;
                }

                EnsureProcessIsRunning();
                Thread.Sleep(ReadyPollInterval);
            }
        }

        private void EnsureProcessIsRunning()
        {
            if (! processTask.IsRunning)
                throw new HostException("The host process terminated abruptly.");
        }

        private void FreeResources(bool abortImmediately)
        {
            if (processTask != null)
            {
                if (! abortImmediately)
                {
                    if (!processTask.Join(JoinBeforeAbortWarningTimeout))
                    {
                        Logger.Log(LogSeverity.Info, "Waiting for the host process to terminate.");
                        if (!processTask.Join(JoinBeforeAbortTimeout - JoinBeforeAbortWarningTimeout))
                            Logger.Log(LogSeverity.Info, string.Format("Timed out after {0} minutes.", JoinBeforeAbortTimeout.TotalMinutes));
                    }
                }

                if (! processTask.Join(TimeSpan.Zero))
                {
                    Logger.Log(LogSeverity.Warning, "Forcibly killing the host process!");
                    processTask.Abort();
                    processTask.Join(JoinAfterAbortTimeout);
                }

                processTask = null;
            }

            if (clientChannel != null)
            {
                clientChannel.Dispose();
                clientChannel = null;
            }

            if (callbackChannel != null)
            {
                callbackChannel.Dispose();
                callbackChannel = null;
            }

            if (temporaryConfigurationFilePath != null)
            {
                File.Delete(temporaryConfigurationFilePath);
                temporaryConfigurationFilePath = null;
            }

            lock (logConsoleOutputBufferTimer)
            {
                logConsoleOutputBufferTimer.Dispose();
            }
        }

        private string GetInstalledHostProcessPath()
        {
            string hostProcessPath = Path.Combine(runtimePath, GetHostFileName(HostSetup.ProcessorArchitecture));
            if (!File.Exists(hostProcessPath))
                throw new HostException(String.Format("Could not find the installed host application in '{0}'.", hostProcessPath));

            return hostProcessPath;
        }

        private static string GetHostFileName(ProcessorArchitecture processorArchitecture)
        {
            // TODO: Should find a way to verify that Amd64 / IA64 are supported.
            switch (processorArchitecture)
            {
                case ProcessorArchitecture.None:
                case ProcessorArchitecture.MSIL:
                case ProcessorArchitecture.Amd64:
                case ProcessorArchitecture.IA64:
                    return "Gallio.Host.exe";

                case ProcessorArchitecture.X86:
                    return "Gallio.Host.x86.exe";

                default:
                    throw new ArgumentOutOfRangeException("processorArchitecture");
            }
        }
    }
}
