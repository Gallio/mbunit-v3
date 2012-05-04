// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using System.Runtime.InteropServices;
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
        private readonly IAcadPluginLocator pluginLocator;
        private readonly ILogger logger;
        private IAsyncResult activeCommand;

        /// <summary>
        /// Creates a new <see cref="AcadProcessBase"/> instance.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="commandRunner">A AutoCAD command runner.</param>
        /// <param name="pluginLocator">A AutoCAD plugin locator.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="logger"/> or <paramref name="commandRunner"/> are null.
        /// </exception>
        protected AcadProcessBase(ILogger logger, IAcadCommandRunner commandRunner, IAcadPluginLocator pluginLocator)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (commandRunner == null)
                throw new ArgumentNullException("commandRunner");
            if (pluginLocator == null)
                throw new ArgumentNullException("pluginLocator");

            this.logger = logger;
            this.commandRunner = commandRunner;
            this.pluginLocator = pluginLocator;
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
            if (disposing)
            {
                IAsyncResult result = Interlocked.Exchange(ref activeCommand, null);
                try
                {
                    // We wait on the handle directly instead of using EndRun since
                    // that may rethrow an exception on this thread that occured on the
                    // command runner's thread. Since we're no longer using the AutoCAD
                    // process anyway we ignore this exception.
                    if (result != null)
                        result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2.0));
                }
                finally
                {
                    if (result != null)
                        result.AsyncWaitHandle.Close();
                }
            }
        }

        /// <inheritdoc/>
        public void Start(string ipcPortName, Guid linkId, DebuggerSetup debuggerSetup)
        {
            if (ipcPortName == null)
                throw new ArgumentNullException("ipcPortName");

            var process = StartProcess(debuggerSetup);
            if (process == null)
                throw new InvalidOperationException("Unable to acquire AutoCAD process.");
            if (process.HasExited)
                throw new InvalidOperationException("Process has exited before assembly could be loaded.");

            NetLoadPlugin(process);

            // Run the create endpoint command asynchronously. This command runs for the duration of the test run.
            activeCommand = commandRunner.BeginRun(new CreateEndpointAndWaitCommand(ipcPortName, linkId), process, null, null);
        }

        private void NetLoadPlugin(IProcess process)
        {
            var command = new NetLoadCommand(logger, pluginLocator);
            IAsyncResult result = commandRunner.BeginRun(command, process, null, null);

            // Block while waiting for the netload command to run. This loads the create endpoint command into AutoCAD.
            commandRunner.EndRun(result);
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
