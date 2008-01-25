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
using Gallio.Concurrency;
using Gallio.Hosting.Channels;
using Gallio.Utilities;

namespace Gallio.Hosting
{
    /// <summary>
    /// <para>
    /// An isolated process host factory creates a <see cref="IHost" /> within a
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
    public class IsolatedProcessHostFactory : BaseHostFactory
    {
        private static readonly TimeSpan ReadyTimeout = TimeSpan.FromSeconds(15);
        private static readonly TimeSpan ReadyPollInterval = TimeSpan.FromSeconds(0.5);

        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup)
        {
            IClientChannel clientChannel = null;
            IServerChannel callbackChannel = null;
            ProcessTask processTask = null;
            IsolatedProcessHost host = null;
            try
            {
                string portName = @"IsolatedProcessHost." + Hash64.CreateUniqueHash();

                processTask = StartProcess(hostSetup, portName);
                clientChannel = new BinaryIpcClientChannel(portName);
                callbackChannel = new BinaryIpcServerChannel(portName + ".Callback");

                IRemoteHostService remoteHostService = HostServiceChannelInterop.GetRemoteHostService(clientChannel);
                WaitUntilReady(remoteHostService);

                host = new IsolatedProcessHost(remoteHostService, processTask, callbackChannel);
                return host;
            }
            catch (Exception ex)
            {
                string diagnostics = "";
                if (processTask != null)
                    diagnostics = String.Format("\n\nConsole Output:\n{0}\n\nConsole Error:\n{1}\n\nExit Code: {2}",
                        processTask.ConsoleOutput, processTask.ConsoleError,
                        processTask.IsRunning ? -1 : processTask.ExitCode);

                if (host != null)
                {
                    host.Dispose();
                }
                else if (processTask != null)
                {
                    processTask.Abort();
                }
                else if (clientChannel != null)
                {
                    clientChannel.Dispose();
                }

                if (callbackChannel != null)
                    callbackChannel.Dispose();

                throw new HostException("Error attaching to the host process." + diagnostics, ex);
            }
        }

        /// <summary>
        /// Creates the process task to start the process.
        /// </summary>
        /// <remarks>
        /// This method can be overridden to change how the process is started.
        /// </remarks>
        /// <param name="executablePath">The executable path</param>
        /// <param name="arguments">The command-line arguments</param>
        /// <returns>The process task</returns>
        protected virtual ProcessTask CreateProcessTask(string executablePath, string arguments)
        {
            return new ProcessTask(executablePath, arguments);
        }

        private ProcessTask StartProcess(HostSetup hostSetup, string portName)
        {
            HostApplicationProfile profile = new HostApplicationProfile(portName);
            try
            {
                profile.Configure(hostSetup);

                try
                {
                    string arguments = "/ipc:" + portName;

                    ProcessTask processTask = CreateProcessTask(profile.HostProcessPath, arguments);
                    processTask.LogStreamWriter = null;
                    processTask.CaptureConsoleError = true;
                    processTask.CaptureConsoleOutput = true;
                    processTask.Terminated += delegate { profile.Dispose(); };

                    processTask.Start();
                    return processTask;
                }
                catch (Exception ex)
                {
                    throw new HostException("Could not launch the host application as a new process.", ex);
                }
            }
            catch (Exception)
            {
                profile.Dispose();
                throw;
            }
        }

        private static void WaitUntilReady(IRemoteHostService host)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (; ; )
            {
                try
                {
                    host.Ping();
                    return;
                }
                catch (Exception)
                {
                    if (stopwatch.Elapsed >= ReadyTimeout)
                        throw;
                }

                Thread.Sleep(ReadyPollInterval);
            }
        }

        private sealed class HostApplicationProfile : IDisposable
        {
            private const string HostAppFileName = "Gallio.Host.exe";
            private const string ConfigFileNameSuffix = ".config";

            private readonly string configuredHostAppDirectory;
            private readonly string configuredHostAppPath;

            public HostApplicationProfile(string portName)
            {
                configuredHostAppDirectory = Path.Combine(Path.GetTempPath(), portName);
                configuredHostAppPath = Path.Combine(configuredHostAppDirectory, HostAppFileName);
            }

            public string HostProcessPath
            {
                get { return configuredHostAppPath; }
            }

            public void Configure(HostSetup hostSetup)
            {
                string installedHostProcessPath = GetInstalledHostProcessPath();

                try
                {
                    Directory.CreateDirectory(configuredHostAppDirectory);

                    if (! File.Exists(configuredHostAppPath))
                        File.Copy(installedHostProcessPath, configuredHostAppPath);

                    string configurationXml = hostSetup.Configuration.ToString();
                    File.WriteAllText(configuredHostAppPath + ConfigFileNameSuffix, configurationXml);
                }
                catch (Exception ex)
                {
                    throw new HostException(String.Format("Could not copy the configured host application to '{0}'.",
                        configuredHostAppDirectory), ex);
                }
            }

            public void Dispose()
            {
                try
                {
                    if (Directory.Exists(configuredHostAppDirectory))
                        Directory.Delete(configuredHostAppDirectory, true);
                }
                catch (DirectoryNotFoundException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Panic.UnhandledException(String.Format("Could not delete temporary copy of the host application from '{0}'.",
                        configuredHostAppDirectory), ex);
                }
            }

            private string GetInstalledHostProcessPath()
            {
                string hostProcessPath = Path.Combine(Loader.InstallationPath, HostAppFileName);
                if (! File.Exists(hostProcessPath))
                    throw new HostException(String.Format("Could not find the installed host application in '{0}'.", hostProcessPath));

                return hostProcessPath;
            }
        }

        private class IsolatedProcessHost : RemoteHost
        {
            private static readonly TimeSpan PingTimeout = TimeSpan.FromSeconds(5);
            private static readonly TimeSpan JoinBeforeKillTimeout = TimeSpan.FromSeconds(10); // TODO: make this configurable
            private static readonly TimeSpan JoinAfterKillTimeout = TimeSpan.FromSeconds(5); // TODO: make this configurable

            private ProcessTask processTask;
            private IServerChannel callbackChannel;

            public IsolatedProcessHost(IRemoteHostService remoteHostService, ProcessTask processTask, IServerChannel callbackChannel)
                : base(remoteHostService, PingTimeout)
            {
                this.processTask = processTask;
                this.callbackChannel = callbackChannel;
            }

            protected override void Dispose(bool disposing)
            {
 	            base.Dispose(disposing);

                if (disposing)
                {
                    if (processTask != null)
                    {
                        if (!processTask.Join(JoinBeforeKillTimeout))
                        {
                            processTask.Abort();
                            processTask.Join(JoinAfterKillTimeout);
                        }

                        processTask = null;
                    }

                    if (callbackChannel != null)
                    {
                        callbackChannel.Dispose();
                        callbackChannel = null;
                    }
                }
            }
        }
    }
}
