// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;
using Gallio.Common.Reflection;

namespace Gallio.Model.Tree
{
    /// <summary>
    /// A test step is a delimited region of a test defined at run-time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each test that is executed consists of at least one step: the "primary" step.
    /// During execution, each test may spawn additional nested steps that may run in
    /// parallel or in series with one another as required.
    /// </para>
    /// <para>
    /// A test step may be used to describe a particular instance of a data driven test.
    /// It can also be used to generate a completely dynamic hierarchy at runtime that
    /// mirrors and extends the static test hierarchy with new information derived from
    /// the actual execution of the test.
    /// </para>
    /// <para>
    /// Each step has its own execution log and test result (pass/fail/inconclusive).
    /// Therefore a multi-step test may possess multiple execution logs and test results.
    /// This is deliberate.  Think of a <see cref="TestStep" /> as being the declarative component
    /// of a test that specifies which test method to invoke and its arguments.  A
    /// <see cref="Tree.Test" /> is the runtime counterpart of the <see cref="Tree.Test" /> that
    /// captures output and control flow information about part or all of the test.
    /// </para>
    /// <para>
    /// A test step also has metadata that can be update at run-time to carry additional
    /// declarative information about the step.
    /// </para>
    /// </remarks>
    public class TestStep : TestComponent
    {
        private readonly TestStep parent;
        private readonly Test test;
        private readonly bool isPrimary;
        private string id;
        private bool isDynamic;
        private bool isTestCase;

        /// <summary>
        /// Creates a primary step using the same name, code element and metadata
        /// as the test to which it belongs.
        /// </summary>
        /// <param name="test">The test to which the step belongs.</param>
        /// <param name="parent">The parent test step, or null if creating the root step.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="test"/> is null.</exception>
        public TestStep(Test test, TestStep parent)
            : this(test, parent, test.Name, test.CodeElement, true)
        {
        }

        /// <summary>
        /// Creates a step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <paramref name="isPrimary"/> is true, then all metadata from the <paramref name="test"/>
        /// is copied to the step.  Otherwise the new step will have no metadata initially.
        /// </para>
        /// </remarks>
        /// <param name="test">The test to which the step belongs.</param>
        /// <param name="parent">The parent step, or null if creating a root step.</param>
        /// <param name="name">The step name.</param>
        /// <param name="codeElement">The point of definition of the step, or null if unknown.</param>
        /// <param name="isPrimary">True if the test step is primary.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="test"/> is null.</exception>
        public TestStep(Test test, TestStep parent, string name, ICodeElementInfo codeElement, bool isPrimary)
            : base(name, codeElement)
        {
            if (test == null)
                throw new ArgumentNullException("test");
            if (name == null)
                throw new ArgumentNullException("name");

            this.test = test;
            this.parent = parent;
            this.isPrimary = isPrimary;

            isTestCase = test.IsTestCase;

            if (isPrimary)
                Metadata.AddAll(test.Metadata);
        }

        /// <inheritdoc />
        public override string Id
        {
            get
            {
                if (id == null)
                    id = Hash64.CreateUniqueHash().ToString();
                return id;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                id = value;
            }
        }

        /// <summary>
        /// Gets the full name of the step.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The full name is derived by concatenating the <see cref="FullName" /> of the
        /// <see cref="Parent"/> followed by a slash ('/') followed by the <see cref="TestComponent.Name" />
        /// of this test step.
        /// </para>
        /// <para>
        /// The full name of the root test step is empty.
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item><strong>""</strong> : The root step.</item>
        /// <item><strong>"SomeAssembly/SomeFixture/SomeTest"</strong> : The step corresponding to SomeTest.</item>
        /// <item><strong>"SomeAssembly/SomeFixture/SomeTest/ChildStep"</strong> : A child step of SomeTest.</item>
        /// </list>
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
        /// Gets the step that contains this one, or null if this instance represents the
        /// root step of the test step tree.
        /// </summary>
        public TestStep Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Gets the test to which this step belongs.
        /// </summary>
        public Test Test
        {
            get { return test; }
        }

        /// <summary>
        /// Returns true if the test step is the top node of a hierarchy of test steps that are
        /// all associated with the same test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// In the case where a single test step is associated with a test, then it is the primary test step.
        /// There may be multiple primary test steps of the same test if they are not
        /// directly related by ancestry (ie. one does not contain the other).
        /// </para>
        /// <para>
        /// A non-primary test step is known as a derived test step.
        /// </para>
        /// <para>
        /// For example, suppose we have a data-driven test fixture F which contains a data-driven
        /// test T.  There will be a primary test step of F that represents the undifferentiated
        /// fixture with no data bound to it.  Its immediate children will be derived test steps of F with particular data bindings.
        /// Similarly, each of these derived test steps of F will contain a primary test step of T
        /// which then contains derived test steps of T.
        /// </para>
        /// <para>
        /// This case is illustrated like this where periods delimit path segments in the test step hierarchy.
        /// <list type="bullet">
        /// <item><strong>F</strong> : Primary test step of F</item>
        /// <item><strong>F.F1</strong> : Non-primary test step of F with data bindings</item>
        /// <item><strong>F.F1.T</strong> : Primary test step of T</item>
        /// <item><strong>F.F1.T.T1</strong> : Non-primary test step of T with data bindings</item>
        /// <item><strong>F.F1.T.T2</strong> : Non-primary test step of T with data bindings</item>
        /// <item><strong>F.F2</strong> : Non-primary test step of F with data bindings</item>
        /// <item><strong>F.F2.T</strong> : Primary test step of T</item>
        /// <item><strong>F.F2.T.T1</strong> : Non-primary test step of T with data bindings</item>
        /// <item><strong>F.F2.T.T2</strong> : Non-primary test step of T with data bindings</item>
        /// </list>
        /// </para>
        /// </remarks>
        public bool IsPrimary
        {
            get { return isPrimary; }
        }

        /// <summary>
        /// Gets or sets whether the test step is dynamic.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A dynamic test step is one whose execution is governed by parameters that are not 
        /// known a priori.  
        /// </para>
        /// <para>
        /// For example, a primary test step is generally not dynamic because its existence
        /// usually only depends on statically known information derived from the test model.
        /// However, a child test step created at runtime by a data binding process might be
        /// considered dynamic if the data items that were used may change between test runs
        /// even when the static structure of the test code remains the same.  So a test step
        /// that uses random data items or that owes its existence to processes that are not
        /// entirely under the control of the test framework should be flagged as dynamic.
        /// </para>
        /// <para>
        /// It can be useful to distinguish between static and dynamic test steps when
        /// correlating results across test runs.  Dynamic test steps are more likely to 
        /// change in dramatic ways between test runs than static test steps.
        /// </para>
        /// </remarks>
        public bool IsDynamic
        {
            get { return isDynamic; }
            set { isDynamic = value; }
        }

        /// <summary>
        /// Gets or sets whether the test step represents a distinct test case.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A test case is typically a leaf of the test step hierarchy.
        /// </para>
        /// <para>
        /// A test step can only be a test case if its associated <see cref="Tree.Test" /> is a test case. However,
        /// not all test steps of tests that are test cases will themselves be test cases.  For example, a
        /// data-driven test might have one primary test step with multiple derived test steps as children.
        /// The primary test step will generally not be considered a test case but its children will.
        /// </para>
        /// </remarks>
        /// <seealso cref="Tree.Test.IsTestCase"/>
        public bool IsTestCase
        {
            get { return isTestCase; }
            set { isTestCase = value; }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return String.Format("[Step] {0}", Name);
        }
    }
}