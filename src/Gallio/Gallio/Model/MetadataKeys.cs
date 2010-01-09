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
using System.Reflection;

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
        /// <para>
        /// May be derived from <c>AuthorAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string AuthorEmail = "AuthorEmail";

        /// <summary>
        /// The metadata key for the author's name.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <c>AuthorAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string AuthorName = "AuthorName";

        /// <summary>
        /// The metadata key for the author's homepage.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <c>AuthorAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string AuthorHomepage = "AuthorHomepage";

        /// <summary>
        /// The metadata key for the name of a category to which a test belongs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <c>CategoryAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string Category = "Category";

        /// <summary>
        /// The matadata key for the location of an assembly as a local
        /// file path or as a Uri.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from the assembly's <see cref="Assembly.CodeBase" /> property.
        /// </para>
        /// </remarks>
        public const string CodeBase = "CodeBase";

        /// <summary>
        /// The metadata key for the name of the company associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyCompanyAttribute" />.
        /// </para>
        /// </remarks>
        public const string Company = "Company";

        /// <summary>
        /// The metadata key for build/release configuration information associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It describes the target environment or usage for the test.
        /// </para>
        /// <para>
        /// May be derived from <see cref="AssemblyConfigurationAttribute" />.
        /// </para>
        /// </remarks>
        public const string Configuration = "Configuration";

        /// <summary>
        /// The metadata key for a copyright associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyCopyrightAttribute" />.
        /// </para>
        /// </remarks>
        public const string Copyright = "Copyright";

        /// <summary>
        /// The metadata key for specifying the origin of data used by a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be the file name and line number of a record in a file, the ID
        /// of a database record or some other identifying characteristic.
        /// </para>
        /// </remarks>
        public const string DataLocation = "DataLocation";

        /// <summary>
        /// The metadata key for the description of a test component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyDescriptionAttribute" />
        /// or <c>DescriptionAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string Description = "Description";

        /// <summary>
        /// The metadata key for the expected exception type which should be the name,
        /// full name or assembly-qualified name of the expected exception type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <c>ExpectedExceptionAttribute</c> in MbUnit
        /// or its equivalent.
        /// </para>
        /// </remarks>
        public const string ExpectedException = "ExpectedException";

        /// <summary>
        /// The metadata key for the expected exception message.
        /// May be a substring of the actual exception message.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <c>ExpectedExceptionAttribute</c> in MbUnit
        /// or its equivalent.
        /// </para>
        /// </remarks>
        public const string ExpectedExceptionMessage = "ExpectedExceptionMessage";

        /// <summary>
        /// The metadata key that describes the reason that a test should be run explicitly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Tests may of course be marked explicit without a declared reason.
        /// </para>
        /// <para>
        /// May be derived from <c>ExplicitAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string ExplicitReason = "ExplicitReason";

        /// <summary>
        /// The metadata key for a file associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The file key should be associated with the top test produced by a file that
        /// the test framework was asked to explore.  For example, if the test package
        /// included "C:\Tests\MyTests.dll" then the assembly-level test emitted by the framework
        /// should have a File metadata entry with the value "C:\Tests\MyTests.dll".
        /// </para>
        /// </remarks>
        public const string File = "File";

        /// <summary>
        /// The metadata key for a file version number associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyFileVersionAttribute" />.
        /// </para>
        /// </remarks>
        public const string FileVersion = "FileVersion";

        /// <summary>
        /// The metadata key for the test framework name associated with a test assembly in the test tree.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value should be the descriptive name of the framework including its version.  eg. "MbUnit v3.1 build 200"
        /// </para>
        /// <para>
        /// A single assembly may have multiple associated frameworks.
        /// </para>
        /// </remarks>
        public const string Framework = "Framework";

        /// <summary>
        /// The metadata key that describes the reason that a test is being ignored.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Tests may of course be ignored without a declared reason.
        /// </para>
        /// <para>
        /// May be derived from <c>IgnoreAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string IgnoreReason = "IgnoreReason";

        /// <summary>
        /// The metadata key for the importance of a test component as the
        /// string representation of one of the <c>TestImportance</c> constants in MbUnit
        /// or its equivalent.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <c>ImportantAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string Importance = "Importance";

        /// <summary>
        /// The metadata key for an informational version number associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyInformationalVersionAttribute" />.
        /// </para>
        /// </remarks>
        public const string InformationalVersion = "InformationalVersion";

        /// <summary>
        /// The metadata key that describes the reason that a test is pending.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Tests may of course be marked pending without a declared reason.
        /// </para>
        /// <para>
        /// May be derived from <c>PendingAttribute</c> in MbUnit or its equivalent.
        /// </para>
        /// </remarks>
        public const string PendingReason = "PendingReason";

        /// <summary>
        /// The metadata key for a product associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyProductAttribute" />.
        /// </para>
        /// </remarks>
        public const string Product = "Product";

        /// <summary>
        /// The metadata key for the name of the type being tested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The associated value should be the full name of the type from <see cref="Type.FullName" />
        /// or the assembly qualified name of the type from <see cref="Type.AssemblyQualifiedName" />.
        /// </para>
        /// <para>
        /// May be derived from <c>TestsOnAttribute</c> in MbUnit or its equivalent.
        /// </para>
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
        /// <para>
        /// May be derived from <see cref="AssemblyTitleAttribute" />.
        /// </para>
        /// </remarks>
        public const string Title = "Title";

        /// <summary>
        /// The metadata key for a trademark associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyTrademarkAttribute" />.
        /// </para>
        /// </remarks>
        public const string Trademark = "Trademark";

        /// <summary>
        /// The metadata key for a version number associated with a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// May be derived from <see cref="AssemblyVersionAttribute" />.
        /// </para>
        /// </remarks>
        public const string Version = "Version";

        /// <summary>
        /// The metadata key for the XML documentation of the test derived from
        /// XML code documentation comments.
        /// </summary>
        public const string XmlDocumentation = "XmlDocumentation";
    }
}
