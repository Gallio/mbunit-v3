// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;

namespace Gallio.Ambience
{
    /// <summary>
    /// The Ambience server provides shared data to remote <see cref="AmbienceClient"/>s.
    /// </summary>
    public class AmbienceServer : IDisposable
    {
        private readonly AmbienceServerConfiguration configuration;
        private readonly string databasePath;
        private bool isDisposed;
        private IObjectServer db4oServer;

        /// <summary>
        /// Creates an ambient server with parameters initialized to defaults.
        /// </summary>
        /// <param name="configuration">The server configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null.</exception>
        public AmbienceServer(AmbienceServerConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.configuration = configuration;
            databasePath = Path.GetFullPath(configuration.DatabasePath);
        }

        /// <summary>
        /// Stops and disposes the server.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the server has already been started.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the server has been disposed.</exception>
        /// <exception cref="AmbienceException">Thrown if the operation failed.</exception>
        public void Start()
        {
            ThrowIfDisposed();

            Directory.CreateDirectory(Path.GetDirectoryName(databasePath));

            try
            {
                IConfiguration db4oConfig = Db4oFactory.NewConfiguration();
                db4oServer = Db4oFactory.OpenServer(db4oConfig, databasePath, configuration.Port);
                db4oServer.GrantAccess(configuration.Credential.UserName, configuration.Credential.Password);
            }
            catch (Db4oException ex)
            {
                throw new AmbienceException("An error occurred while starting the server.", ex);
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does nothing if the server has already been stopped.
        /// </para>
        /// </remarks>
        /// <exception cref="ObjectDisposedException">Thrown if the server has been disposed.</exception>
        public void Stop()
        {
            ThrowIfDisposed();

            if (db4oServer != null)
            {
                try
                {
                    db4oServer.Close();
                }
                catch (Db4oException ex)
                {
                    throw new AmbienceException("An error occurred while stopping the server.", ex);
                }
                finally
                {
                    db4oServer = null;
                }
            }
        }

        /// <summary>
        /// Stops and disposes the server.
        /// </summary>
        /// <param name="disposing">True if disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !isDisposed)
                    Stop();
            }
            finally
            {
                isDisposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The server has been disposed.");
        }
    }
}
