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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// <para>
    /// A test command requests the execution of a tree of <see cref="ITest" />s.
    /// The test command hierarchy mirrors a filtered subset of the test hierarchy.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The order in which commands appear in the command tree is significant.
    /// Commands should be sorted such that if command A depends on command B,
    /// then B will appear before A in a pre-order traversal of the tree.
    /// </para>
    /// <para>
    /// The ordering constraint is intended to simplify the implementation of
    /// <see cref="ITestController"/>s.  Test controllers may assume that executing
    /// <see cref="ITestCommand" />s in pre-order traversal sequence will be
    /// sufficient to ensure that all dependencies can be evaluated in time.
    /// </para>
    /// <para>
    /// However, a <see cref="ITestController" /> is NOT required to run the
    /// tests in the specified order.  Moreover, it is NOT required to run them
    /// serially at all.  A smart <see cref="ITestController" /> might run
    /// tests in parallel or take into account additional sequencing constraints
    /// governing execution order.
    /// </para>
    /// <para>
    /// In order to achieve correct behavior, a <see cref="ITestController" />
    /// should satisfy the following guarantees with respect to test commands:
    /// <list type="bullet">
    /// <item>A test command runs within the scope of its parent test command.</item>
    /// <item>A test command runs only after all of the test commands it depends on have passed.</item>
    /// <item>A test command with a failed dependency or that is not run for internal reasons reports a test outcome of <see cref="TestOutcome.Skipped" />.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface ITestCommand
    {
        /// <summary>
        /// Gets the test that is to be executed.
        /// </summary>
        ITest Test { get; }

        /// <summary>
        /// Gets the total number of tests in the command subtree, including itself.
        /// </summary>
        int TestCount { get; }

        /// <summary>
        /// Returns true if the test was explicitly selected by a filter.
        /// </summary>
        /// <remarks>
        /// This flag enables test controllers to distinguish the case when a particular test
        /// was explicitly selected by a user-specified filter rather than by virtue of
        /// one of its parents having been selected.
        /// </remarks>
        bool IsExplicit { get; }

        /// <summary>
        /// Gets the number of times that a root step of this test has failed.
        /// </summary>
        /// <remarks>
        /// The value of this field is automatically updated as each root step
        /// created by <see cref="StartPrimaryChildStep"/> finishes.
        /// </remarks>
        int RootStepFailureCount { get; }

        /// <summary>
        /// <para>
        /// Gets the list of child commands to run within the scope of this command.
        /// </para>
        /// <para>
        /// Each child command represents a test that is a child of the test
        /// managed by this command.
        /// </para>
        /// <para>
        /// The children are listed in an order that is consistent with
        /// their dependencies.  See class commends for details.
        /// </para>
        /// </summary>
        IList<ITestCommand> Children { get; }

        /// <summary>
        /// <para>
        /// Gets the list of other commands that this command depends upon.
        /// </para>
        /// <para>
        /// The dependent commands are guaranteed to have appeared before this
        /// command in a pre-order traversal of the command tree.
        /// A test command cannot depend on one of its direct ancestors.
        /// </para>
        /// </summary>
        /// <remarks>
        /// There must be no circular dependencies.
        /// </remarks>
        IList<ITestCommand> Dependencies { get; }

        /// <summary>
        /// Enumerates this command and all of its descendants in pre-order tree
        /// traversal.
        /// </summary>
        IEnumerable<ITestCommand> PreOrderTraversal { get; }

        /// <summary>
        /// Gets a list consisting of this command and all of its descendants as
        /// enumerated by pre-order tree traversal.
        /// </summary>
        /// <returns>The list of all command</returns>
        IList<ITestCommand> GetAllCommands();

        /// <summary>
        /// Returns true if all of the dependencies of this test command have
        /// been satisfied.
        /// </summary>
        /// <returns>True if the dependencies of this test command have been satisfied</returns>
        bool AreDependenciesSatisfied();

        /// <summary>
        /// Starts a new step of the test using the specified test step object.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is set to a new context for the
        /// test step that is starting.  The new context will be a child of the
        /// current thread's context.
        /// </remarks>
        /// <param name="testStep">The test step to start</param>
        /// <returns>The test context for the test step</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStep"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="testStep"/> does not
        /// belong to the test associated with this test command</exception>
        ITestContext StartStep(ITestStep testStep);

        /// <summary>
        /// <para>
        /// Starts the primary step of the test associated with this command as a child
        /// of the specified test step and returns its test context.
        /// </para>
        /// <para>
        /// This method is equivalent to calling <see cref="StartStep" />
        /// using a default implementation of <see cref="ITestStep" /> that is
        /// initialized using <paramref name="parentTestStep" />.
        /// </para>
        /// </summary>
        /// <param name="parentTestStep">The parent test step, or null if none</param>
        /// <returns>The test context for the new primary test step</returns>
        /// <seealso cref="StartStep"/>
        ITestContext StartPrimaryChildStep(ITestStep parentTestStep);
    }
}
