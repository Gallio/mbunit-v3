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
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Utilities;

namespace Gallio.PowerShellCommands
{
    /// <exclude />
    /// <summary>
    /// Abstract base class for PowerShell commands.
    /// Provides some useful runtime support.
    /// </summary>
    public abstract class BaseCommand : PSCmdlet
    {
        private CommandLogger logger;
        private CommandProgressMonitorProvider progressMonitorProvider;
        private event EventHandler stopRequested;
        private Queue<Action> pendingBlocks;

        /// <summary>
        /// The event dispatches when the command is asynchronously being stopped
        /// via <see cref="StopProcessing" />.
        /// </summary>
        public event EventHandler StopRequested
        {
            add
            {
                lock (this)
                    stopRequested += value;
            }
            remove
            {
                lock (this)
                    stopRequested -= value;
            }
        }

        /// <summary>
        /// Gets the logger for the cmdlet.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                lock (this)
                {
                    if (logger == null)
                        logger = new CommandLogger(this);
                    return logger;
                }
            }
        }

        /// <summary>
        /// Sets whether progress information is shown during the execution. If this option is specified,
        /// the execution is silent and no progress information is displayed.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>This parameter takes the value true if present and false if not. No
        /// value has to be specified.</item>
        /// </list>
        /// </remarks>
        /// <example>
        /// <code>
        /// # Shows progress information
        /// Run-Gallio SomeAssembly.dll
        /// # Hides progress information
        /// Run-Gallio SomeAssembly.dll -np
        /// </code>
        /// </example>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        [Alias("np", "no-progress")]
        public SwitchParameter NoProgress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the progress monitor provider for the cmdlet.
        /// </summary>
        public IProgressMonitorProvider ProgressMonitorProvider
        {
            get
            {
                lock (this)
                {
                    if (progressMonitorProvider == null)
                        progressMonitorProvider = new CommandProgressMonitorProvider(this);
                    return progressMonitorProvider;
                }
            }
        }

        /// <summary>
        /// Posts an action to perform later within the message loop.
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <seealso cref="RunWithMessagePump"/>
        public void PostMessage(Action action)
        {
            lock (this)
            {
                if (pendingBlocks == null)
                    throw new InvalidOperationException("There is no message pump currently running.");

                pendingBlocks.Enqueue(action);
                Monitor.PulseAll(this);
            }
        }

        /// <summary>
        /// Starts a message pump running on the current thread and performs the
        /// specified action in another thread.  The action can asynchronously communicate back to the
        /// cmdlet using <see cref="PostMessage" /> on the current thread.
        /// </summary>
        /// <param name="action">The action to perform</param>
        public void RunWithMessagePump(Action action)
        {
            bool loopInitialized = false;
            try
            {
                lock (this)
                {
                    if (pendingBlocks != null)
                        throw new InvalidOperationException("Already have a message pump.");

                    pendingBlocks = new Queue<Action>();
                    loopInitialized = true;
                }

                IAsyncResult result = action.BeginInvoke(QuitMessagePump, action);
                RunMessagePumpUntilQuit();
                action.EndInvoke(result);
            }
            finally
            {
                if (loopInitialized)
                {
                    lock (this)
                        pendingBlocks = null;
                }
            }
        }

        private void RunMessagePumpUntilQuit()
        {
            while (pendingBlocks != null)
            {
                Action messageAction;
                lock (this)
                {
                    if (pendingBlocks.Count == 0)
                    {
                        Monitor.Wait(this);
                        continue;
                    }

                    messageAction = pendingBlocks.Dequeue();
                }

                if (messageAction == null)
                    break;

                try
                {
                    messageAction();
                }
                catch (Exception ex)
                {
                    try
                    {
                        WriteError(new ErrorRecord(ex, "An exception occurred in the message pump.", ErrorCategory.NotSpecified, "Gallio"));
                    }
                    catch (Exception)
                    {
                        // Ignore the error since there's nothing we can do about it.
                        // We probably can't even log the error because the logger will just end
                        // up back in here.
                    }
                }
            }
        }

        private void QuitMessagePump(IAsyncResult ar)
        {
            PostMessage(null);
        }

        /// <summary>
        /// Aborts the processing of the command.
        /// </summary>
        protected override void StopProcessing()
        {
            EventHandler handler;
            lock (this)
                handler = stopRequested;

            EventHandlerUtils.SafeInvoke(handler, this, EventArgs.Empty);
        }
    }
}
