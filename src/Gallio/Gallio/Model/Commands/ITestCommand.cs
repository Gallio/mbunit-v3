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
using Gallio.Model.Contexts;
using Gallio.Model.Tree;

namespace Gallio.Model.Commands
{
    /// <summary>
    /// A test command requests the execution of a tree of <see cref="Tree.Test" />s.
    /// The test command hierarchy mirrors a filtered subset of the test hierarchy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The order in which commands appear in the command tree is significant.
    /// Commands should be sorted such that if command A depends on command B,
    /// then B will appear before A in a pre-order traversal of the tree.
    /// </para>
    /// <para>
    /// The ordering constraint is intended to simplify the implementation of
    /// <see cref="ITestDriver"/>s.  Test drivers may assume that executing
    /// <see cref="ITestCommand" />s in pre-order traversal sequence will be
    /// sufficient to ensure that all dependencies can be evaluated in time.
    /// </para>
    /// <para>
    /// However, a <see cref="ITestDriver" /> is not required to run the
    /// tests in the specified order.  Moreover, it is not required to run them
    /// serially at all.  (In fact, it is not even required to use this API.)
    /// A smart <see cref="ITestDriver" /> might run tests in parallel or take
    /// into account additional sequencing constraints governing execution order.
    /// </para>
    /// <para>
    /// In order to achieve correct behavior, a <see cref="ITestDriver" />
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
        Test Test { get; }

        /// <summary>
        /// Gets the total number of tests in the command subtree, including itself.
        /// </summary>
        int TestCount { get; }

        /// <summary>
        /// Returns true if the test was explicitly selected by a filter.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This flag enables test controllers to distinguish the case when a particular test
        /// was explicitly selected by a user-specified filter rather than by virtue of
        /// one of its parents having been selected.
        /// </para>
        /// </remarks>
        bool IsExplicit { get; }

        /// <summary>
        /// Gets the number of times that a root step of this test has failed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this field is automatically updated as each root step
        /// created by <see cref="StartPrimaryChildStep"/> finishes.
        /// </para>
        /// </remarks>
        int RootStepFailureCount { get; }

        /// <summary>
        /// Gets the list of child commands to run within the scope of this command.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each child command represents a test that is a child of the test
        /// managed by this command.
        /// </para>
        /// <para>
        /// The children are listed in an order that is consistent with
        /// their dependencies.  See class commends for details.
        /// </para>
        /// </remarks>
        IList<ITestCommand> Children { get; }

        /// <summary>
        /// Gets the list of other commands that this command depends upon.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The dependent commands are guaranteed to have appeared before this
        /// command in a pre-order traversal of the command tree.
        /// A test command cannot depend on one of its direct ancestors.
        /// </para>
        /// <para>
        /// There must be no circular dependencies.
        /// </para>
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
        /// <returns>The list of all command.</returns>
        IList<ITestCommand> GetAllCommands();

        /// <summary>
        /// Returns true if all of the dependencies of this test command have
        /// been satisfied.
        /// </summary>
        /// <returns>True if the dependencies of this test command have been satisfied.</returns>
        bool AreDependenciesSatisfied();

        /// <summary>
        /// Starts a new step of the test using the specified test step object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The current thread's test context is set to a new context for the
        /// test step that is starting.  The new context will be a child of the
        /// current thread's context.
        /// </para>
        /// </remarks>
        /// <param name="testStep">The test step to start.</param>
        /// <returns>The test context for the test step.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStep"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="testStep"/> does not
        /// belong to the test associated with this test command.</exception>
        ITestContext StartStep(TestStep testStep);

        /// <summary>
        /// Starts the primary step of the test associated with this command as a child
        /// of the specified test step and returns its test context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is equivalent to calling <see cref="StartStep" />
        /// using a default implementation of <see cref="TestStep" /> that is
        /// initialized using <paramref name="parentTestStep" /> and the metadata
        /// from the 
        /// </para>
        /// </remarks>
        /// <param name="parentTestStep">The parent test step, or null if none.</param>
        /// <returns>The test context for the new primary test step.</returns>
        /// <seealso cref="StartStep"/>
        ITestContext StartPrimaryChildStep(TestStep parentTestStep);
    }
}