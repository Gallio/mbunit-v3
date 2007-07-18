// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
        private bool isTemplateTreeReady;
        private bool isTestTreeReady;

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
        public void LoadProject(TestProject project)
        {
            logger.Debug("Loading project into test domain.");

            domain.LoadProject(project);
        }

        /// <summary>
        /// Gets the root of the template tree.
        /// Automatically builds the template tree if needed.
        /// </summary>
        public TemplateInfo GetTemplateTreeRoot()
        {
            BuildTemplatesIfNeeded();
            return domain.TemplateTreeRoot;
        }

        /// <summary>
        /// Gets the root of the test tree.
        /// Automatically builds the test tree if needed.
        /// </summary>
        public TestInfo GetTestTreeRoot()
        {
            BuildTestsIfNeeded();
            return domain.TestTreeRoot;
        }

        /// <summary>
        /// Runs the tests.
        /// </summary>
        public void Run()
        {
            BuildTestsIfNeeded();
            domain.RunTests();
        }

        /// <summary>
        /// Writes a test report.
        /// </summary>
        public void WriteReport()
        {
        }

        private void BuildTemplatesIfNeeded()
        {
            if (! isTemplateTreeReady)
            {
                domain.BuildTemplates();
                isTemplateTreeReady = true;
            }
        }

        private void BuildTestsIfNeeded()
        {
            BuildTemplatesIfNeeded();

            if (!isTestTreeReady)
            {
                domain.BuildTests();
                isTestTreeReady = true;
            }
        }
    }
}
