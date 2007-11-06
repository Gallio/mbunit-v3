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
using System.Text;
using Gallio.Model.Execution;
using Gallio.Core.ProgressMonitoring;
using Gallio.Model.Filters;
using Gallio.Model;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// <para>
    /// A test plan plans and tracks the execution of tests.  A <see cref="ITestController" />
    /// interacts with the test plan to decide what to run next based on the dependencies
    /// among tests and the results of dependent tests.
    /// </para>
    /// <para>
    /// A test plan provides the following guarantees:
    /// <list type="bullet">
    /// <item>A test runs within the scope of its parent test.</item>
    /// <item>A test runs only after all of the tests it depends on have passed.</item>
    /// <item>A test with a failed or circular dependency always fails.</item>
    /// <item>A test result is always reported for each test, even if the test did not run.</item>
    /// </list>
    /// </para>
    /// <para>
    /// The sequence of operations involving the use of a test plan is as follows:
    /// <list type="bullet">
    /// <item>The test harness creates a test plan.</item>
    /// <item>The <see cref="ScheduleTests" /> method is called at least once
    /// until some tests are added.</item>
    /// <item>The <see cref="RunTests" /> method is called.</item>
    /// <item>The test plan internally runs tests in dependency order until it
    /// finds a master test in the tree.</item>
    /// <item>When a master test is found, the test plan uses the factory returned by
    /// <see cref="ITest.TestControllerFactory" /> to create a test controller.
    /// Then it invokes <see cref="ITestController.RunTests" /> and passes in
    /// a <see cref="ITestMonitor" /> corresponding to the master test
    /// that was found.</item>
    /// <item>When <see cref="RunTests" /> returns, the test harness then calls
    /// <see cref="CleanUpTests" /> to clean up the state of any tests that
    /// did not complete normally.  For example, an incorrectly written
    /// <see cref="ITestController" /> might fail to call <see cref="IStepMonitor.FinishStep" />
    /// for some steps when runtime errors occurs.</item>
    /// <item>The test plan object can now be recycled.</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// All operations on a test plan assume that a single client is using
    /// the test plan from a single thread at a time.  However, the <see cref="ITestMonitor" />
    /// and <see cref="IStepMonitor" /> objects yielded by a test plan are safe
    /// for multi-threaded access.
    /// </para>
    /// <para>
    /// It is not possible to schedule additional tests while tests are running.
    /// </para>
    /// </remarks>
    public interface ITestPlan
    {
        /// <summary>
        /// Schedules a filtered tree of tests to run as part of the test plan.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The tree of tests is scanned recursively for filter matches.  When a filter match
        /// is found, that test, its descendants and its ancestors are scheduled for execution.
        /// If no filter matches are found then this method returns <c>false</c> and does not
        /// schedule any tests.
        /// </para>
        /// <para>
        /// A test plan implementation might place constraints on how many batches
        /// of tests can be scheduled with <see cref="ScheduleTests" /> at one time.
        /// However, all implementations are guaranteed to support at least one
        /// batch of tests.
        /// </para>
        /// </remarks>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="rootTest">The root test in the tree</param>
        /// <param name="options">The test execution options, including the test filter</param>
        /// <returns>True if any tests were actually scheduled</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/>,
        /// <paramref name="rootTest"/> or <paramref name="options"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="rootTest"/> is
        /// not the root of a test tree</exception>
        bool ScheduleTests(IProgressMonitor progressMonitor, ITest rootTest, TestExecutionOptions options);

        /// <summary>
        /// Runs the currently scheduled tests.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        void RunTests(IProgressMonitor progressMonitor);

        /// <summary>
        /// Submits results for all tests that have not yet run, ends any abandoned
        /// tests and clears the list of remaining tests to run.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progressMonitor"/> is null</exception>
        void CleanUpTests(IProgressMonitor progressMonitor);
    }
}
