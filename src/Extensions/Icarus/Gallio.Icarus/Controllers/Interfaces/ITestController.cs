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
using System.ComponentModel;
using System.Threading;
using Gallio.Concurrency;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface ITestController : IDisposable
    {
        /// <summary>
        /// Gets the list of currently selected tests.
        /// </summary>
        BindingList<TestTreeNode> SelectedTests { get; }

        /// <summary>
        /// Indicator if any tests failed during the last run.
        /// </summary>
        bool FailedTests { get; }

        /// <summary>
        /// Gets the current test tree model.
        /// </summary>
        ITestTreeModel Model { get; }

        /// <summary>
        /// Gets or sets the current tree category.
        /// </summary>
        string TreeViewCategory { get; set; }

        /// <summary>
        /// Gets the list of all test framework names.
        /// </summary>
        IList<string> TestFrameworks { get; }

        /// <summary>
        /// Gets the total number of tests.
        /// </summary>
        int TestCount { get; }
        SynchronizationContext SynchronizationContext { get; set; }
        bool FilterPassed { get; set; }
        bool FilterFailed { get; set; }
        bool FilterInconclusive { get; set; }
        bool SortAsc { get; set; }
        bool SortDesc { get; set; }

        /// <summary>
        /// Event raised after each test step completes.
        /// </summary>
        event EventHandler<TestStepFinishedEventArgs> TestStepFinished;

        /// <summary>
        /// Event raised when showing source code.
        /// FIXME: Does not belong here.
        /// </summary>
        event EventHandler<ShowSourceCodeEventArgs> ShowSourceCode;

        event EventHandler RunStarted;
        event EventHandler RunFinished;
        event EventHandler ExploreStarted;
        event EventHandler ExploreFinished;

        /// <summary>
        /// Sets the test runner factory used by the test controller.
        /// </summary>
        /// <param name="testRunnerFactory">The test runner factory to use</param>
        void SetTestRunnerFactory(ITestRunnerFactory testRunnerFactory);

        /// <summary>
        /// Sets the test package configuration to be used during subsequent calls to <see cref="Explore" /> or <see cref="Run" />.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        void SetTestPackageConfig(TestPackageConfig testPackageConfig);

        /// <summary>
        /// Acquires a read lock on the report and executes the specified action.
        /// </summary>
        /// <param name="action">The action to execute within the context of the read lock</param>
        void ReadReport(ReadAction<Report> action);

        /// <summary>
        /// Applies a filter to the tests, potentially altering selections.
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        void ApplyFilter(Filter<ITest> filter);

        /// <summary>
        /// Generates a filter from selected tests.
        /// </summary>
        /// <returns>The generated filter</returns>
        Filter<ITest> GenerateFilterFromSelectedTests();

        /// <summary>
        /// Explores the tests and updates the model, does not run them.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        void Explore(IProgressMonitor progressMonitor);

        /// <summary>
        /// Runs the tests and updates the model.
        /// </summary>
        /// <param name="debug">If true, runs tests with the debugger</param>
        /// <param name="progressMonitor">The progress monitor</param>
        void Run(bool debug, IProgressMonitor progressMonitor);

        /// <summary>
        /// Refreshes the contents of the test tree based on the tests most recently run or explored.
        /// </summary>
        /// <param name="progressMonitor">The progress monitor</param>
        void RefreshTestTree(IProgressMonitor progressMonitor);

        /// <summary>
        /// Views the source code associated with a particular test.
        /// </summary>
        /// <param name="testId">The test id</param>
        /// <param name="progressMonitor">The progress monitor</param>
        void ViewSourceCode(string testId, IProgressMonitor progressMonitor);

        /// <summary>
        /// Resets the status of all tests.
        /// </summary>
        void ResetTestStatus(IProgressMonitor progressMonitor);
    }
}
