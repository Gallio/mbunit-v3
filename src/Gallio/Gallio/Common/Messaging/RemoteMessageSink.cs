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
using System.Text;
using Gallio.Common.Remoting;

namespace Gallio.Common.Messaging
{
    /// <summary>
    /// Wraps a <see cref="IMessageSink"/> so that messages can be sent remotely.
    /// </summary>
    [Serializable]
    public class RemoteMessageSink : IMessageSink
    {
        private readonly Forwarder forwarder;

        /// <summary>
        /// Creates a remote message sink.
        /// </summary>
        /// <param name="messageSink">The message sink to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="messageSink"/> is null.</exception>
        public RemoteMessageSink(IMessageSink messageSink)
        {
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");

            forwarder = new Forwarder(messageSink);
        }

        /// <inheritdoc />
        public void Publish(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            forwarder.Publish(message);
        }

        private sealed class Forwarder : LongLivedMarshalByRefObject
        {
            private readonly IMessageSink messageSink;

            public Forwarder(IMessageSink messageSink)
            {
                this.messageSink = messageSink;
            }

            public void Publish(Message message)
            {
                messageSink.Publish(message);
            }
        }
    }
}
