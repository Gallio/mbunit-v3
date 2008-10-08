// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Net;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// The Ambience client accesses shared data provided by a remote <see cref="AmbienceServer" />.
    /// </para>
    /// </summary>
    public sealed class AmbienceClient : IDisposable
    {
        private Db4oAmbientDataContainer container;

        private AmbienceClient(Db4oAmbientDataContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Gets the client's data container.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the client has been disposed</exception>
        public IAmbientDataContainer Container
        {
            get
            {
                ThrowIfDisposed();
                return container;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (container != null)
            {
                container.Inner.Dispose();
                container = null;
            }
        }

        /// <summary>
        /// Connects the client to the remote server.
        /// </summary>
        /// <param name="configuration">The client configuration</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null</exception>
        public static AmbienceClient Connect(AmbienceClientConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            IConfiguration db4oConfig = Db4oFactory.NewConfiguration();
            IObjectContainer db4oContainer = Db4oFactory.OpenClient(db4oConfig,
                configuration.HostName, configuration.Port,
                configuration.Credential.UserName, configuration.Credential.Password);

            return new AmbienceClient(new Db4oAmbientDataContainer(db4oContainer));
        }

        private void ThrowIfDisposed()
        {
            if (container == null)
                throw new ObjectDisposedException("The client has been disposed.");
        }
    }
}
