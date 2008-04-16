using System;
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// <para>
    /// A test runner extension plugs augments the behavior oa <see cref="ITestRunner"/>
    /// by attaching new behaviors to its event handlers.
    /// </para>
    /// <para>
    /// Typical extension use-cases:
    /// <list type="bullet">
    /// <item>Custom logging by listening for events of interest and writing messages
    /// to a file or to another location.</item>
    /// <item>Custom configuration of test package loading, test exploration
    /// and test execution options by listening for the starting events of the phase
    /// and modifying the options in place.</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// More complex extensions such as integration of new test frameworks are typically
    /// performed by plugins because new runtime services must be registered to support them.
    /// </para>
    /// </remarks>
    public interface ITestRunnerExtension
    {
        /// <summary>
        /// Gets or sets configuration parameters for the extension.
        /// </summary>
        string Parameters { get; set; }

        /// <summary>
        /// Installs the extension into a test runner.
        /// </summary>
        /// <param name="events">The test runner events, not null</param>
        /// <param name="logger">The logger, not null</param>
        void Install(ITestRunnerEvents events, ILogger logger);
    }
}