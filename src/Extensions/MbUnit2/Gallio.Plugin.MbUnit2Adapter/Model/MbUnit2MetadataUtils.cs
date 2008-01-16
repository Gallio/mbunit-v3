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

extern alias MbUnit2;

using System;
using Gallio.Model;
using Gallio.Reflection;

using TestFixturePatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestFixturePatternAttribute;
using TestPatternAttribute2 = MbUnit2::MbUnit.Core.Framework.TestPatternAttribute;
using FixtureCategoryAttribute2 = MbUnit2::MbUnit.Framework.FixtureCategoryAttribute;
using TestCategoryAttribute2 = MbUnit2::MbUnit.Framework.TestCategoryAttribute;
using TestImportance2 = MbUnit2::MbUnit.Framework.TestImportance;
using AuthorAttribute2 = MbUnit2::MbUnit.Framework.AuthorAttribute;
using TestsOnAttribute2 = MbUnit2::MbUnit.Framework.TestsOnAttribute;
using ImportanceAttribute2 = MbUnit2::MbUnit.Framework.ImportanceAttribute;
using IgnoreAttribute2 = MbUnit2::MbUnit.Framework.IgnoreAttribute;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Populates metadata from MbUnit v2 attributes.
    /// </summary>
    internal static class MbUnit2MetadataUtils
    {
        public static void PopulateAssemblyMetadata(ITest test, IAssemblyInfo assembly)
        {
            ModelUtils.PopulateMetadataFromAssembly(assembly, test.Metadata);
        }

        public static void PopulateFixtureMetadata(ITest test, ITypeInfo fixtureType)
        {
            foreach (AuthorAttribute2 attrib in AttributeUtils.GetAttributes<AuthorAttribute2>(fixtureType, true))
            {
                if (!String.IsNullOrEmpty(attrib.Name))
                    test.Metadata.Add(MetadataKeys.AuthorName, attrib.Name);
                if (!String.IsNullOrEmpty(attrib.EMail) && attrib.EMail != @"unspecified")
                    test.Metadata.Add(MetadataKeys.AuthorEmail, attrib.EMail);
                if (!String.IsNullOrEmpty(attrib.HomePage) && attrib.HomePage != @"unspecified")
                    test.Metadata.Add(MetadataKeys.AuthorHomepage, attrib.HomePage);
            }

            foreach (FixtureCategoryAttribute2 attrib in AttributeUtils.GetAttributes<FixtureCategoryAttribute2>(fixtureType, true))
            {
                test.Metadata.Add(MetadataKeys.CategoryName, attrib.Category);
            }

            try
            {
                foreach (TestsOnAttribute2 attrib in AttributeUtils.GetAttributes<TestsOnAttribute2>(fixtureType, true))
                {
                    test.Metadata.Add(MetadataKeys.TestsOn, attrib.TestedType.AssemblyQualifiedName);
                }
            }
            catch (CodeElementResolveException)
            {
                // Ignore failures to resolve the tested type.
            }

            foreach (ImportanceAttribute2 attrib in AttributeUtils.GetAttributes<ImportanceAttribute2>(fixtureType, true))
            {
                test.Metadata.Add(MetadataKeys.Importance, attrib.Importance.ToString());
            }

            foreach (TestFixturePatternAttribute2 attrib in AttributeUtils.GetAttributes<TestFixturePatternAttribute2>(fixtureType, true))
            {
                if (!String.IsNullOrEmpty(attrib.Description))
                    test.Metadata.Add(MetadataKeys.Description, attrib.Description);
            }

            PopulateFixtureOrTestMetadata(test, fixtureType);
        }

        public static void PopulateTestMetadata(ITest test, IMemberInfo member)
        {
            foreach (TestPatternAttribute2 attrib in AttributeUtils.GetAttributes<TestPatternAttribute2>(member, true))
            {
                if (!String.IsNullOrEmpty(attrib.Description))
                    test.Metadata.Add(MetadataKeys.Description, attrib.Description);
            }

            PopulateFixtureOrTestMetadata(test, member);
        }

        private static void PopulateFixtureOrTestMetadata(ITest test, ICodeElementInfo codeElement)
        {
            foreach (IgnoreAttribute2 attrib in AttributeUtils.GetAttributes<IgnoreAttribute2>(codeElement, true))
            {
                test.Metadata.Add(MetadataKeys.IgnoreReason, attrib.Description ?? "<unknown>");
            }

            string xmlDocumentation = codeElement.GetXmlDocumentation();
            if (xmlDocumentation != null)
                test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
        }
    }
}
