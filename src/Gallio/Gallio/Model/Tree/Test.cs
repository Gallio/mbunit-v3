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
using Gallio.Common.Collections;
using Gallio.Common;
using Gallio.Common.Reflection;
using Gallio.Properties;

namespace Gallio.Model.Tree
{
    /// <summary>
    /// A test object represents a parameterized test case or test
    /// container.  The test parameters are used as placeholders for
    /// data-binding during test execution.  A single test can
    /// produce multiple steps (<seealso cref="TestStep" />) at runtime.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="Test" /> can be thought of as a declarative
    /// artifact that describes about what a test "looks like"
    /// from the outside based on available reflective metadata.
    /// A <see cref="TestStep" /> is then the runtime counterpart of a
    /// <see cref="Test" /> that is created to describe different
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
    /// <para>
    /// The kind of type type of test is set to <see cref="TestKinds.Group" /> by default.
    /// </para>
    /// </remarks>
    public class Test : TestComponent
    {
        private List<TestParameter> parameters;
        private List<Test> children;
        private List<Test> dependencies;
        private HashSet<string> assignedChildLocalIds;
        private Test parent;
        private bool isTestCase;
        private int order;

        private string id;
        private string cachedId;
        private string cachedLocalId;
        private string localIdHint;

        /// <summary>
        /// Initializes a test initially without a parent.
        /// </summary>
        /// <param name="name">The name of the test.</param>
        /// <param name="codeElement">The point of definition of the test, or null if unknown.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        public Test(string name, ICodeElementInfo codeElement)
            : base(name, codeElement)
        {
            Kind = TestKinds.Group;
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                if (id != null)
                    return id;

                // We compute an id using the combined hash of the local ids
                // from this test up the tree to the root.  A hash is used to
                // keep the ids relatively short and to prevent people from 
                // making undue assumptions about the internal structure of an id.
                if (cachedId == null)
                {
                    Hash64 hash = new Hash64();
                    hash = hash.Add(LocalId);

                    for (Test ancestor = parent; ancestor != null; ancestor = ancestor.Parent)
                        hash = hash.Add(ancestor.LocalId);

                    cachedId = hash.ToString();
                }

                return cachedId;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                id = value;
            }
        }

        /// <summary>
        /// Gets or sets a suggested <see cref="LocalId" /> hint, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value returned by this method will be checked for uniqueness and amended as necessary
        /// to produce a truly unique <see cref="LocalId" />.
        /// </para>
        /// </remarks>
        /// <value>
        /// The default value of this property is <c>null</c> which causes the <see cref="TestComponent.Name" />
        /// property to be used as the local id hint.
        /// </value>
        /// <returns>The local id hint.</returns>
        public string LocalIdHint
        {
            get { return localIdHint; }
            set { localIdHint = value; }
        }

        /// <summary>
        /// Gets the full name of the test.  The full name is derived by concatenating the
        /// <see cref="FullName" /> of the <see cref="Parent"/> followed by a slash ('/')
        /// followed by the <see cref="TestComponent.Name" /> of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The full name of the root test is empty.
        /// </para>
        /// </remarks>
        public string FullName
        {
            get
            {
                if (parent == null)
                    return @"";
                if (parent.Parent == null)
                    return Name;
                return String.Concat(parent.FullName, "/", Name);
            }
        }

        /// <summary>
        /// Gets or sets the parent of this test, or null if this is the root test.
        /// </summary>
        public Test Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                cachedId = null;
                cachedLocalId = null;
            }
        }

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
        /// <see cref="TestComponent.CodeElement" /> and an ordering condition among siblings
        /// to guarantee uniqueness.
        /// </para>
        /// <para>
        /// The locally unique <see cref="LocalId" /> property may be used to generate the
        /// globally unique <see cref="TestComponent.Id" /> property of a test by combining
        /// it with the locally unique identifiers of its parents.
        /// </para>
        /// </remarks>
        /// <returns>The locally unique identifier.</returns>
        public string LocalId
        {
            get
            {
                if (cachedLocalId == null)
                {
                    string resolvedLocalIdHint = localIdHint ?? Name;
                    if (parent != null)
                        cachedLocalId = parent.GetUniqueLocalIdForChild(resolvedLocalIdHint);
                    else
                        cachedLocalId = resolvedLocalIdHint;
                }

                return cachedLocalId;
            }
        }

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
        public bool IsTestCase
        {
            get { return isTestCase; }
            set { isTestCase = value; }
        }

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
        public IList<TestParameter> Parameters
        {
            get { return parameters != null ? (IList<TestParameter>) parameters.AsReadOnly() : EmptyArray<TestParameter>.Instance; }
        }

        /// <summary>
        /// Gets a read-only list of the children of this test.
        /// </summary>
        public IList<Test> Children
        {
            get { return children != null ? (IList<Test>) children.AsReadOnly() : EmptyArray<Test>.Instance; }
        }

        /// <summary>
        /// Gets a read-only list of the dependencies of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Some test frameworks may choose to ignore test dependencies or may impose their own dependency schemes.
        /// </para>
        /// </remarks>
        public IList<Test> Dependencies
        {
            get { return dependencies != null ? (IList<Test>) dependencies.AsReadOnly() : EmptyArray<Test>.Instance; }
        }

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
        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        /// <summary>
        /// Clears the list of test parameters.
        /// </summary>
        public void ClearParameters()
        {
            GenericCollectionUtils.ForEach(Parameters, x => x.Owner = null);
            parameters = null;
        }

        /// <summary>
        /// Adds a test parameter and sets its <see cref="TestParameter.Owner" /> property.
        /// </summary>
        /// <param name="parameter">The test parameter to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="parameter"/> is already
        /// owned by some other test.</exception>
        public void AddParameter(TestParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (parameter.Owner != null)
                throw new InvalidOperationException("The test parameter to be added is already owned by another test.");

            parameter.Owner = this;

            if (parameters == null)
                parameters = new List<TestParameter>();
            parameters.Add(parameter);
        }

        /// <summary>
        /// Removes a test parameter and resets its <see cref="TestParameter.Owner" /> property.
        /// </summary>
        /// <param name="parameter">The test parameter to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameter"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="parameter"/> is not owned by this test.</exception>
        public void RemoveParameter(TestParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");
            if (parameter.Owner != this || parameters == null || ! parameters.Contains(parameter))
                throw new InvalidOperationException("The test parameter to be removed is not owned by this test.");

            parameters.Remove(parameter);
            parameter.Owner = null;
        }

        /// <summary>
        /// Clears the list of children.
        /// </summary>
        public void ClearChildren()
        {
            GenericCollectionUtils.ForEach(Children, x => x.Parent = null);
            children = null;
        }

        /// <summary>
        /// Adds a child test and sets its <see cref="Test.Parent" /> property.
        /// </summary>
        /// <param name="test">The test to add as a child.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="test"/> is already
        /// the child of some other test.</exception>
        public void AddChild(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (test.Parent != null)
                throw new InvalidOperationException(Resources.BaseTest_TestAlreadyHasAParent);

            test.Parent = this;

            if (children == null)
                children = new List<Test>();
            children.Add(test);
        }

        /// <summary>
        /// Removes a child test and resets its <see cref="Test.Parent" /> property.
        /// </summary>
        /// <param name="test">The child test to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="test"/> is not a child of this test.</exception>
        public void RemoveChild(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (test.Parent != this || children == null || ! children.Contains(test))
                throw new InvalidOperationException("The test to be removed is not a child of this test.");

            children.Remove(test);
            test.Parent = null;
        }

        /// <summary>
        /// Clears the list of dependencies.
        /// </summary>
        public void ClearDependencies()
        {
            dependencies = null;
        }

        /// <summary>
        /// Adds a test dependency.
        /// </summary>
        /// <param name="test">The test to add as a dependency.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        public void AddDependency(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            if (dependencies == null)
                dependencies = new List<Test>();
            if (! dependencies.Contains(test))
                dependencies.Add(test);
        }

        /// <summary>
        /// Removes a test dependency.
        /// </summary>
        /// <param name="test">The test to remove as a dependency.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        public void RemoveDependency(Test test)
        {
            if (test == null)
                throw new ArgumentNullException("test");

            if (dependencies != null)
                dependencies.Remove(test);
        }

        /// <summary>
        /// Obtains a unique local id for a child of this test.
        /// </summary>
        /// <param name="localIdHint">A suggested id which will be used if no conflicts occur.</param>
        /// <returns>The unique local id to use.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="localIdHint"/> is null.</exception>
        public string GetUniqueLocalIdForChild(string localIdHint)
        {
            if (localIdHint == null)
                throw new ArgumentNullException("localIdHint");

            string candidateLocalId = localIdHint;

            if (assignedChildLocalIds == null)
            {
                assignedChildLocalIds = new HashSet<string>();
            }
            else
            {
                int index = 2;
                while (assignedChildLocalIds.Contains(candidateLocalId))
                {
                    candidateLocalId = localIdHint + index;
                    index += 1;
                }
            }

            assignedChildLocalIds.Add(candidateLocalId);
            return candidateLocalId;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[{0}] {1}", Kind, Name);
        }
    }
}