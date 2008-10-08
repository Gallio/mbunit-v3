using System;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// The Ambience server provides shared data to remote <see cref="AmbienceClient"/>s.
    /// </para>
    /// </summary>
    public class AmbienceServer : IDisposable
    {
        private readonly AmbienceServerConfiguration configuration;
        private bool isDisposed;
        private IObjectServer db4oServer;

        /// <summary>
        /// Creates an ambient server with parameters initialized to defaults.
        /// </summary>
        /// <param name="configuration">The server configuration</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configuration"/> is null</exception>
        public AmbienceServer(AmbienceServerConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            this.configuration = configuration;
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
        /// <exception cref="InvalidOperationException">Thrown if the server has already been started</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the server has been disposed</exception>
        public void Start()
        {
            ThrowIfDisposed();

            Directory.CreateDirectory(configuration.DatabaseFolder);

            IConfiguration db4oConfig = Db4oFactory.NewConfiguration();
            db4oServer = Db4oFactory.OpenServer(db4oConfig, DatabasePath, configuration.Port);
            db4oServer.GrantAccess(configuration.Credential.UserName, configuration.Credential.Password);
        }

        /// <summary>
        /// Stops the server.
        /// Does nothing if the server has already been stopped.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the server has been disposed</exception>
        public void Stop()
        {
            ThrowIfDisposed();

            if (db4oServer != null)
            {
                db4oServer.Close();
                db4oServer = null;
            }
        }

        /// <summary>
        /// Stops and disposes the server.
        /// </summary>
        /// <param name="disposing">True if disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && ! isDisposed)
            {
                Stop();
                isDisposed = true;
            }
        }

        private string DatabasePath
        {
            get { return Path.Combine(configuration.DatabaseFolder, Constants.DatabaseFileName); }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The server has been disposed.");
        }
    }
}