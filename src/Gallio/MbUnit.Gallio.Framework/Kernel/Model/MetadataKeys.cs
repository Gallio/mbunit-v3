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
    /// Provides common metadata keys.
    /// </summary>
    public static class MetadataKeys
    {
        /// <summary>
        /// The metadata key for the author's email.
        /// The associated value should be the author's email as a string.
        /// </summary>
        public const string AuthorEmail = "AuthorEmail";

        /// <summary>
        /// The metadata key for the author's name.
        /// The associated value should be the author's name as a string.
        /// </summary>
        public const string AuthorName = "AuthorName";

        /// <summary>
        /// The metadata key for the author's homepage.
        /// The associated value should be the author's homepage as a string.
        /// </summary>
        public const string AuthorHomepage = "AuthorHomepage";

        /// <summary>
        /// The metadata key for the name of a category to which a test belongs.
        /// The associated value should be the category name as a string.
        /// </summary>
        public const string CategoryName = "CategoryName";

        /// <summary>
        /// The metadata key used to describe the kind of a component.
        /// The associated value should be one of the <see cref="Model.ComponentKind" /> string constants.
        /// </summary>
        public const string ComponentKind = "ComponentKind";

        /// <summary>
        /// The metadata key for the description of a test component.
        /// The associated value should be the description as a string.
        /// </summary>
        public const string Description = "Description";

        /// <summary>
        /// The metadata key for the ignore reason of a test component.
        /// The associated value should describe the reason the test is being ignored.
        /// (Tests may of course be ignored without a declared reason.)
        /// </summary>
        public const string IgnoreReason = "IgnoreReason";

        /// <summary>
        /// The metadata key for the importance of a test component.
        /// The associated value should be one of those from <see cref="TestImportance" /> represented as a string.
        /// </summary>
        public const string Importance = "Importance";

        /// <summary>
        /// The metadata key for the name of the type being tested.
        /// The associated value should be the full name of the type from <see cref="Type.FullName" />
        /// or the assembly qualified name of the type from <see cref="Type.AssemblyQualifiedName" />.
        /// </summary>
        public const string TestsOn = "TestsOn";

        /// <summary>
        /// The metadata key for the XML documentation of the test.
        /// The associated value should be the contents of the XML documentation contents
        /// associated with the test class or method.
        /// </summary>
        public const string XmlDocumentation = "XmlDocumentation";
    }
}
