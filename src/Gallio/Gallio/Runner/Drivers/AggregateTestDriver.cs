// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Serialization;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// An aggregate test driver partitions the work of running tests among multiple test drivers.
    /// </summary>
    public abstract class AggregateTestDriver : BaseTestDriver
    {
        private readonly List<Partition> partitions = new List<Partition>();

        /// <inheritdoc />
        protected override void LoadImpl(TestPackageConfig testPackageConfig, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Loading tests.", 2);

                DisposeDrivers();

                partitions.AddRange(CreatePartitions(testPackageConfig));
                progressMonitor.Worked(1);

                if (partitions.Count != 0)
                {
                    double workPerPartition = 1.0 / partitions.Count;
                    foreach (Partition partition in partitions)
                    {
                        partition.TestDriver.Load(partition.TestPackageConfig, progressMonitor.CreateSubProgressMonitor(workPerPartition));
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override TestModelData ExploreImpl(TestExplorationOptions options, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Exploring tests.", 1);

                TestModelData model = new TestModelData(new TestData(new RootTest()));
                if (partitions.Count != 0)
                {
                    double workPerPartition = 1.0 / partitions.Count;
                    foreach (Partition partition in partitions)
                    {
                        model.MergeFrom(partition.TestDriver.Explore(options, progressMonitor.CreateSubProgressMonitor(workPerPartition)));
                    }
                }

                return model;
            }
        }

        /// <inheritdoc />
        protected override void RunImpl(TestExecutionOptions options, ITestListener listener, IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", 1);

                if (partitions.Count != 0)
                {
                    double workPerPartition = 1.0 / partitions.Count;
                    foreach (Partition partition in partitions)
                    {
                        partition.TestDriver.Run(options, listener, progressMonitor.CreateSubProgressMonitor(workPerPartition));
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void UnloadImpl(IProgressMonitor progressMonitor)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Unloading tests.", 1);

                if (partitions.Count != 0)
                {
                    try
                    {
                        double workPerPartition = 1.0 / partitions.Count;
                        foreach (Partition partition in partitions)
                        {
                            partition.TestDriver.Unload(progressMonitor.CreateSubProgressMonitor(workPerPartition));
                        }
                    }
                    finally
                    {
                        DisposeDrivers();
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DisposeDrivers();
        }

        /// <summary>
        /// Creates the partitions of the aggregate test driver.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration, not null</param>
        /// <returns>The partitions</returns>
        protected abstract IEnumerable<Partition> CreatePartitions(TestPackageConfig testPackageConfig);

        private void DisposeDrivers()
        {
            foreach (Partition partition in partitions)
            {
                try
                {
                    partition.TestDriver.Dispose();
                }
                catch (Exception ex)
                {
                    UnhandledExceptionPolicy.Report("An exception occurred while disposing a test driver.", ex);
                }
            }

            partitions.Clear();
        }

        /// <summary>
        /// Provides information about a partition of the aggregate test driver.
        /// Each partition specifies a test driver and a test package configuration that
        /// covers a portion of the total test package.
        /// </summary>
        protected struct Partition
        {
            private readonly ITestDriver testDriver;
            private readonly TestPackageConfig testPackageConfig;

            /// <summary>
            /// Creates a partition information structure.
            /// </summary>
            /// <param name="testDriver">The test driver</param>
            /// <param name="testPackageConfig">The test package configuration for the driver</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDriver"/>
            /// or <paramref name="testPackageConfig"/> is null</exception>
            public Partition(ITestDriver testDriver, TestPackageConfig testPackageConfig)
            {
                if (testDriver == null)
                    throw new ArgumentNullException("testDriver");
                if (testPackageConfig == null)
                    throw new ArgumentNullException("testPackageConfig");

                this.testDriver = testDriver;
                this.testPackageConfig = testPackageConfig;
            }

            /// <summary>
            /// Gets the test driver.
            /// </summary>
            public ITestDriver TestDriver
            {
                get { return testDriver; }
            }

            /// <summary>
            /// Gets the test package configuration for the driver.
            /// </summary>
            public TestPackageConfig TestPackageConfig
            {
                get { return testPackageConfig; }
            }
        }
    }
}
