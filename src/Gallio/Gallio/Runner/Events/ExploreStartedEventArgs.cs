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
using Gallio.Concurrency;
using Gallio.Model;
using Gallio.Runner.Reports;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test exploration has started.
    /// </summary>
    public sealed class ExploreStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestPackageConfig testPackageConfig;
        private readonly TestExplorationOptions testExplorationOptions;
        private readonly LockBox<Report> reportLockBox;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <param name="testExplorationOptions">The test exploration options</param>
        /// <param name="reportLockBox">The report lock-box which may be used to access the report asynchronously during execution</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/>
        /// or <paramref name="testExplorationOptions"/> is null</exception>
        public ExploreStartedEventArgs(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions,
            LockBox<Report> reportLockBox)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");

            this.testPackageConfig = testPackageConfig;
            this.testExplorationOptions = testExplorationOptions;
            this.reportLockBox = reportLockBox;
        }

        /// <summary>
        /// Gets the test package configuration.
        /// </summary>
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
        }

        /// <summary>
        /// Gets the test exploration options.
        /// </summary>
        public TestExplorationOptions TestExplorationOptions
        {
            get { return testExplorationOptions; }
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
