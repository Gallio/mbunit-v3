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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Model;
using Gallio.Model.Tree;
using Gallio.NUnitAdapter.Properties;

namespace Gallio.NUnitAdapter.Model
{
    internal abstract class NUnitTestExplorerEngine
    {
        private const string NUnitFrameworkAssemblyDisplayName = @"nunit.framework";

        public abstract void ExploreAssembly(bool skipChildren);

        public virtual void ExploreType(ITypeInfo type)
        {
        }

        public virtual void Finish()
        {
        }

        protected void PopulateAssemblyTestMetadata(Test assemblyTest, IAssemblyInfo assembly)
        {
            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            Version frameworkVersion = GetFrameworkVersion(assembly);

            string frameworkName = String.Format(Resources.NUnitTestExplorer_FrameworkNameWithVersionFormat, frameworkVersion);
            assemblyTest.Metadata.SetValue(MetadataKeys.Framework, frameworkName);
            assemblyTest.Metadata.SetValue(MetadataKeys.File, assembly.Path);
            assemblyTest.Kind = NUnitTestExplorer.AssemblyKind;
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, NUnitFrameworkAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private static void BuildTest(IAssemblyInfo assembly, Test parentTest, NUnit.Core.ITest nunitTest)
        {
            string kind;
            ICodeElementInfo codeElement;

            switch (nunitTest.TestType)
            {
#if NUNIT248
                case @"Test Case":
#elif NUNIT253
                case @"NUnitTestMethod":
#elif NUNITLATEST
                case @"TestMethod":
#else
#error "Unrecognized NUnit framework version."
#endif                   
                    kind = TestKinds.Test;
                    codeElement = ParseTestCaseName(assembly, nunitTest.TestName.FullName);
                    break;

#if NUNIT248
                case @"Test Fixture":
#elif NUNIT253
                case @"NUnitTestFixture":
#elif NUNITLATEST
                case @"TestFixture":
#else
#error "Unrecognized NUnit framework version."
#endif                    
                    kind = TestKinds.Fixture;
                    codeElement = ParseTestFixtureName(assembly, nunitTest.TestName.FullName);
                    break;

#if NUNITLATEST
                case @"ParameterizedTest":
                    kind = TestKinds.Suite;
                    codeElement = ParseTestCaseName(assembly, nunitTest.TestName.FullName);
                    break;
#endif

                default:
                    kind = nunitTest.IsSuite ? TestKinds.Suite : TestKinds.Test;
                    codeElement = parentTest.CodeElement;
                    break;
            }

            // Build the test.
            NUnitTest test = new NUnitTest(nunitTest.TestName.Name, codeElement, nunitTest);
            test.Kind = kind;
            test.IsTestCase = !nunitTest.IsSuite;

            PopulateMetadata(test);

            parentTest.AddChild(test);
            BuildTestChildren(assembly, test, nunitTest);
        }

        protected static void BuildTestChildren(IAssemblyInfo assembly, NUnitTest parentTest, NUnit.Core.ITest parentNUnitTest)
        {
            if (parentNUnitTest.Tests != null)
            {
                foreach (NUnit.Core.ITest childNUnitTest in parentNUnitTest.Tests)
                    BuildTest(assembly, parentTest, childNUnitTest);
            }
        }

        protected static void PopulateMetadata(NUnitTest test)
        {
            NUnit.Core.ITest nunitTest = test.Test;

            if (!String.IsNullOrEmpty(nunitTest.Description))
                test.Metadata.Add(MetadataKeys.Description, nunitTest.Description);

            if (!String.IsNullOrEmpty(nunitTest.IgnoreReason))
                test.Metadata.Add(MetadataKeys.IgnoreReason, nunitTest.IgnoreReason);

            if (nunitTest.Categories != null)
            {
                foreach (string category in nunitTest.Categories)
                    if (! string.IsNullOrEmpty(category))
                        test.Metadata.Add(MetadataKeys.Category, category);
            }

            if (nunitTest.Properties != null)
            {
                foreach (DictionaryEntry entry in nunitTest.Properties)
                {
                    if (entry.Key != null && entry.Value != null)
                    {
                        string keyString = entry.Key.ToString();
                        if (!keyString.StartsWith("_") && keyString.Length != 0)
                        {
                            ICollection values = entry.Value as ICollection;
                            if (values != null)
                            {
                                foreach (object value in values)
                                    if (value != null)
                                        test.Metadata.Add(keyString, value.ToString());
                            }
                            else
                            {
                                test.Metadata.Add(keyString, entry.Value.ToString());
                            }
                        }
                    }
                }
            }

            ICodeElementInfo codeElement = test.CodeElement;
            if (codeElement != null)
            {
                // Add documentation.
                string xmlDocumentation = codeElement.GetXmlDocumentation();
                if (xmlDocumentation != null)
                    test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
            }
        }

        /// <summary>
        /// Parses a code element from an NUnit test case name.
        /// The name generally consists of the fixture type full-name followed by
        /// a dot and the test method name.
        /// </summary>
        private static ICodeElementInfo ParseTestCaseName(IAssemblyInfo assembly, string name)
        {
            if (assembly != null)
            {
                // Handle row-test naming scheme.
                int firstParen = name.IndexOf('(');
                if (firstParen >= 0)
                    name = name.Substring(0, firstParen);

                // Parse the identifier.
                if (IsProbableIdentifier(name))
                {
                    int lastDot = name.LastIndexOf('.');
                    if (lastDot > 0 && lastDot < name.Length - 1)
                    {
                        string typeName = name.Substring(0, lastDot);
                        string methodName = name.Substring(lastDot + 1);

                        ITypeInfo type = assembly.GetType(typeName);
                        if (type != null)
                        {
                            try
                            {
                                return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                            }
                            catch (AmbiguousMatchException)
                            {
                                // We may have insufficient information to distinguish overloaded
                                // test methods.  In this case we give up trying to find the code element.
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a code reference from an NUnit test fixture name.
        /// The name generally consists of the fixture type full-name.
        /// </summary>
        private static ICodeElementInfo ParseTestFixtureName(IAssemblyInfo assembly, string name)
        {
            if (assembly != null)
            {
                if (IsProbableIdentifier(name))
                {
                    return assembly.GetType(name);
                }
            }

            return null;
        }

        private static bool IsProbableIdentifier(string name)
        {
            return name.Length != 0
                && !name.Contains(@" ")
                && !name.StartsWith(@".")
                && !name.EndsWith(@".");
        }
    }
}
