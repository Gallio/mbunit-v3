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

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// Specifies the kind of a test or template component.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The component kind is not significant to the MbUnit test runner.  Instead
    /// it provides a loose classification that may be used to provide appropriate
    /// decorations when presenting the template in a user interface.
    /// </para>
    /// <para>
    /// If none of the built-in kinds are appropriate, you may use the
    /// <see cref="Custom" /> kind or invent one of your own to present
    /// in the user interface (albeit perhaps without special affordances.)
    /// </para>
    /// </remarks>
    /// <seealso cref="MetadataKeys.ComponentKind"/>
    public static class ComponentKind
    {
        /// <summary>
        /// The component is the root of the test or template tree.
        /// </summary>
        public const string Root = "Root";

        /// <summary>
        /// The component encloses all contributions offered by a given test framework.
        /// </summary>
        public const string Framework = "Framework";

        /// <summary>
        /// The component describes the tests contained in a single test assembly.
        /// </summary>
        public const string Assembly = "Assembly";

        /// <summary>
        /// The component describes the tests contained in a single test namespace.
        /// </summary>
        public const string Namespace = "Namespace";

        /// <summary>
        /// The component describes a grouping of templates or tests for descriptive purposes.
        /// </summary>
        public const string Group = "Group";

        /// <summary>
        /// The component describes a test suite.
        /// </summary>
        public const string Suite = "Suite";

        /// <summary>
        /// The component describes a test fixture.
        /// </summary>
        public const string Fixture = "Fixture";

        /// <summary>
        /// The component describes a test.
        /// </summary>
        public const string Test = "Test";

        /// <summary>
        /// The component is of some other unspecified kind.
        /// </summary>
        /// <remarks>
        /// If none of the built-in kinds are appropriate, you may use the
        /// <see cref="Custom" /> kind or invent one of your own to present
        /// in the user interface (albeit perhaps without special affordances.)
        /// </remarks>
        public const string Custom = "Custom";
    }
}
