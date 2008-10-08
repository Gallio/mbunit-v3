using System;
using System.Net;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// Provides configuration data for <see cref="AmbienceClient" />.
    /// </para>
    /// </summary>
    public class AmbienceClientConfiguration
    {
        private string hostName = "localhost";
        private int port = Constants.DefaultPortNumber;
        private NetworkCredential credential = Constants.AnonymousCredential;

        /// <summary>
        /// Gets or sets the Ambient server hostname.
        /// </summary>
        /// <value>The hostname, defaults to "localhost".</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string HostName
        {
            get { return hostName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                hostName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Ambient server port number.
        /// </summary>
        /// <value>The port number, defaults to 7822.</value>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        /// <summary>
        /// Gets or sets the Ambient server username and password.
        /// </summary>
        /// <value>The username and password, defaults to an anonymous credential.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public NetworkCredential Credential
        {
            get { return credential; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                credential = value;
            }
        }
    }
}
