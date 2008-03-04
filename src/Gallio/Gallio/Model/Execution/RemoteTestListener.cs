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
using Gallio.Hosting;
using Gallio.Utilities;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A remote test listener is a serializable wrapper for another listener.
    /// The wrapper can be passed to another AppDomain and communication occurs over
    /// .Net remoting.
    /// </summary>
    [Serializable]
    public sealed class RemoteTestListener : ITestListener
    {
        private readonly Forwarder forwarder;

        /// <summary>
        /// Creates a wrapper for the specified listener.
        /// </summary>
        /// <param name="listener">The listener</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="listener"/> is null</exception>
        public RemoteTestListener(ITestListener listener)
        {
            if (listener == null)
                throw new ArgumentNullException(@"listener");

            forwarder = new Forwarder(listener);
        }

        /// <inheritdoc />
        public void NotifyLifecycleEvent(LifecycleEventArgs e)
        {
            try
            {
                forwarder.NotifyLifecycleEvent(e);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("Could not remotely dispatch lifecycle event.", ex);
            }
        }

        /// <inheritdoc />
        public void NotifyLogEvent(LogEventArgs e)
        {
            try
            {
                forwarder.NotifyLogEvent(e);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("Could not remotely dispatch log event.", ex);
            }
        }

        /// <summary>
        /// The forwarding event listener forwards events to the host's event listener.
        /// </summary>
        private sealed class Forwarder : LongLivedMarshalByRefObject
        {
            private readonly ITestListener listener;

            public Forwarder(ITestListener listener)
            {
                this.listener = listener;
            }

            public void NotifyLifecycleEvent(LifecycleEventArgs e)
            {
                try
                {
                    listener.NotifyLifecycleEvent(e);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("Could not locally dispatch lifecycle event.", ex);
                }
            }

            public void NotifyLogEvent(LogEventArgs e)
            {
                try
                {
                    listener.NotifyLogEvent(e);
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("Could not locally dispatch log event.", ex);
                }
            }
        }
    }
}