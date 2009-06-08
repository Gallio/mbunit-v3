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

namespace Gallio.Model
{
    /// <summary>
    /// A list of standard test kind names provided by Gallio.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test kind has no effect on the semantics of the test runner but it allows tests to
    /// be classified so that a user interface can provide appropriate icons, descriptions,
    /// decorations and affordances for registered test kinds.
    /// </para>
    /// <para>
    /// To create your own custom test kinds for your test framework, register a
    /// <see cref="ITestKind" /> component in your plugin metadata with a unique name.
    /// </para>
    /// </remarks>
    /// <seealso cref="ITestKind"/>
    /// <seealso cref="TestKindTraits"/>
    /// <seealso cref="MetadataKeys.TestKind"/>
    public static class TestKinds
    {
        /// <summary>
        /// The test represents the root of the test tree.
        /// </summary>
        public const string Root = "Root";

        /// <summary>
        /// The test represents a grouping of all contributions offered by a given test framework.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A framework test should also have associated <see cref="MetadataKeys.Framework" /> metadata.
        /// </para>
        /// </remarks>
        public const string Framework = "Framework";

        /// <summary>
        /// The test represents the tests contained in a single test assembly.
        /// </summary>
        public const string Assembly = "Assembly";

        /// <summary>
        /// The test represents the tests contained in a single test namespace.
        /// </summary>
        public const string Namespace = "Namespace";

        /// <summary>
        /// The test represents a grouping of tests for descriptive purposes.
        /// </summary>
        public const string Group = "Group";

        /// <summary>
        /// The test represents a test suite.
        /// </summary>
        public const string Suite = "Suite";

        /// <summary>
        /// The test represents a test fixture.
        /// </summary>
        public const string Fixture = "Fixture";

        /// <summary>
        /// The test represents a test case.
        /// </summary>
        public const string Test = "Test";
    }
}
