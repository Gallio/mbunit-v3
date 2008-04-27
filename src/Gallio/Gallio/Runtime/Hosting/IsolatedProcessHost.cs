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
using System.IO;
using System.Text;
using System.Threading;
using Gallio.Runtime.Logging;
using Gallio.Concurrency;
using Gallio.Runtime.Remoting;
using Gallio.Runtime;
using Gallio.Utilities;

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
        private const string HostAppFileName = "Gallio.Host.exe";

        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan PingInterval = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan JoinBeforeAbortTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan JoinAfterAbortTimeout = TimeSpan.FromSeconds(15);

        // FIXME: Large timeout to workaround the remoting starvation issue.  See Google Code issue #147.  Reduce value when fixed.
        private static readonly TimeSpan WatchdogTimeout = TimeSpan.FromSeconds(120);

        private readonly string uniqueId;
        private ProcessTask processTask;
        private IClientChannel clientChannel;
        private IServerChannel callbackChannel;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> 
        /// or <paramref name="logger"/> is null</exception>
        public IsolatedProcessHost(HostSetup hostSetup, ILogger logger)
            : base(hostSetup, logger, PingInterval)
        {
            uniqueId = Hash64.CreateUniqueHash().ToString();
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
                string diagnostics = "";
                if (processTask != null)
                    diagnostics = String.Format("  Exit code: {0}.  See log for more details.", processTask.Result);

                FreeResources(true);

                throw new HostException("Error attaching to the host process." + diagnostics, ex);
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
#if true
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
            hostArguments = "/tcp-port:33333";
            clientChannelFactory = delegate { return new BinaryTcpClientChannel("localhost", 33333); };
            callbackChannelFactory = delegate { return new BinaryTcpServerChannel("localhost", 33334); };
#endif
        }

        private void StartProcess(string hostConnectionArguments)
        {
            HostConfiguration editedHostConfiguration = HostSetup.Configuration.Copy();
            editedHostConfiguration.AddAssemblyBinding(typeof(IsolatedProcessHost).Assembly, false);

            string configurationFile = Path.GetTempFileName();
            File.WriteAllText(configurationFile, editedHostConfiguration.ToString());

            StringBuilder hostArguments = new StringBuilder();
            hostArguments.Append(hostConnectionArguments);
            hostArguments.Append(@" /timeout:").Append((int)WatchdogTimeout.TotalSeconds);
            hostArguments.Append(@" /owner-process:").Append(Process.GetCurrentProcess().Id);
            hostArguments.Append(@" ""/application-base-directory:").Append(FileUtils.StripTrailingBackslash(HostSetup.ApplicationBaseDirectory)).Append('"');
            hostArguments.Append(@" ""/configuration-file:").Append(configurationFile).Append('"');
            if (HostSetup.ShadowCopy)
                hostArguments.Append(@" /shadow-copy");

            processTask = CreateProcessTask(GetInstalledHostProcessPath(), hostArguments.ToString(), HostSetup.WorkingDirectory);
            processTask.CaptureConsoleOutput = true;
            processTask.CaptureConsoleError = true;
            processTask.ConsoleOutputDataReceived += LogConsoleOutput;
            processTask.ConsoleErrorDataReceived += LogConsoleError;
            processTask.Terminated += LogExitCode;

            processTask.Start();
        }

        private void LogConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                Logger.Log(LogSeverity.Info, e.Data);
        }

        private void LogConsoleError(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                Logger.Log(LogSeverity.Error, e.Data);
        }

        private void LogExitCode(object sender, EventArgs e)
        {
            ProcessTask processTask = (ProcessTask)sender;
            Logger.Log(LogSeverity.Debug, String.Format("* Host process exit code: {0}", processTask.ExitCode));
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
                    processTask.Join(JoinBeforeAbortTimeout);

                processTask.Abort();
                processTask.Join(JoinAfterAbortTimeout);
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
        }

        private static string GetInstalledHostProcessPath()
        {
            string hostProcessPath = Path.Combine(RuntimeAccessor.InstallationPath, HostAppFileName);
            if (!File.Exists(hostProcessPath))
                throw new HostException(String.Format("Could not find the installed host application in '{0}'.", hostProcessPath));

            return hostProcessPath;
        }
    }
}
