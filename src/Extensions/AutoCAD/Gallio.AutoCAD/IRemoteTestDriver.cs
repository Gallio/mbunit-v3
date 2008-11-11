using System;
using System.Runtime.Remoting;
using Gallio.Runner.Drivers;

namespace Gallio.AutoCAD
{
    /// <summary>
    /// Represents a <see cref="ITestDriver"/> executing in a remote context.
    /// </summary>
    public interface IRemoteTestDriver : ITestDriver
    {
        /// <summary>
        /// Pings the test driver to verify and maintain connectivity.
        /// </summary>
        /// <exception cref="RemotingException">Thrown if the remote host is unreachable</exception>
        void Ping();

        /// <summary>
        /// Shuts down the test driver.
        /// </summary>
        void Shutdown();
    }
}
