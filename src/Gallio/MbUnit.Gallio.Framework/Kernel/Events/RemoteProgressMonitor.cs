// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Threading;
using MbUnit.Framework.Kernel.Events;

namespace MbUnit.Framework.Kernel.Events
{
    /// <summary>
    /// A remote progress monitor is a serializable wrapper for another progress monitor.
    /// The wrapper can be passed to another AppDomain and communication occurs over
    /// .Net remoting.
    /// </summary>
    /// <remarks>
    /// The implementation is defined so as to guard against latency and failures
    /// in the remoting channel.
    /// </remarks>
    /// <seealso cref="IProgressMonitor"/> for important thread-safety and usage remarks.
    [Serializable]
    public sealed class RemoteProgressMonitor : BaseProgressMonitor, IDeserializationCallback
    {
        private readonly Forwarder forwarder;

        [NonSerialized]
        private Dispatcher dispatcher;

        /// <summary>
        /// Creates a wrapper for the specified progress monitor.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        public RemoteProgressMonitor(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            forwarder = new Forwarder(progressMonitor);
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            dispatcher = new Dispatcher(this);
            forwarder.RegisterDispatcher(dispatcher);
        }

        /// <inheritdoc />
        protected override void OnBeginTask(string taskName, double totalWorkUnits)
        {
            forwarder.BeginTask(taskName, totalWorkUnits);
        }

        /// <inheritdoc />
        protected override void OnBeginSubTask(string subTaskName)
        {
            forwarder.BeginSubTask(subTaskName);
        }

        protected override void OnEndSubTask()
        {
            forwarder.EndSubTask();
        }

        protected override void OnDone()
        {
            forwarder.Done();
        }

        protected override void OnSetStatus(string status)
        {
            forwarder.SetStatus(status);
        }

        protected override void OnWorked(double workUnits)
        {
            forwarder.Worked(workUnits);
        }

        protected override void OnCancel()
        {
            forwarder.Cancel();
        }

        /// <summary>
        /// The forwarding event listener forwards messages to the host's progress monitor.
        /// </summary>
        private sealed class Forwarder : MarshalByRefObject
        {
            private readonly IProgressMonitor progressMonitor;

            public Forwarder(IProgressMonitor progressMonitor)
            {
                this.progressMonitor = progressMonitor;
            }

            public void BeginTask(string taskName, double totalWorkUnits)
            {
                progressMonitor.BeginTask(taskName, totalWorkUnits);
            }

            public void SetStatus(string status)
            {
                progressMonitor.SetStatus(status);
            }

            public void Worked(double workUnits)
            {
                progressMonitor.Worked(workUnits);
            }

            public void Cancel()
            {
                progressMonitor.Cancel();
            }

            public void Done()
            {
                progressMonitor.Done();
            }

            public void BeginSubTask(string subTaskName)
            {
                progressMonitor.BeginSubTask(subTaskName);
            }

            public void EndSubTask()
            {
                progressMonitor.EndSubTask();
            }

            public void RegisterDispatcher(Dispatcher dispatcher)
            {
                progressMonitor.Canceled += delegate
                {
                    try
                    {
                        dispatcher.Cancel();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to dispatch cancelation event to remote progress monitor: " + ex);
                    }
                };
            }
        }

        /// <summary>
        /// Dispatches events to the remote progress monitor.
        /// </summary>
        private sealed class Dispatcher : MarshalByRefObject
        {
            private readonly RemoteProgressMonitor remoteProgressMonitor;

            public Dispatcher(RemoteProgressMonitor remoteProgressMonitor)
            {
                this.remoteProgressMonitor = remoteProgressMonitor;
            }

            [OneWay]
            public void Cancel()
            {
                remoteProgressMonitor.NotifyCanceled();
            }
        }
    }
}
