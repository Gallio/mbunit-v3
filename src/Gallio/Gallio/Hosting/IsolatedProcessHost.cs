// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Threading;
using Castle.Core.Logging;
using Gallio.Concurrency;
using Gallio.Hosting.Channels;
using Gallio.Utilities;

namespace Gallio.Hosting
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
        private static readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan JoinBeforeAbortTimeout = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan JoinAfterAbortTimeout = TimeSpan.FromSeconds(5);

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
            : base(hostSetup, logger)
        {
        }

        /// <inheritdoc />
        protected override void InitializeImpl()
        {
            try
            {
                string portName = @"IsolatedProcessHost." + Hash64.CreateUniqueHash();

                StartProcess(portName);
                EnsureProcessIsRunning();

                clientChannel = new BinaryIpcClientChannel(portName);
                callbackChannel = new BinaryIpcServerChannel(portName + ".Callback");

                IHostService hostService = HostServiceChannelInterop.GetRemoteHostService(clientChannel);
                WaitUntilReady(hostService);

                if (HostSetup.ShadowCopy)
                    hostService.DoCallback(RemotelyEnableShadowCopy);

                ConfigureHostService(hostService, PingTimeout);
            }
            catch (Exception ex)
            {
                string diagnostics = "";
                if (processTask != null)
                    diagnostics = String.Format("\n\nConsole Output:\n{0}\n\nConsole Error:\n{1}\n\nResult: {2}",
                        processTask.ConsoleOutput, processTask.ConsoleError, processTask.Result);

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

        private void StartProcess(string portName)
        {
            HostApplicationProfile profile = new HostApplicationProfile(HostSetup, portName);
            try
            {
                profile.Initialize();

                string arguments = "/ipc:" + portName;

                processTask = CreateProcessTask(profile.HostProcessPath, arguments, HostSetup.WorkingDirectory);
                processTask.CaptureConsoleOutput = true;
                processTask.CaptureConsoleError = true;
                processTask.ConsoleOutputDataReceived += LogConsoleOutput;
                processTask.ConsoleErrorDataReceived += LogConsoleError;
                processTask.Terminated += LogExitCode;
                processTask.Terminated += delegate { profile.Dispose(); };

                processTask.Start();
            }
            catch (Exception)
            {
                profile.Dispose();
                throw;
            }
        }

        private void LogConsoleOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                Logger.Info(e.Data);
        }

        private void LogConsoleError(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
                Logger.Error(e.Data);
        }

        private void LogExitCode(object sender, EventArgs e)
        {
            ProcessTask processTask = (ProcessTask)sender;
            Logger.Debug("Host Process Exit Code: {0}", processTask.ExitCode);
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

        private static void RemotelyEnableShadowCopy()
        {
            // Note: These functions have been deprecated but they are our only choice for configuring
            //       shadow copying once the application has been loaded short of creating another AppDomain.
#pragma warning disable 618
            AppDomain.CurrentDomain.SetShadowCopyFiles();
            AppDomain.CurrentDomain.SetShadowCopyPath(null);
#pragma warning restore 618
        }

        private sealed class HostApplicationProfile : IDisposable
        {
            private const string HostAppFileName = "Gallio.Host.exe";

            private readonly HostSetup hostSetup;
            private readonly string hostAppPath;
            private readonly string hostConfigPath;

            public HostApplicationProfile(HostSetup hostSetup, string portName)
            {
                this.hostSetup = hostSetup;

                hostAppPath = Path.Combine(hostSetup.ApplicationBaseDirectory, "Gallio.Host." + portName + ".tmp");
                hostConfigPath = hostAppPath + ".config";
            }

            public string HostProcessPath
            {
                get { return hostAppPath; }
            }

            public void Initialize()
            {
                string installedHostProcessPath = GetInstalledHostProcessPath();

                try
                {
                    File.Copy(installedHostProcessPath, hostAppPath);

                    string configurationXml = hostSetup.Configuration.ToString();
                    File.WriteAllText(hostConfigPath, configurationXml);
                }
                catch (Exception ex)
                {
                    throw new HostException(String.Format("Could not copy the configured host application to '{0}'.", hostAppPath), ex);
                }
            }

            public void Dispose()
            {
                SafeDelete(hostAppPath);
                SafeDelete(hostConfigPath);
            }

            private static void SafeDelete(string path)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);
                }
                catch (FileNotFoundException)
                {
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report(String.Format("Could not temporary file '{0}'.", path), ex);
                }
            }

            private static string GetInstalledHostProcessPath()
            {
                string hostProcessPath = Path.Combine(Loader.InstallationPath, HostAppFileName);
                if (!File.Exists(hostProcessPath))
                    throw new HostException(String.Format("Could not find the installed host application in '{0}'.", hostProcessPath));

                return hostProcessPath;
            }
        }
    }
}
