using System;
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Abstract implementation of a test runner extension.
    /// <seealso cref="ITestRunnerExtension"/> for more details.
    /// </summary>
    public abstract class TestRunnerExtension : ITestRunnerExtension
    {
        private ITestRunnerEvents events;
        private ILogger logger;
        private string parameters = string.Empty;

        /// <summary>
        /// Gets the test runner event extension point.
        /// </summary>
        public ITestRunnerEvents Events
        {
            get { return events; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <inheritdoc />
        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <inheritdoc />
        public void Install(ITestRunnerEvents events, ILogger logger)
        {
            if (events == null)
                throw new ArgumentNullException("events");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.events = events;
            this.logger = logger;

            Initialize();
        }

        /// <summary>
        /// Initializes the extension as part of extension installation.
        /// </summary>
        protected abstract void Initialize();
    }
}
