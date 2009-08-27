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
using System.Threading;
using Gallio.AutoCAD.Commands;
using Gallio.Common.Concurrency;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Logging;

namespace Gallio.AutoCAD.ProcessManagement
{
    /// <summary>
    /// Default base class for <see cref="IAcadProcess"/> implementations.
    /// </summary>
    /// <remarks>
    /// <c>AcadProcessBase</c> provides the basics needed to communicate with
    /// the AutoCAD process. There are two derived types, <see cref="CreatedAcadProcess"/>
    /// and <see cref="ExistingAcadProcess"/>, that provide a means to launch
    /// new AutoCAD processes and to attach to existing ones, respectively.
    /// </remarks>
    public abstract class AcadProcessBase : IAcadProcess
    {
        private TimeSpan? readyPollInterval;
        private TimeSpan? readyTimeout;
        private readonly IAcadCommandRunner commandRunner;
        private readonly ILogger logger;

        /// <summary>
        /// Creates a new <see cref="AcadProcessBase"/> instance.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="commandRunner">A AutoCAD command runner.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="logger"/> or <paramref name="commandRunner"/> are null.
        /// </exception>
        protected AcadProcessBase(ILogger logger, IAcadCommandRunner commandRunner)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (commandRunner == null)
                throw new ArgumentNullException("commandRunner");

            this.logger = logger;
            this.commandRunner = commandRunner;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <inheritdoc/>
        public void Start(string ipcPortName, Guid linkId, string gallioLoaderAssemblyPath, DebuggerSetup debuggerSetup)
        {
            if (ipcPortName == null)
                throw new ArgumentNullException("ipcPortName");

            var process = StartProcess(debuggerSetup);
            if (process == null)
                throw new InvalidOperationException("Unable to acquire AutoCAD process.");

            var pluginLocation = AcadPluginLocator.GetAcadPluginLocation();
            if (pluginLocation == null)
                throw new InvalidOperationException("Unable to determine the location of Gallio.AutoCAD.Plugin.dll.");

            NetLoadPlugin(process, pluginLocation);
            CreateEndpointAndWait(process, ipcPortName, linkId, gallioLoaderAssemblyPath);
        }

        private void NetLoadPlugin(IProcess process, string pluginPath)
        {
            var command = new NetLoadCommand(pluginPath);

            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (process.HasExited)
                    throw new InvalidOperationException("Process has exited before assembly could be loaded.");

                commandRunner.Run(command, process);

                // Give it some time to perform the Load.
                Thread.Sleep(ReadyPollInterval);
                process.Refresh();

                if (process.IsModuleLoaded(pluginPath))
                    return;

            } while (TimeRemaining(stopwatch.Elapsed));
        }

        private bool TimeRemaining(TimeSpan elapsed)
        {
            if (elapsed < ReadyTimeout)
                return true;
            
            throw new TimeoutException("Timeout waiting for AutoCAD to load assembly.");
        }

        private void CreateEndpointAndWait(IProcess process, string ipcPortName, Guid linkId, string gallioLoaderAssemblyPath)
        {
            var command = new CreateEndpointAndWaitCommand(ipcPortName, linkId, gallioLoaderAssemblyPath);

            // Run this command on a seperate thread since the command runner won't return
            // until the command has finished executing. The "create endpoint and wait"
            // command runs for the duration of the test run in order to hold on to the
            // AutoCAD document thread (fiber).
            var thread = new Thread(() => commandRunner.Run(command, process))
                             {
                                 IsBackground = true,
                                 Name = command.GlobalName
                             };
            thread.Start();
        }

        /// <summary>
        /// Derived types are resposible for creating an returning a <see cref="IProcess"/> instance.
        /// The return value must represent a running AutoCAD process cabable of receiving messages.
        /// </summary>
        /// <param name="debuggerSetup">The debugger setup or null if the debugger shouldn't be used.</param>
        /// <returns>A <see cref="IProcess"/> instance.</returns>
        protected abstract IProcess StartProcess(DebuggerSetup debuggerSetup);

        /// <summary>
        /// The logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Gets or sets how frequently the AutoCAD process should
        /// be polled to see if it is ready after calling <see cref="Start"/>.
        /// </summary>
        public TimeSpan ReadyPollInterval
        {
            get { return readyPollInterval ?? TimeSpan.FromMilliseconds(200); }
            set { readyPollInterval = value; }
        }

        /// <summary>
        /// Gets or sets how long to wait for the AutoCAD process to
        /// become ready after calling <see cref="Start"/>.
        /// </summary>
        public TimeSpan ReadyTimeout
        {
            get { return readyTimeout ?? TimeSpan.FromSeconds(60); }
            set { readyTimeout = value; }
        }
    }
}
