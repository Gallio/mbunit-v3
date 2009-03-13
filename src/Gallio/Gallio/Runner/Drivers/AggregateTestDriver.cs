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
using System.Collections.Generic;
using System.Diagnostics;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Model.Logging;
using Gallio.Model.Messages;
using Gallio.Model.Serialization;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runner.Drivers
{
    /// <summary>
    /// An aggregate test driver partitions the work of running tests among multiple test drivers.
    /// </summary>
    public abstract class AggregateTestDriver : BaseTestDriver
    {
        private delegate void PartitionAction(ITestDriver testDriver, TestPackageConfig testPackageConfig, Listener listener, IProgressMonitor progressMonitor);

        /// <summary>
        /// Initializes an aggregate test driver.
        /// </summary>
        protected AggregateTestDriver()
        {
        }

        /// <inheritdoc />
        protected override void ExploreImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Exploring the tests.", 1))
            {
                ExploreOrRunEachPartition(testPackageConfig, testExplorationListener, null, (testDriver, partitionConfig, listener, subProgressMonitor) =>
                {
                    testDriver.Explore(partitionConfig, testExplorationOptions, listener, subProgressMonitor);
                }, progressMonitor);
            }
        }

        /// <inheritdoc />
        protected override void RunImpl(TestPackageConfig testPackageConfig, TestExplorationOptions testExplorationOptions, ITestExplorationListener testExplorationListener, TestExecutionOptions testExecutionOptions, ITestExecutionListener testExecutionListener, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Running the tests.", 1))
            {
                ExploreOrRunEachPartition(testPackageConfig, testExplorationListener, testExecutionListener, (testDriver, partitionConfig, listener, subProgressMonitor) =>
                {
                    testDriver.Run(partitionConfig, testExplorationOptions, listener, testExecutionOptions, listener, subProgressMonitor);
                }, progressMonitor);
            }
        }

        private void ExploreOrRunEachPartition(TestPackageConfig testPackageConfig,
            ITestExplorationListener testExplorationListener,
            ITestExecutionListener testExecutionListener,
            PartitionAction action, IProgressMonitor progressMonitor)
        {
            bool shutdownExceptionEncountered = false;

            DoWithPartitions(testPackageConfig, partitions =>
            {
                using (Listener listener = new Listener(testExplorationListener, testExecutionListener))
                {
                    if (partitions.Count != 0)
                    {
                        double workPerPartition = 1.0 / partitions.Count;
                        foreach (Partition partition in partitions)
                        {
                            using (IProgressMonitor subProgressMonitor =
                                progressMonitor.CreateSubProgressMonitor(workPerPartition))
                            {
                                progressMonitor.SetStatus("Initializing test driver.");

                                ITestDriver testDriver = partition.TestDriverFactory.CreateTestDriver();
                                testDriver.Initialize(RuntimeSetup, TestRunnerOptions, Logger);
                                try
                                {
                                    progressMonitor.SetStatus("");

                                    action(testDriver, partition.TestPackageConfig, listener, subProgressMonitor);
                                }
                                finally
                                {
                                    progressMonitor.SetStatus("Disposing test driver.");

                                    try
                                    {
                                        testDriver.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (!shutdownExceptionEncountered)
                                        {
                                            shutdownExceptionEncountered = true;
                                            Logger.Log(LogSeverity.Warning, "A fatal exception occurred while disposing a test driver.", ex);
                                        }
                                    }

                                    progressMonitor.SetStatus("");
                                }
                            }
                        }
                    }
                }
            }, progressMonitor.SetStatus);
        }

        /// <summary>
        /// Runs a block of code with partitions of the test package.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The subclass should perform any setup / teardown around the callback.
        /// </para>
        /// </remarks>
        /// <param name="testPackageConfig">The test package configuration, not null</param>
        /// <param name="action">The action to perform given a list of partitions, not null</param>
        /// <param name="setStatus">An action that can be used to report progress, not null</param>
        protected abstract void DoWithPartitions(TestPackageConfig testPackageConfig, Action<IList<Partition>> action,
            Action<string> setStatus);

        /// <summary>
        /// Provides information about a partition of the aggregate test driver.
        /// Each partition specifies a test driver and a test package configuration that
        /// covers a portion of the total test package.
        /// </summary>
        protected struct Partition
        {
            private readonly ITestDriverFactory testDriverFactory;
            private readonly TestPackageConfig testPackageConfig;

            /// <summary>
            /// Creates a partition information structure.
            /// </summary>
            /// <param name="testDriverFactory">The test driver factory</param>
            /// <param name="testPackageConfig">The test package configuration for the driver</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDriverFactory"/>
            /// or <paramref name="testPackageConfig"/> is null</exception>
            public Partition(ITestDriverFactory testDriverFactory, TestPackageConfig testPackageConfig)
            {
                if (testDriverFactory == null)
                    throw new ArgumentNullException("testDriverFactory");
                if (testPackageConfig == null)
                    throw new ArgumentNullException("testPackageConfig");

                this.testDriverFactory = testDriverFactory;
                this.testPackageConfig = testPackageConfig;
            }

            /// <summary>
            /// Gets the test driver factory.
            /// </summary>
            public ITestDriverFactory TestDriverFactory
            {
                get { return testDriverFactory; }
            }

            /// <summary>
            /// Gets the test package configuration for the driver.
            /// </summary>
            public TestPackageConfig TestPackageConfig
            {
                get { return testPackageConfig; }
            }
        }

        /// <summary>
        /// A test listener that consolidates the root steps across all test
        /// domains into a single one.
        /// </summary>
        /// <todo author="jeff">
        /// This feels like a great big gigantic hack to make up for the fact that the
        /// test model knows nothing at all about intersecting test domains.  Probably
        /// what needs to happen is to allow the test model to have multiple roots.
        /// </todo>
        private sealed class Listener : ITestExplorationListener, ITestExecutionListener, IDisposable
        {
            private readonly ITestExplorationListener testExplorationListener;
            private readonly ITestExecutionListener testExecutionListener;
            private readonly Dictionary<string, string> redirectedSteps;

            private TestStepData rootTestStepData;
            private TestResult rootTestStepResult;
            private Stopwatch rootTestStepStopwatch;

            private bool wasRootTestStepStarted;

            public Listener(ITestExplorationListener testExplorationListener, ITestExecutionListener testExecutionListener)
            {
                this.testExplorationListener = testExplorationListener;
                this.testExecutionListener = testExecutionListener;

                redirectedSteps = new Dictionary<string, string>();

                rootTestStepStopwatch = Stopwatch.StartNew();
                rootTestStepData = new TestStepData(new BaseTestStep(new RootTest(), null));
                rootTestStepResult = new TestResult()
                {
                    Outcome = TestOutcome.Passed
                };
            }

            public void Dispose()
            {
                if (wasRootTestStepStarted)
                {
                    rootTestStepResult.Duration = rootTestStepStopwatch.Elapsed.TotalSeconds;
                    testExecutionListener.NotifyTestStepFinished(rootTestStepData.Id, rootTestStepResult);
                }
            }

            public void NotifySubtreeMerged(string parentTestId, TestData test)
            {
                testExplorationListener.NotifySubtreeMerged(parentTestId, test);
            }

            public void NotifyAnnotationAdded(AnnotationData annotation)
            {
                testExplorationListener.NotifyAnnotationAdded(annotation);
            }

            public void NotifyTestStepStarted(TestStepData step)
            {
                if (step.ParentId == null)
                {
                    redirectedSteps.Add(step.Id, rootTestStepData.Id);

                    if (!wasRootTestStepStarted)
                    {
                        testExecutionListener.NotifyTestStepStarted(rootTestStepData);
                        wasRootTestStepStarted = true;
                    }
                }
                else
                {
                    step.ParentId = Redirect(step.ParentId);
                    testExecutionListener.NotifyTestStepStarted(step);
                }
            }

            public void NotifyTestStepLifecyclePhaseChanged(string stepId, string lifecyclePhase)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLifecyclePhaseChanged(stepId, lifecyclePhase);
            }

            public void NotifyTestStepMetadataAdded(string stepId, string metadataKey, string metadataValue)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepMetadataAdded(stepId, metadataKey, metadataValue);
            }

            public void NotifyTestStepFinished(string stepId, TestResult result)
            {
                if (redirectedSteps.ContainsKey(stepId))
                {
                    rootTestStepResult.AssertCount += result.AssertCount;
                    rootTestStepResult.Outcome = rootTestStepResult.Outcome.CombineWith(result.Outcome);
                }
                else
                {
                    testExecutionListener.NotifyTestStepFinished(stepId, result);
                }
            }

            public void NotifyTestStepLogAttach(string stepId, Attachment attachment)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLogAttach(stepId, attachment);
            }

            public void NotifyTestStepLogStreamWrite(string stepId, string streamName, string text)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLogStreamWrite(stepId, streamName, text);
            }

            public void NotifyTestStepLogStreamEmbed(string stepId, string streamName,
                string attachmentName)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLogStreamEmbed(stepId, streamName, attachmentName);
            }

            public void NotifyTestStepLogStreamBeginSection(string stepId, string streamName, string sectionName)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLogStreamBeginSection(stepId, streamName, sectionName);
            }

            public void NotifyTestStepLogStreamBeginMarker(string stepId, string streamName, Marker marker)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLogStreamBeginMarker(stepId, streamName, marker);
            }

            public void NotifyTestStepLogStreamEnd(string stepId, string streamName)
            {
                stepId = Redirect(stepId);
                testExecutionListener.NotifyTestStepLogStreamEnd(stepId, streamName);
            }

            private string Redirect(string id)
            {
                string targetId;
                if (redirectedSteps.TryGetValue(id, out targetId))
                    return targetId;
                return id;
            }
        }
    }
}
