using System;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Sets the maximum amount of time that a test or fixture is permitted to run.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method,
        AllowMultiple = false, Inherited = true)]
    public class TimeoutAttribute : TestDecoratorPatternAttribute
    {
        private readonly int timeoutSeconds;

        /// <summary>
        /// Sets the test timeout in seconds.
        /// </summary>
        /// <param name="timeoutSeconds">The timeout in seconds</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeoutSeconds"/> is less than 1</exception>
        public TimeoutAttribute(int timeoutSeconds)
        {
            if (timeoutSeconds < 1)
                throw new ArgumentOutOfRangeException("timeoutSeconds", "The timeout must be at least 1 second.");

            this.timeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// Gets the timeout in seconds.
        /// </summary>
        public int TimeoutSeconds
        {
            get { return timeoutSeconds; }
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternTestBuilder builder, ICodeElementInfo codeElement)
        {
            builder.Test.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }
    }
}