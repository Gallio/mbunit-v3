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
using System.Reflection;
using System.Text;

namespace Gallio.Model
{
    /// <summary>
    /// Provides common metadata keys.
    /// </summary>
    public static class MetadataKeys
    {
        /// <summary>
        /// The metadata key for the author's email.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>AuthorAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string AuthorEmail = "AuthorEmail";

        /// <summary>
        /// The metadata key for the author's name.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>AuthorAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string AuthorName = "AuthorName";

        /// <summary>
        /// The metadata key for the author's homepage.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>AuthorAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string AuthorHomepage = "AuthorHomepage";

        /// <summary>
        /// The metadata key for the name of a category to which a test belongs.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>CategoryAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string CategoryName = "CategoryName";

        /// <summary>
        /// The matadata key for the location of an assembly as a local
        /// file path or as a Uri.
        /// </summary>
        /// <remarks>
        /// May be derived from the assembly's <see cref="Assembly.CodeBase" /> property.
        /// </remarks>
        public const string CodeBase = "CodeBase";

        /// <summary>
        /// The metadata key for the name of the company associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyCompanyAttribute" />.
        /// </remarks>
        public const string Company = "Company";

        /// <summary>
        /// The metadata key for build/release configuration information associated with a test.
        /// It describes the target environment or usage for the test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyConfigurationAttribute" />.
        /// </remarks>
        public const string Configuration = "Configuration";

        /// <summary>
        /// The metadata key for a copyright associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyCopyrightAttribute" />.
        /// </remarks>
        public const string Copyright = "Copyright";

        /// <summary>
        /// The metadata key for the description of a test component.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyDescriptionAttribute" />
        /// or <c>DescriptionAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string Description = "Description";

        /// <summary>
        /// The metadata key for the expected exception type.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>ExpectedExceptionAttribute</c> in MbUnit
        /// or its equivalent.
        /// </remarks>
        public const string ExpectedException = "ExpectedException";

        /// <summary>
        /// The metadata key for a file version number associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyFileVersionAttribute" />.
        /// </remarks>
        public const string FileVersion = "FileVersion";

        /// <summary>
        /// The metadata key that describes the reason that a test is being ignored.
        /// (Tests may of course be ignored without a declared reason.)
        /// </summary>
        /// <remarks>
        /// May be derived from <c>IgnoreAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string IgnoreReason = "IgnoreReason";

        /// <summary>
        /// The metadata key for the importance of a test component as the
        /// string representation of one of the <c>TestImportance</c> constants in MbUnit
        /// or its equivalent.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>ImportantAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string Importance = "Importance";

        /// <summary>
        /// The metadata key for an informational version number associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyInformationalVersionAttribute" />.
        /// </remarks>
        public const string InformationalVersion = "InformationalVersion";

        /// <summary>
        /// The metadata key for a product associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyProductAttribute" />.
        /// </remarks>
        public const string Product = "Product";

        /// <summary>
        /// The metadata key for the name of the type being tested.
        /// The associated value should be the full name of the type from <see cref="Type.FullName" />
        /// or the assembly qualified name of the type from <see cref="Type.AssemblyQualifiedName" />.
        /// </summary>
        /// <remarks>
        /// May be derived from <c>TestsOnAttribute</c> in MbUnit or its equivalent.
        /// </remarks>
        public const string TestsOn = "TestsOn";

        /// <summary>
        /// The metadata key used to describe the kind of a test as the
        /// string representation of one of the <see cref="TestKinds" /> constants.
        /// </summary>
        public const string TestKind = "TestKind";

        /// <summary>
        /// The metadata key for a title associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyTitleAttribute" />.
        /// </remarks>
        public const string Title = "Title";

        /// <summary>
        /// The metadata key for a trademark associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyTrademarkAttribute" />.
        /// </remarks>
        public const string Trademark = "Trademark";

        /// <summary>
        /// The metadata key for a version number associated with a test.
        /// </summary>
        /// <remarks>
        /// May be derived from <see cref="AssemblyVersionAttribute" />.
        /// </remarks>
        public const string Version = "Version";

        /// <summary>
        /// The metadata key for the XML documentation of the test derived from
        /// XML code documentation comments.
        /// </summary>
        public const string XmlDocumentation = "XmlDocumentation";
    }
}
