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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Gallio.Common;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test builder applies contributions to a test under construction.
    /// </summary>
    public interface ITestBuilder : ITestComponentBuilder
    {
        /// <summary>
        /// Gets or sets the value of the <see cref="MetadataKeys.TestKind" />
        /// metadata entry.  (This is a convenience method.)
        /// </summary>
        /// <value>
        /// One of the <see cref="TestKinds" /> constants.
        /// </value>
        string Kind { get; set; }

        /// <summary>
        /// Gets or sets the apartment state to be used to run the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the apartment state is <see cref="System.Threading.ApartmentState.Unknown" />
        /// the test will inherit the apartment state of its parent.  Otherwise
        /// it will run in a thread with the specified apartment state.
        /// </para>
        /// <para>
        /// The test runner guarantees that the root test runs with the <see cref="System.Threading.ApartmentState.STA" />
        /// apartment state.  Consequently the apartment state only needs to be overridden to run 
        /// a test in some mode that may differ from that which it would ordinarily inherit.
        /// </para>
        /// </remarks>
        /// <value>
        /// The default value of this property is <see cref="System.Threading.ApartmentState.Unknown" />.
        /// </value>
        ApartmentState ApartmentState { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of time the whole test including
        /// its setup, teardown and body should be permitted to run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the test runs any longer than this, it will be aborted by the framework.
        /// The timeout may be null to indicate the absence of a timeout.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/>
        /// represents a negative time span.</exception>
        /// <value>The timeout.  Default value is null.</value>
        Func<TimeSpan?> TimeoutFunc { get; set; }

        /// <summary>
        /// Gets whether this test represents an individual test case
        /// as opposed to a test container such as a fixture or suite.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this property can be used by the test harness to avoid processing containers
        /// that have no test cases.  It can also be used by the reporting infrastructure
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
        bool IsTestCase { get; set; }

        /// <summary>
        /// Gets or sets whether the test is parallelizable.
        /// </summary>
        /// <value>
        /// True if the test is parallelizable.  The default value of this property is <c>false</c>.
        /// </value>
        bool IsParallelizable { get; set; }

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
        /// Gets a locally unique identifier for this test that satisfies some specific conditions.
        /// </summary>
        /// <remarks>
        /// The local identifier must satisfy the following conditions:
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
        string LocalId { get; }

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
        /// The default value of this property is <c>null</c> which causes the <see cref="ITestComponentBuilder.Name" />
        /// property to be used as the local id hint.
        /// </value>
        /// <returns>The local id hint.</returns>
        string LocalIdHint { get; set; }

        /// <summary>
        /// Gets the set of actions that describe the behavior of the test.
        /// </summary>
        PatternTestActions TestActions { get; }

        /// <summary>
        /// Gets the set of actions that describe the behavior of the test's instances.
        /// </summary>
        PatternTestInstanceActions TestInstanceActions { get; }

        /// <summary>
        /// Creates a child test and returns its builder.
        /// </summary>
        /// <param name="name">The test name.</param>
        /// <param name="codeElement">The associated code element, or null if none.</param>
        /// <param name="dataContextBuilder">The data context builder for the new test.</param>
        /// <returns>The builder for the child test.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="dataContextBuilder"/> is null.</exception>
        ITestBuilder CreateChild(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder);

        /// <summary>
        /// Creates a test parameter and returns its builder.
        /// </summary>
        /// <param name="name">The test parameter name.</param>
        /// <param name="codeElement">The associated code element, or null if none.</param>
        /// <param name="dataContextBuilder">The data context builder for the new test parameter.</param>
        /// <returns>The builder for the test parameter.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="dataContextBuilder"/> is null.</exception>
        ITestParameterBuilder CreateParameter(string name, ICodeElementInfo codeElement, ITestDataContextBuilder dataContextBuilder);

        /// <summary>
        /// Gets a test parameter builder by name.
        /// </summary>
        /// <param name="name">The test parameter name.</param>
        /// <returns>The builder for the test parameter, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null.</exception>
        ITestParameterBuilder GetParameter(string name);

        /// <summary>
        /// Adds a test dependency.
        /// </summary>
        /// <param name="testDependency">The test to add as a dependency.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testDependency"/> is null.</exception>
        void AddDependency(Test testDependency);

        /// <summary>
        /// Gets the underlying test.
        /// </summary>
        /// <returns>The underlying test.</returns>
        PatternTest ToTest();
    }
}
