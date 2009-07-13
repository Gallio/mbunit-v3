// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using Gallio.Common.Concurrency;
using Gallio.Model;
using Gallio.Model.Schema;
using Gallio.Runner.Reports.Schema;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test execution has started.
    /// </summary>
    public sealed class RunStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestPackage testPackage;
        private readonly TestExplorationOptions testExplorationOptions;
        private readonly TestExecutionOptions testExecutionOptions;
        private readonly LockBox<Report> reportLockBox;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testPackage">The test package.</param>
        /// <param name="testExplorationOptions">The test exploration options.</param>
        /// <param name="testExecutionOptions">The test execution options.</param>
        /// <param name="reportLockBox">The report lock-box which may be used to access the report asynchronously during execution.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackage"/>,
        /// <paramref name="testExplorationOptions"/> or <paramref name="testExecutionOptions"/> is null.</exception>
        public RunStartedEventArgs(TestPackage testPackage,
            TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions,
            LockBox<Report> reportLockBox)
        {
            if (testPackage == null)
                throw new ArgumentNullException("testPackage");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (testExecutionOptions == null)
                throw new ArgumentNullException("testExecutionOptions");

            this.testPackage = testPackage;
            this.testExplorationOptions = testExplorationOptions;
            this.testExecutionOptions = testExecutionOptions;
            this.reportLockBox = reportLockBox;
        }

        /// <summary>
        /// Gets the test package.
        /// </summary>
        public TestPackage TestPackage
        {
            get { return testPackage; }
        }

        /// <summary>
        /// Gets the test exploration options.
        /// </summary>
        public TestExplorationOptions TestExplorationOptions
        {
            get { return testExplorationOptions; }
        }

        /// <summary>
        /// Gets the test execution options.
        /// </summary>
        public TestExecutionOptions TestExecutionOptions
        {
            get { return testExecutionOptions; }
        }

        /// <summary>
        /// Gets the report lock-box which may be used to access the report asynchronously during execution.
        /// </summary>
        public LockBox<Report> ReportLockBox
        {
            get { return reportLockBox; }
        }
    }
}
