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

namespace Gallio.Hosting
{
    /// <summary>
    /// An abstract base class for host factories.
    /// </summary>
    public abstract class BaseHostFactory : IHostFactory
    {
        /// <inheritdoc />
        public IHost CreateHost(HostSetup hostSetup)
        {
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            HostSetup canonicalHostSetup = hostSetup.Copy();
            canonicalHostSetup.Canonicalize(null);

            return CreateHostImpl(canonicalHostSetup);
        }

        /// <summary>
        /// Creates the host.
        /// </summary>
        /// <param name="hostSetup">The host setup, non-null</param>
        /// <returns>The host</returns>
        protected abstract IHost CreateHostImpl(HostSetup hostSetup);
    }
}
