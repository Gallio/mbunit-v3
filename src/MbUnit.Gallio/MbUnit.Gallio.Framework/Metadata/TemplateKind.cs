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

namespace MbUnit.Framework.Model.Metadata
{
    /// <summary>
    /// Specifies the kind of a test template.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The template kind is not significant to the MbUnit test runner.  Instead
    /// it provides a loose classification that may be used to provide appropriate
    /// decorations when presenting the template in a user interface.
    /// </para>
    /// <para>
    /// If none of the built-in kinds are appropriate, you may use the
    /// <see cref="Custom" /> kind or invent one of your own to present
    /// in the user interface (albeit perhaps without special affordances.)
    /// </para>
    /// </remarks>
    /// <seealso cref="MetadataConstants.TemplateKindKey"/>
    public static class TemplateKind
    {
        /// <summary>
        /// The template is the root of the template tree.
        /// </summary>
        public const string Root = "Root";

        /// <summary>
        /// The template encloses all contributions offered by a given test framework.
        /// </summary>
        public const string Framework = "Framework";

        /// <summary>
        /// The template describes the tests contained in a single test assembly.
        /// </summary>
        public const string Assembly = "Assembly";

        /// <summary>
        /// The template describes a grouping of templates for descriptive purposes.
        /// </summary>
        public const string Group = "Group";

        /// <summary>
        /// The template describes a test suite.
        /// </summary>
        public const string Suite = "Suite";

        /// <summary>
        /// The template describes a test fixture.
        /// </summary>
        public const string Fixture = "Fixture";

        /// <summary>
        /// The template describes a test.
        /// </summary>
        public const string Test = "Test";

        /// <summary>
        /// The template is of some other unspecified kind.
        /// </summary>
        /// <remarks>
        /// If none of the built-in kinds are appropriate, you may use the
        /// <see cref="Custom" /> kind or invent one of your own to present
        /// in the user interface (albeit perhaps without special affordances.)
        /// </remarks>
        public const string Custom = "Custom";
    }
}
