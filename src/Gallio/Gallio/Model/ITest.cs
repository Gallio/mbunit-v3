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
using Gallio.Common;
using Gallio.Model.Execution;

namespace Gallio.Model
{
    /// <summary>
    /// A test object represents a parameterized test case or test
    /// container.  The test parameters are used as placeholders for
    /// data-binding during test execution.  A single test can
    /// produce multiple steps (<seealso cref="ITestStep" />) at runtime.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="ITest" /> can be thought of as a declarative
    /// artifact that describes about what a test "looks like"
    /// from the outside based on available reflective metadata.
    /// A <see cref="ITestStep" /> is then the runtime counterpart of a
    /// <see cref="ITest" /> that is created to describe different
    /// parameter bindigns or other characteristics of a test's structure
    /// that become manifest only at runtime.
    /// </para>
    /// <para>
    /// A test may depend on one or more other tests.  When a test
    /// fails, the tests that depend on it are also automatically
    /// considered failures.  Moreover, the test harness ensures
    /// that a test will only run once all of its dependencies have
    /// completed execution successfully.  A run-time error will
    /// occur when the system detects the presence of circular test dependencies
    /// or attempts to execute a test concurrently with its dependencies.
    /// </para>
    /// <para>
    /// A test contain child tests.  The children of a test are executed
    /// in dependency order within the scope of the parent test.  Thus the parent
    /// test may setup/teardown the execution environment used to execute
    /// its children.  Tests that belong to different subtrees are executed in
    /// relative isolation within the common environment established by their common parent.
    /// </para>
    /// <para>
    /// The object model distinguishes between tests that represent individual test cases
    /// and other test containers.  Test containers are skipped if they do not
    /// contain any test cases or if none of their test cases have been selected for execution.
    /// </para>
    /// </remarks>
    public interface ITest : ITestComponent
    {
        /// <summary>
        /// Gets the full name of the test.  The full name is derived by concatenating the
        /// <see cref="FullName" /> of the <see cref="Parent"/> followed by a slash ('/')
        /// followed by the <see cref="ITestComponent.Name" /> of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The full name of the root test is empty.
        /// </para>
        /// </remarks>
        string FullName { get; }

        /// <summary>
        /// Gets a locally unique identifier for this test that satisfies the following conditions:
        /// </summary>
        /// <remarks>
        /// <para>
        /// <list type="bullet">
        /// <item>The identifier is unique among all siblings of this test belonging to the same parent.</item>
        /// <item>The identifier is likely to be stable across multiple sessions including
        /// changes and recompilations of the test projects.</item>
        /// <item>The identifier is non-null.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The local identifier may be the same as the test's name.  However since the name is
        /// intended for display to end-users, it may contain irrelevant details (such as version
        /// numbers) that would reduce its long-term stability.  In that case, a different
        /// local identifier should be selected such as one based on the test's
        /// <see cref="ITestComponent.CodeElement" /> and an ordering condition among siblings
        /// to guarantee uniqueness.
        /// </para>
        /// <para>
        /// The locally unique <see cref="LocalId" /> property may be used to generate the
        /// globally unique <see cref="ITestComponent.Id" /> property of a test by combining
        /// it with the locally unique identifiers of its parents.
        /// </para>
        /// </remarks>
        /// <returns>The locally unique identifier.</returns>
        string LocalId { get; }

        /// <summary>
        /// Gets whether this test represents an individual test case
        /// as opposed to a test container such as a fixture or suite.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this property can be used by the test harness to avoid processing 
        /// containers that have no test cases.  It can also be used by the reporting infrastructure
        /// to constrain output statistics to test cases only.
        /// </para>
        /// <para>
        /// Not all test cases are leaf nodes in the test tree and vice-versa.       
        /// </para>
        /// <para>
        /// This value is defined as a property rather than as a metadata key because it
        /// significantly changes the semantics of test execution.
        /// </para>
        /// </remarks>
        bool IsTestCase { get; }

        /// <summary>
        /// Gets or sets the parent of this test, or null if this is the root test.
        /// </summary>
        ITest Parent { get; set; }

        /// <summary>
        /// Gets a read-only list of the children of this test.
        /// </summary>
        IList<ITest> Children { get; }

        /// <summary>
        /// Gets a read-only list of the parameters of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each parameter must have a unique name.
        /// </para>
        /// <para>
        /// The order in which the parameters appear is not significant.
        /// </para>
        /// </remarks>
        IList<ITestParameter> Parameters { get; }

        /// <summary>
        /// Gets a read-only list of the dependencies of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Some test frameworks may choose to ignore test dependencies or may impose their own dependency schemes.
        /// </para>
        /// </remarks>
        IList<ITest> Dependencies { get; }

        /// <summary>
        /// Gets or sets a number that defines an ordering for the test with respect to its siblings.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Unless compelled otherwise by test dependencies, tests with a lower order number than
        /// their siblings will run before those siblings and tests with the same order number
        /// as their siblings with run in an arbitrary sequence with respect to those siblings.
        /// </para>
        /// <para>
        /// Some test frameworks may choose to ignore test order or may impose their own ordering schemes.
        /// </para>
        /// </remarks>
        /// <value>The test execution order with respect to siblings, initially zero.</value>
        int Order { get; set; }

        /// <summary>
        /// Gets a <see cref="ITestController" /> <see cref="Func{TResult}" /> to run this tes
        /// and all of its children.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Returns null if this test is merely a container for other tests or if it otherwise 
        /// does not require or provide its own controller.
        /// </para>
        /// <para>
        /// The top-most test that returns a non-null <see cref="Func{T}" /> is
        /// referred to as the master test.  It may contain other tests that also have
        /// non-null factories but there is no built-in mechanism provided to delegate control
        /// from one controller to another in the middle of its execution.
        /// Thus the nested controller will only be executed if the master controller chooses
        /// to implement a suitable delegation policy of this sort.
        /// </para>
        /// <para>
        /// During test execution, the test plan scans the list of tests to be executed
        /// in depth-first order to locate the master tests for each subtree.  The test plan
        /// instantiates and invokes a test controller for each master test.
        /// </para>
        /// <para>
        /// For example, the top-level test created by a <see cref="ITestFramework" />
        /// is usually a master test.  The <see cref="ITestController" /> created by the
        /// framework's master test will then take care of setting up the environment for the entire
        /// subtree beneath it and then actually running the tests.  Thus the interaction
        /// between the test harness and test controllers (as mediated by master tests) represents
        /// a division of labor among multiple possible test execution strategies.
        /// </para>
        /// </remarks>
        /// <returns>The test controller factory, or null if this test cannot produce
        /// a controller (and consequently is not a master test according to the definition above)</returns>
        Func<ITestController> TestControllerFactory { get; }

        /// <summary>
        /// Adds a test parameter and sets its <see cref="ITestParameter.Owner" /> property.
        /// </summary>
        /// <param name="parameter">The test parameter to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="parameter"/> is already
        /// owned by some other test.</exception>
        void AddParameter(ITestParameter parameter);

        /// <summary>
        /// Adds a child test and sets its <see cref="ITest.Parent" /> property.
        /// </summary>
        /// <param name="test">The test to add as a child.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="test"/> is already
        /// the child of some other test.</exception>
        void AddChild(ITest test);

        /// <summary>
        /// Adds a test dependency.
        /// </summary>
        /// <param name="test">The test to add as a dependency.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        void AddDependency(ITest test);

        /// <summary>
        /// Obtains a unique local id for a child of this test.
        /// </summary>
        /// <param name="localIdHint">A suggested id which will be used if no conflicts occur.</param>
        /// <returns>The unique local id to use.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="localIdHint"/> is null.</exception>
        string GetUniqueLocalIdForChild(string localIdHint);
    }
}
