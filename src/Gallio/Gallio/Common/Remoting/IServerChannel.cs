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

namespace Gallio.Common.Remoting
{
    /// <summary>
    /// A server channel manages the server side of a remoting channel.
    /// </summary>
    /// <seealso cref="IClientChannel"/>
    public interface IServerChannel : IDisposable
    {
        /// <summary>
        /// Registers a well-known service with the specified name.
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <param name="component">The component that provides the service</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceName"/> or
        /// <paramref name="component"/> is null</exception>
        void RegisterService(string serviceName, MarshalByRefObject component);
    }
}
