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

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// A host factory encapsulates a policy for creating new <see cref="IHost"/> instances.
    /// </summary>
    public interface IHostFactory
    {
        /// <summary>
        /// Creates a host instance.
        /// </summary>
        /// <param name="hostSetup">The host setup</param>
        /// <param name="logger">The logger for host message output</param>
        /// <returns>The newly created host</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/> 
        /// or <paramref name="logger"/> is null</exception>
        IHost CreateHost(HostSetup hostSetup, ILogger logger);
    }
}
