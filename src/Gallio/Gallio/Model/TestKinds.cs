// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Model
{
    /// <summary>
    /// Specifies the kind of a test component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The test kind is ignored by the test runner but it allows tests to be classified
    /// so that a user interface can provide appropriate decorations and other affordances
    /// for any test kinds that it recognizes.
    /// </para>
    /// <para>
    /// If none of the built-in kinds are appropriate, you may use the
    /// <see cref="Custom" /> kind or invent your own kind as you wish.
    /// </para>
    /// </remarks>
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

        /// <summary>
        /// The test is an error placeholder used in place of a test
        /// when an error occurs during test enumeration.
        /// </summary>
        public const string Error = "Error";

        /// <summary>
        /// The test is of some other unspecified kind.
        /// </summary>
        /// <remarks>
        /// If none of the built-in kinds are appropriate, you may use the
        /// <see cref="Custom" /> kind or invent one of your own to present
        /// in the user interface (albeit perhaps without special affordances.)
        /// </remarks>
        public const string Custom = "Custom";
    }
}
