using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Castle.Core.Logging;
using MbUnit.Core.Serialization;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// Runs tests and reports progress by writing log messages.
    /// The implementation is not comprehensive.  It is intended to provide a simple
    /// interface for running tests inside an isolated domain to assist with MbUnit
    /// integration in the common cases.
    /// </summary>
    public class AutoRunner
    {
        private ILogger logger;
        private ITestDomain domain;

        /// <summary>
        /// Creates a runner, initially without a project.
        /// </summary>
        public AutoRunner()
        {
            logger = NullLogger.Instance;
            domain = new IsolatedTestDomain();
        }

        /// <summary>
        /// Gets or sets the logger for writing messages.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }

        /// <summary>
        /// Gets the test domain used by the runner.
        /// </summary>
        public ITestDomain Domain
        {
            get { return domain; }
        }

        /// <summary>
        /// Loads a test project.
        /// </summary>
        /// <param name="project">The test project</param>
        public void LoadProject(TestProjectInfo project)
        {
            logger.Debug("Loading project into test domain.");

            domain.LoadProject(project);
        }

        /// <summary>
        /// Runs the tests.
        /// </summary>
        public void Run()
        {
            domain.RunTests();
        }

        /// <summary>
        /// Writes a test report.
        /// </summary>
        public void WriteReport()
        {
        }
    }
}
