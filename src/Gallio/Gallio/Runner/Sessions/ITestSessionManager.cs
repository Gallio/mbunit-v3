using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// <para>
    /// Manages test sessions.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Instances of this class are safe for use by multiple concurrent threads.
    /// </para>
    /// </remarks>
    /// <seealso cref="ITestSession"/>
    public interface ITestSessionManager
    {
        /// <summary>
        /// An event that is fired whenever a test session is opened.
        /// </summary>
        event EventHandler<TestSessionEventArgs> SessionOpened;

        /// <summary>
        /// An event that is fired whenever a test session is closed.
        /// </summary>
        event EventHandler<TestSessionEventArgs> SessionClosed;

        /// <summary>
        /// Opens a new test session.
        /// </summary>
        /// <returns>The new test session</returns>
        ITestSession OpenSession();

        /// <summary>
        /// Closes a test session.
        /// </summary>
        /// <param name="session">The test session to close</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="session"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="session"/> is not
        /// owned by this session manager</exception>
        void CloseSession(ITestSession session);

        /// <summary>
        /// Gets all opened test sessions.
        /// </summary>
        /// <returns>The list of open test sessions</returns>
        IList<ITestSession> GetSessions();
    }
}
