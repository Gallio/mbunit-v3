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

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// <para>
    /// A test object represents a single instance of a test that has been
    /// generated from a <see cref="ITestTemplate" /> using particular
    /// bindings in a particular scope.
    /// </para>
    /// <para>
    /// A test may depend on one or more other tests.  When a test
    /// fails, the tests that depend on it are also automatically
    /// considered failures.  Moreover, the test runner ensures
    /// that a test will only run once all of its dependencies have
    /// completed execution successfully.  A run-time error will
    /// occur when the system detects the presence of circular test dependencies
    /// or attempts to execute a test concurrently with its dependencies.
    /// </para>
    /// <para>
    /// A test may be decomposed into a tree of subtests.  The subtests
    /// encapsulate logical units of processing within a test.  This feature
    /// makes it easier to isolate individual verification activities that
    /// are performed as part of some larger scenario.  Subtests are executed
    /// in dependency order just like ordinary tests.
    /// </para>
    /// <para>
    /// A test is executed in isolation of other tests only insofar as they belong
    /// to disjoint (and mutually exclusive) scopes.  Thus tests belonging to the same
    /// assembly-level scope will not be executed in pure isolation; instead, they
    /// will share the environment established by their common ancestor as part of
    /// setup/teardown activities.  For example, if the common ancestor includes rules to
    /// set up and tear down a temporary database, then descendent tests may share the
    /// same database whereas tests in different assembly-level scopes generally will
    /// not (unless there are side-effects).
    /// </para>
    /// </summary>
    public interface ITest : ITestComponent
    {
        /// <summary>
        /// Gets or sets the parent of this test, or null if this test
        /// is at the root of the test tree.
        /// </summary>
        ITest Parent { get; set; }

        /// <summary>
        /// Gets the children of this test.
        /// The children are considered subordinate to the parent.
        /// </summary>
        IEnumerable<ITest> Children { get; }

        /// <summary>
        /// Gets the list of the dependencies of this test.
        /// </summary>
        IList<ITest> Dependencies { get; }

        /// <summary>
        /// Adds a child test.
        /// Sets the child's parent to this test as part of the addition process.
        /// </summary>
        /// <param name="test">The test to add</param>
        /// <exception cref="NotSupportedException">Thrown if the test does not support
        /// the addition of arbitrary children (because it has some more specific internal structure)</exception>
        void AddChild(ITest test);
    }
}
