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
using System.Reflection;
using System.Threading;
using Gallio.Common;
using Gallio.Common.Concurrency;
using Gallio.Common.Reflection;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Environments;
using Gallio.Model.Messages;
using Gallio.Model.Tree;
using Gallio.Common.Messaging;
using Gallio.Runtime;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// A simple test driver explores tests using a <see cref="TestExplorer" />
    /// and runs them using a <see cref="TestController" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The simple test driver class provides a good foundation for implementing
    /// test drivers for most .Net frameworks.  All you need to do is override the
    /// required methods to provide a <see cref="TestExplorer" /> to populate a
    /// <see cref="TestModel" /> and a <see cref="TestController" /> to run the
    /// <see cref="TestModel" />.  The tests run within an environment configured
    /// by the <see cref="ITestEnvironmentManager" />.
    /// </para>
    /// </remarks>
    public abstract class SimpleTestDriver : DotNetTestDriver
    {
        /// <inheritdoc />
        sealed protected override void DescribeImpl(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(string.Format("Describing {0} tests.", FrameworkName), 100))
            {
                progressMonitor.SetStatus("Building the test model.");
                GenerateTestModel(reflectionPolicy, codeElements, messageSink);
            }
        }

        /// <inheritdoc />
        sealed protected override void ExploreAssembly(Assembly assembly, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(string.Format("Exploring {0} tests.", FrameworkName), 5))
            {
                using (TestHarness testHarness = CreateTestHarness())
                {
                    try
                    {
                        progressMonitor.SetStatus("Setting up the test harness.");
                        testHarness.SetUp();
                        progressMonitor.Worked(1);

                        progressMonitor.SetStatus("Building the test model.");
                        GenerateTestModel(assembly, messageSink);
                        progressMonitor.Worked(3);
                    }
                    finally
                    {
                        progressMonitor.SetStatus("Tearing down the test harness.");
                        testHarness.TearDown();
                        progressMonitor.Worked(1);
                    }
                }
            }
        }

        /// <inheritdoc />
        sealed protected override void RunAssembly(Assembly assembly, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask(string.Format("Running {0} tests.", FrameworkName), 100))
            {
                using (TestHarness testHarness = CreateTestHarness())
                {
                    try
                    {
                        progressMonitor.SetStatus("Setting up the test harness.");
                        testHarness.SetUp();
                        progressMonitor.Worked(1);

                        progressMonitor.SetStatus("Building the test model.");
                        TestModel testModel = GenerateTestModel(assembly, messageSink);
                        progressMonitor.Worked(3);

                        progressMonitor.SetStatus("Building the test commands.");
                        ITestContextManager testContextManager = CreateTestContextManager(messageSink);
                        ITestCommand rootTestCommand = GenerateCommandTree(testModel, testExecutionOptions, testContextManager);
                        progressMonitor.Worked(2);

                        progressMonitor.SetStatus("Running the tests.");
                        if (rootTestCommand != null)
                        {
                            RunTestCommands(rootTestCommand, testExecutionOptions, testHarness, testContextManager, progressMonitor.CreateSubProgressMonitor(93));
                        }
                        else
                        {
                            progressMonitor.Worked(93);
                        }
                    }
                    finally
                    {
                        progressMonitor.SetStatus("Tearing down the test harness.");
                        testHarness.TearDown();
                        progressMonitor.Worked(1);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the framework as it should appear in progress messages.
        /// </summary>
        protected abstract string FrameworkName { get; }

        /// <summary>
        /// Creates a test explorer to describe and explore tests.
        /// </summary>
        /// <returns>The test explorer.</returns>
        protected abstract TestExplorer CreateTestExplorer();

        /// <summary>
        /// Creates a test controller to run tests.
        /// </summary>
        /// <returns>The test controller.</returns>
        protected abstract TestController CreateTestController();

        /// <summary>
        /// Creates a test harness to configure the test environment.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns a new <see cref="TestEnvironmentAwareTestHarness" />
        /// configured with the <see cref="ITestEnvironmentManager" /> registered
        /// with the runtime.
        /// </para>
        /// </remarks>
        /// <returns>The test harness.</returns>
        protected virtual TestHarness CreateTestHarness()
        {
            return new TestEnvironmentAwareTestHarness(GetTestEnvironmentManager());
        }

        /// <summary>
        /// Creates a test command factory for generating <see cref="ITestCommand"/>s from
        /// a <see cref="TestModel"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns a new <see cref="DefaultTestCommandFactory" />
        /// with a default implementation.
        /// </para>
        /// </remarks>
        /// <returns>The test command factory.</returns>
        protected virtual ITestCommandFactory CreateTestCommandFactory()
        {
            return new DefaultTestCommandFactory();
        }

        /// <summary>
        /// Creates a test context manager for tracking test context and publishing
        /// test messages to a message sink.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default implementation returns a new <see cref="ObservableTestContextManager" />
        /// attached to the current <see cref="TestContextTrackerAccessor.Instance" />
        /// and the <see cref="IMessageSink" />.
        /// </para>
        /// </remarks>
        /// <param name="messageSink">The message sink to which test messages are published.</param>
        /// <returns>The test command factory.</returns>
        protected virtual ITestContextManager CreateTestContextManager(IMessageSink messageSink)
        {
            return new ObservableTestContextManager(TestContextTrackerAccessor.Instance, messageSink);
        }

        private static ITestEnvironmentManager GetTestEnvironmentManager()
        {
            return RuntimeAccessor.ServiceLocator.Resolve<ITestEnvironmentManager>();
        }

        private TestModel GenerateTestModel(Assembly assembly, IMessageSink messageSink)
        {
            return GenerateTestModel(Reflector.NativeReflectionPolicy,
                new[] { Reflector.Wrap(assembly) }, messageSink);
        }

        private TestModel GenerateTestModel(IReflectionPolicy reflectionPolicy, IEnumerable<ICodeElementInfo> codeElements, IMessageSink messageSink)
        {
            using (TestExplorer testExplorer = CreateTestExplorer())
            {
                foreach (var codeElement in codeElements)
                    testExplorer.Explore(reflectionPolicy, codeElement);

                testExplorer.Finish();

                TestModelSerializer.PublishTestModel(testExplorer.TestModel, messageSink);
                return testExplorer.TestModel;
            }
        }

        private ITestCommand GenerateCommandTree(TestModel testModel, TestExecutionOptions testExecutionOptions, ITestContextManager testContextManager)
        {
            ITestCommandFactory testCommandFactory = CreateTestCommandFactory();

            ITestCommand rootCommand = testCommandFactory.BuildCommands(testModel, testExecutionOptions.FilterSet,
                testExecutionOptions.ExactFilter, testContextManager);
            return rootCommand;
        }

        private void RunTestCommands(ITestCommand rootTestCommand, TestExecutionOptions options, TestHarness testHarness, ITestContextManager testContextManager, IProgressMonitor progressMonitor)
        {
            Action action = () =>
            {
                testHarness.Run(() =>
                {
                    using (testContextManager.ContextTracker.EnterContext(null))
                    {
                        using (TestController testController = CreateTestController())
                        {
                            testController.Run(rootTestCommand, null, options, progressMonitor);
                        }
                    }
                });
            };

            if (options.SingleThreaded)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    throw new ModelException("A fatal exception occurred while running test commands.", ex);
                }
            }
            else
            {
                var task = new ThreadTask("Test Runner", action);

                // Use STA as the default for all tests.  A test framework may of course choose
                // to create its own threads with different apartment states.
                task.ApartmentState = ApartmentState.STA;
                task.Run(null);

                if (task.Result.Exception != null)
                    throw new ModelException("A fatal exception occurred while running test commands.",
                        task.Result.Exception);
            }
        }
    }
}