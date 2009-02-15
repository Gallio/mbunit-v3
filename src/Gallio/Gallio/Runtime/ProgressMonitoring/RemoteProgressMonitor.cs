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
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using Gallio.Runtime.Remoting;
using Gallio.Runtime;

namespace Gallio.Runtime.ProgressMonitoring
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
    public sealed class RemoteProgressMonitor : CancelableProgressMonitor, IDeserializationCallback
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
            RemotelyRegisterDispatcher();
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            RemotelyRegisterDispatcher();
        }

        /// <inheritdoc />
        public override ProgressMonitorTaskCookie BeginTask(string taskName, double totalWorkUnits)
        {
            forwarder.BeginTask(taskName, totalWorkUnits);
            return new ProgressMonitorTaskCookie(this);
        }

        /// <inheritdoc />
        public override void SetStatus(string status)
        {
            forwarder.SetStatus(status);
        }

        /// <inheritdoc />
        public override void Worked(double workUnits)
        {
            forwarder.Worked(workUnits);
        }

        /// <inheritdoc />
        public override void Done()
        {
            forwarder.Done();
        }

        /// <inheritdoc />
        public override IProgressMonitor CreateSubProgressMonitor(double parentWorkUnits)
        {
            return forwarder.CreateSubProgressMonitor(parentWorkUnits);
        }

        /// <inheritdoc />
        protected override void OnCancel()
        {
            forwarder.Cancel();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                forwarder.Dispose();
        }

        private void RemotelyRegisterDispatcher()
        {
            dispatcher = new Dispatcher(this);

            try
            {
                forwarder.RegisterDispatcher(dispatcher);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("Could not remotely register the progress monitor callback dispatcher.", ex);
            }
        }

        /// <summary>
        /// Forwards messages to the host's progress monitor.
        /// </summary>
        private sealed class Forwarder : LongLivedMarshalByRefObject
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

            public void Dispose()
            {
                progressMonitor.Dispose();
            }

            public IProgressMonitor CreateSubProgressMonitor(double parentWorkUnits)
            {
                return new RemoteProgressMonitor(progressMonitor.CreateSubProgressMonitor(parentWorkUnits));
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
                        UnhandledExceptionPolicy.Report("Could not remotely dispatch cancelation event.", ex);
                    }
                };
            }
        }

        /// <summary>
        /// Dispatches events to the remote progress monitor.
        /// </summary>
        private sealed class Dispatcher : LongLivedMarshalByRefObject
        {
            private readonly RemoteProgressMonitor remoteProgressMonitor;

            public Dispatcher(RemoteProgressMonitor remoteProgressMonitor)
            {
                this.remoteProgressMonitor = remoteProgressMonitor;
            }

            [OneWay]
            public void Cancel()
            {
                try
                {
                    remoteProgressMonitor.NotifyCanceled();
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("Could not locally dispatch cancelation event.", ex);
                }
            }
        }
    }
}