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
using Gallio.Runtime.Logging;
using Gallio.Runtime.Remoting;

namespace Gallio.Runtime.Logging
{
    /// <summary>
    /// Wraps a logger so that it can be accessed remotely.
    /// </summary>
    [Serializable]
    public class RemoteLogger : BaseLogger
    {
        private readonly Forwarder forwarder;

        /// <summary>
        /// Creates a wrapper for the specified logger.
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="logger"/> is null</exception>
        public RemoteLogger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            forwarder = new Forwarder(logger);
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, Exception exception)
        {
            forwarder.Log(severity, message, exception);
        }

        /// <summary>
        /// Forwards messages to the host's logger.
        /// </summary>
        private sealed class Forwarder : LongLivedMarshalByRefObject
        {
            private readonly ILogger logger;

            public Forwarder(ILogger logger)
            {
                this.logger = logger;
            }

            public void Log(LogSeverity severity, string message, Exception exception)
            {
                logger.Log(severity, message, exception);
            }
        }
    }
}