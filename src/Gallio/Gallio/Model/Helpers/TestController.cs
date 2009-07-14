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
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model.Helpers
{
    /// <summary>
    /// A test controller executes tests defined by a tree of <see cref="ITestCommand" />s.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test controller may be stateful because it is used for only one test run then disposed.
    /// </para>
    /// <para>
    /// Subclasses of this class provide the algorithm used to execute the commands.
    /// </para>
    /// </remarks>
    public abstract class TestController : IDisposable
    {
        /// <summary>
        /// Disposes the test controller.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the test controller.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()" /> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method can be called at most once during the lifetime of a test controller.
        /// </para>
        /// </remarks>
        /// <param name="rootTestCommand">The root test monitor.</param>
        /// <param name="parentTestStep">The parent test step, or null if starting a root step.</param>
        /// <param name="options">The test execution options.</param>
        /// <param name="progressMonitor">The progress monitor.</param>
        /// <returns>The combined result of the root test command.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootTestCommand"/>
        /// <paramref name="progressMonitor"/>, or <paramref name="options"/> is null.</exception>
        public TestResult Run(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor)
        {
            if (rootTestCommand == null)
                throw new ArgumentNullException("rootTestCommand");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (options == null)
                throw new ArgumentNullException("options");

            return RunImpl(rootTestCommand, parentTestStep, options, progressMonitor);
        }

        /// <summary>
        /// Implementation of <see cref="Run" /> called after argument validation has taken place.
        /// </summary>
        /// <param name="rootTestCommand">The root test command, not null.</param>
        /// <param name="parentTestStep">The parent test step, or null if none.</param>
        /// <param name="options">The test execution options, not null.</param>
        /// <param name="progressMonitor">The progress monitor, not null.</param>
        /// <returns>The combined result of the root test command.</returns>
        protected abstract TestResult RunImpl(ITestCommand rootTestCommand, TestStep parentTestStep, TestExecutionOptions options, IProgressMonitor progressMonitor);

        /// <summary>
        /// Recursively generates single test steps for each <see cref="ITestCommand" /> and
        /// sets the final outcome to <see cref="TestOutcome.Skipped" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is useful for implementing fallback behavior when 
        /// <see cref="TestExecutionOptions.SkipTestExecution" /> is true.
        /// </para>
        /// </remarks>
        /// <param name="rootTestCommand">The root test command.</param>
        /// <param name="parentTestStep">The parent test step.</param>
        /// <returns>The combined result of the test commands.</returns>
        protected static TestResult SkipAll(ITestCommand rootTestCommand, TestStep parentTestStep)
        {
            ITestContext context = rootTestCommand.StartPrimaryChildStep(parentTestStep);

            foreach (ITestCommand child in rootTestCommand.Children)
                SkipAll(child, context.TestStep);

            return context.FinishStep(TestOutcome.Skipped, null);
        }
    }
}