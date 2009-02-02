// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio;
using Gallio.Model;
using Gallio.Model.Diagnostics;

namespace MbUnit.Framework
{
    /// <summary>
    /// Describes a test suite generated either at test exploration time or at test
    /// execution time by a test factory.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test suite has no timeout by default.  This may be changed
    /// setting the <see cref="TestDefinition.Timeout" /> property.
    /// </para>
    /// <para>
    /// Refer to the examples on the <see cref="Test" /> class for more information.
    /// </para>
    /// </remarks>
    /// <seealso cref="Test"/>
    public class TestSuite : TestDefinition
    {
        private readonly List<Test> children = new List<Test>();

        /// <summary>
        /// Creates a test suite.
        /// </summary>
        /// <param name="name">The test suite name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public TestSuite(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets a mutable list of the children of this test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Add test cases to this collection to include them in the test suite.
        /// </para>
        /// </remarks>
        public IList<Test> Children
        {
            get { return children; }
        }

        /// <summary>
        /// Gets or sets a delegate to run before each test case in the suite, or null if none.
        /// </summary>
        public Action SetUp { get; set; }

        /// <summary>
        /// Gets or sets a delegate to run after each test case in the suite, or null if none.
        /// </summary>
        public Action TearDown { get; set; }

        /// <summary>
        /// Gets or sets a delegate to run before all test cases in the suite, or null if none.
        /// </summary>
        public Action SuiteSetUp { get; set; }

        /// <summary>
        /// Gets or sets a delegate to run after all test cases in the suite, or null if none.
        /// </summary>
        public Action SuiteTearDown { get; set; }

        /// <inheritdoc />
        protected override bool IsTestCase
        {
            get { return false; }
        }

        /// <inheritdoc />
        protected override string Kind
        {
            get { return TestKinds.Suite; }
        }

        /// <inheritdoc />
        protected override IEnumerable<Test> GetChildren()
        {
            return Children;
        }

        /// <inheritdoc />
        [TestEntryPoint]
        protected override void OnSetupSelf()
        {
            if (SuiteSetUp != null)
                SuiteSetUp();
        }

        /// <inheritdoc />
        [TestEntryPoint]
        protected override void OnTearDownSelf()
        {
            if (SuiteTearDown != null)
                SuiteTearDown();
        }

        /// <inheritdoc />
        [TestEntryPoint]
        protected override void OnSetupChild()
        {
            if (SetUp != null)
                SetUp();
        }

        /// <inheritdoc />
        [TestEntryPoint]
        protected override void OnTearDownChild()
        {
            if (TearDown != null)
                TearDown();
        }
    }
}
