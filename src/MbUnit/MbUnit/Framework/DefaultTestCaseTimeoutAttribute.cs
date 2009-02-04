using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Specifies the default test case timeout for all tests in the test assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Defaults to 10 minutes.
    /// </para>
    /// </remarks>
    /// <seealso cref="TestAssemblyExecutionParameters.DegreeOfParallelism"/>
    [AttributeUsage(PatternAttributeTargets.TestAssembly, AllowMultiple = true, Inherited = true)]
    public class DefaultTestCaseTimeoutAttribute : TestAssemblyDecoratorPatternAttribute
    {
        private readonly int timeoutSeconds;

        /// <summary>
        /// Sets the default timeout in seconds, or zero if none.
        /// </summary>
        /// <param name="timeoutSeconds">The timeout in seconds, or zero if none</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeoutSeconds"/> is less than 0</exception>
        public DefaultTestCaseTimeoutAttribute(int timeoutSeconds)
        {
            if (timeoutSeconds < 0)
                throw new ArgumentOutOfRangeException("timeoutSeconds", "The timeout must be zero or at least 1 second.");

            this.timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Gets the default timeout in seconds, or zero if none.
        /// </summary>
        public int TimeoutSeconds
        {
            get { return timeoutSeconds; }
        }

        /// <summary>
        /// Gets the timeout, or null if none.
        /// </summary>
        public TimeSpan? Timeout
        {
            get { return timeoutSeconds == 0 ? (TimeSpan?) null : TimeSpan.FromSeconds(timeoutSeconds); }
        }

        /// <inheritdoc />
        protected override void DecorateAssemblyTest(IPatternScope assemblyScope, IAssemblyInfo assembly)
        {
            assemblyScope.TestBuilder.TestActions.InitializeTestChain.After(state =>
            {
                TestAssemblyExecutionParameters.DefaultTestCaseTimeout = Timeout;
            });
        }
    }
}
