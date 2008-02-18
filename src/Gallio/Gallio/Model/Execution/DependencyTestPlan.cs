// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Hosting.ProgressMonitoring;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test plan based on a topological sort of the tests according to
    /// their dependencies.
    /// </summary>
    /// FIXME: Missing topological sort
    /// FIXME: Should override Passed / Inconclusive outcome with Failed / Inconclusive if
    ///        a child test step passes/fails.
    /// FIXME: Need high-level cancelation support to tolerate buggy test controllers
    ///        that have somehow managed to get hung up.
    public class DependencyTestPlan : ITestPlan
    {
        private readonly ITestContextManager contextManager;
        private ManagedTestCommand rootTestCommand;

        /// <summary>
        /// Creates an empty dependency test plan.
        /// </summary>
        /// <param name="contextManager">The context manager</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextManager"/> is null</exception>
        public DependencyTestPlan(ITestContextManager contextManager)
        {
            if (contextManager == null)
                throw new ArgumentNullException("contextManager");

            this.contextManager = contextManager;
        }

        /// <inheritdoc />
        public bool ScheduleTests(IProgressMonitor progressMonitor, TestModel testModel, TestExecutionOptions options)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");
            if (testModel == null)
                throw new ArgumentNullException(@"testModel");
            if (options == null)
                throw new ArgumentNullException(@"options");

            using (progressMonitor)
            {
                progressMonitor.BeginTask("Preparing the test plan.", 1);

                if (rootTestCommand != null)
                    throw new InvalidOperationException("The currently scheduled tests must be run or cleaned up before a new batch of tests can be scheduled.");

                Dictionary<ITest, ManagedTestCommand> lookupTable = new Dictionary<ITest, ManagedTestCommand>();
                rootTestCommand = ScheduleFilteredClosure(lookupTable, testModel.RootTest, options);

                return rootTestCommand != null;
            }
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException(@"progressMonitor");

            // Note we just pass the progress monitor straight into the RootTestController
            // after setting up the environent appropriately without calling BeginTask first.
            using (progressMonitor)
            {
                if (rootTestCommand != null)
                {
                    RecursivelyRunAllTestsWithinANullContext(progressMonitor);
                    rootTestCommand = null;
                }
            }
        }

        private ManagedTestCommand ScheduleFilteredClosure(IDictionary<ITest, ManagedTestCommand> lookupTable, ITest test, TestExecutionOptions options)
        {
            if (options.Filter.IsMatch(test))
                return ScheduleSubtree(lookupTable, test, true);

            List<ManagedTestCommand> children = new List<ManagedTestCommand>(test.Children.Count);

            foreach (ITest child in test.Children)
            {
                ManagedTestCommand childMonitor = ScheduleFilteredClosure(lookupTable, child, options);
                if (childMonitor != null)
                    children.Add(childMonitor);
            }

            if (children.Count != 0)
                return ScheduleTest(lookupTable, test, children, false);

            return null;
        }

        private ManagedTestCommand ScheduleSubtree(IDictionary<ITest, ManagedTestCommand> lookupTable, ITest test, bool isExplicit)
        {
            List<ManagedTestCommand> children = new List<ManagedTestCommand>(test.Children.Count);

            foreach (ITest child in test.Children)
                children.Add(ScheduleSubtree(lookupTable, child, false));

            return ScheduleTest(lookupTable, test, children, isExplicit);
        }

        private ManagedTestCommand ScheduleTest(IDictionary<ITest, ManagedTestCommand> lookupTable, ITest test, IEnumerable<ManagedTestCommand> children, bool isExplicit)
        {
            ManagedTestCommand testMonitor = new ManagedTestCommand(contextManager, test, isExplicit);
            foreach (ManagedTestCommand child in children)
                testMonitor.AddChild(child);

            lookupTable.Add(test, testMonitor);
            return testMonitor;
        }

        private void RecursivelyRunAllTestsWithinANullContext(IProgressMonitor progressMonitor)
        {
            using (contextManager.ContextTracker.EnterContext(null))
            {
                Func<ITestController> rootTestControllerFactory = rootTestCommand.Test.TestControllerFactory;

                if (rootTestControllerFactory != null)
                {
                    using (ITestController controller = rootTestControllerFactory())
                        controller.RunTests(progressMonitor, rootTestCommand, null);
                }
            }
        }
    }
}
