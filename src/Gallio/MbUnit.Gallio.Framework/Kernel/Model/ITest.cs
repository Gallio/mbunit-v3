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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// <para>
    /// A test object represents a single instance of a test that has been
    /// generated from a <see cref="ITemplate" /> using particular
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
    public interface ITest : ITestComponent, IModelTreeNode<ITest>
    {
        /// <summary>
        /// Gets or sets whether this test represents an individual test case
        /// as opposed to a test container such as a fixture or suite.  The value of
        /// this property can be used by the test harness to avoid processing containers
        /// that have no test cases.  It can also be used by the reporting infrastructure
        /// to constrain output statistics to test cases only.
        /// </summary>
        /// <remarks>
        /// This value is defined as a property rather than as a metadata key because it
        /// can modify the semantics of test execution.
        /// </remarks>
        bool IsTestCase { get; set; }

        /// <summary>
        /// Gets or sets the template binding from which the test was produced,
        /// or null if this test was not produced from any particular template.
        /// </summary>
        ITemplateBinding TemplateBinding { get; set; }

        /// <summary>
        /// Gets the list of the dependencies of this test.
        /// </summary>
        /// <remarks>
        /// It is an error to create a dependency on a <see cref="ITest" /> that
        /// belongs to a different <see cref="TestBatch" />.
        /// </remarks>
        IList<ITest> Dependencies { get; }

        /// <summary>
        /// Gets the scope of this test.
        /// </summary>
        TestScope Scope { get; }

        /// <summary>
        /// Gets or sets the test batch to which the test belongs.
        /// </summary>
        /// <remarks>
        /// The test inherits the value of the <see cref="Batch" /> property from its parent
        /// unless it is overridden here.  It is an error to set the value of this property
        /// so as to create a nested <see cref="TestBatch" />.
        /// </remarks>
        /// <seealso cref="TestBatch"/> for a discussion of the rules governing test batches.
        TestBatch Batch { get; set; }
    }
}
