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
using Castle.Core.Logging;

namespace Gallio.Hosting
{
    /// <summary>
    /// An abstract base class for host factories.
    /// </summary>
    public abstract class BaseHostFactory : IHostFactory
    {
        /// <inheritdoc />
        public IHost CreateHost(HostSetup hostSetup, ILogger logger)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");
            if (logger == null)
                throw new ArgumentNullException("logger");

            HostSetup canonicalHostSetup = hostSetup.Copy();
            canonicalHostSetup.Canonicalize(null);

            return CreateHostImpl(canonicalHostSetup, logger);
        }

        /// <summary>
        /// Creates the host.
        /// </summary>
        /// <param name="hostSetup">The canonicalized host setup, non-null</param>
        /// <param name="logger">The logger, non-null</param>
        /// <returns>The host</returns>
        protected abstract IHost CreateHostImpl(HostSetup hostSetup, ILogger logger);
    }
}
