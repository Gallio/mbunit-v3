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

namespace MbUnit.Framework.Kernel.Metadata
{
    /// <summary>
    /// Provides common metadata constants.
    /// </summary>
    public static class MetadataConstants
    {
        /// <summary>
        /// The metadata key for the author's email.
        /// The associated value should be the author's email as a string.
        /// </summary>
        public const string AuthorEmailKey = "AuthorEmail";

        /// <summary>
        /// The metadata key for the author's name.
        /// The associated value should be the author's name as a string.
        /// </summary>
        public const string AuthorNameKey = "AuthorName";

        /// <summary>
        /// The metadata key for the name of a category to which a test belongs.
        /// The associated value should be the category name as a string.
        /// </summary>
        public const string CategoryNameKey = "CategoryName";

        /// <summary>
        /// The metadata key for the description of a test component.
        /// The associated value should be the description as a string.
        /// </summary>
        public const string DescriptionKey = "Description";

        /// <summary>
        /// The metadata key used to describe the kind of a component.
        /// The associated value should be one of the <see cref="ComponentKind" /> string constants.
        /// </summary>
        public const string ComponentKindKey = "ComponentKind";

        /// <summary>
        /// The metadata key for the name of the type being tested.
        /// The associated value should be the fully qualified name of the type as a string.
        /// </summary>
        public const string TestsOnKey = "TestsOn";
    }
}
