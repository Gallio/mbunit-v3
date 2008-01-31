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
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Castle.Core.Logging;
using Gallio.Hosting.ProgressMonitoring;
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
        private Queue<Block> pendingBlocks;

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
        /// Posts a block to run in the message loop.
        /// </summary>
        /// <param name="block">The block to run</param>
        /// <seealso cref="RunWithMessagePump"/>
        public void PostMessage(Block block)
        {
            lock (this)
            {
                if (pendingBlocks == null)
                    throw new InvalidOperationException("There is no message pump currently running.");

                pendingBlocks.Enqueue(block);
                Monitor.PulseAll(this);
            }
        }

        /// <summary>
        /// Starts a message pump running on the current thread and runs the specified
        /// block in another thread.  The block can asynchronously communicate back to the
        /// cmdlet using <see cref="PostMessage" /> on the current thread.
        /// </summary>
        /// <param name="block">The block to run</param>
        public void RunWithMessagePump(Block block)
        {
            bool loopInitialized = false;
            try
            {
                lock (this)
                {
                    if (pendingBlocks != null)
                        throw new InvalidOperationException("Already have a message pump.");

                    pendingBlocks = new Queue<Block>();
                    loopInitialized = true;
                }

                IAsyncResult result = block.BeginInvoke(QuitMessagePump, block);
                RunMessagePumpUntilQuit();
                block.EndInvoke(result);
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
                Block messageBlock;
                lock (this)
                {
                    if (pendingBlocks.Count == 0)
                    {
                        Monitor.Wait(this);
                        continue;
                    }

                    messageBlock = pendingBlocks.Dequeue();
                }

                if (messageBlock == null)
                    break;

                try
                {
                    messageBlock();
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
