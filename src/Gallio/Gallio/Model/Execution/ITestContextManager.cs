using System;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test context manager functions as a factory for <see cref="ITestContext" />
    /// objects and tracks them with an <see cref="ITestContextTracker" />.
    /// </summary>
    public interface ITestContextManager
    {
        /// <summary>
        /// Gets the test context tracker.
        /// </summary>
        ITestContextTracker ContextTracker { get; }

        /// <summary>
        /// Starts a test step and returns its associated test context.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is set to a new context for the
        /// test step that is starting.  The new context will be a child of the
        /// current thread's context.
        /// </remarks>
        /// <param name="testStep">The test step</param>
        /// <returns>The test context associated with the test step</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStep"/> is null</exception>
        ITestContext StartStep(ITestStep testStep);
    }
}
