using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// Event arguments describing an event affecting a test session.
    /// </summary>
    public class TestSessionEventArgs : EventArgs
    {
        /// <summary>
        /// Creates event arguments for a test session event.
        /// </summary>
        /// <param name="session">The test session</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> is null</exception>
        public TestSessionEventArgs(ITestSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            Session = session;
        }

        /// <summary>
        /// Gets the test session.
        /// </summary>
        public ITestSession Session { get; private set; }
    }
}
