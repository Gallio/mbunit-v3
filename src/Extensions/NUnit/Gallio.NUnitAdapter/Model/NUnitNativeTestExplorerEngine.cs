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

using System;
using System.Collections;
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;
using Gallio.NUnitAdapter.Properties;
using NUnitCoreExtensions = NUnit.Core.CoreExtensions;
using NUnitTestRunner = NUnit.Core.TestRunner;
using NUnitTestPackage = NUnit.Core.TestPackage;
using NUnitRemoteTestRunner = NUnit.Core.RemoteTestRunner;
using NUnitITest = NUnit.Core.ITest;

namespace Gallio.NUnitAdapter.Model
{
    /// <summary>
    /// Explores tests in NUnit assemblies using the native NUnit test enumeration mechanism.
    /// </summary>
    internal class NUnitNativeTestExplorerEngine : NUnitTestExplorerEngine
    {
        private readonly TestModel testModel;
        private readonly Assembly assembly;

        private NUnitAssemblyTest assemblyTest;

        public NUnitNativeTestExplorerEngine(TestModel testModel, Assembly assembly)
        {
            this.testModel = testModel;
            this.assembly = assembly;
        }

        public override Test GetAssemblyTest()
        {
            return assemblyTest;
        }

        public override void ExploreAssembly(bool skipChildren)
        {
            if (assemblyTest == null)
            {
                assemblyTest = BuildAssemblyTest(testModel.RootTest);
            }
        }

        private NUnitAssemblyTest BuildAssemblyTest(Test parent)
        {
            IAssemblyInfo assemblyInfo = Reflector.Wrap(assembly);
            NUnitTestRunner runner = CreateTestRunner(assembly.Location);

            NUnitAssemblyTest assemblyTest = new NUnitAssemblyTest(assemblyInfo, runner);
            PopulateMetadata(assemblyTest);
            PopulateAssemblyTestMetadata(assemblyTest, assemblyInfo);

            foreach (NUnitITest assemblyTestSuite in runner.Test.Tests)
                BuildTestChildren(assemblyInfo, assemblyTest, assemblyTestSuite);

            parent.AddChild(assemblyTest);
            return assemblyTest;
        }

        private static NUnitTestRunner CreateTestRunner(string assemblyLocation)
        {
            NUnitTestPackage package = new NUnitTestPackage(@"Tests");

            // The SetupFixture feature requires namespace suites even though we
            // would prefer to turn this off since the namespaces are mainly a presentation concern.
            package.Settings.Add(@"AutoNamespaceSuites", true);
            package.Assemblies.Add(assemblyLocation);

            NUnitTestRunner runner = new NUnitRemoteTestRunner();
            if (!runner.Load(package))
                throw new ModelException(Resources.NUnitTestExplorer_CannotLoadNUnitTestAssemblies);

            return runner;
        }

        private static void BuildTest(IAssemblyInfo assembly, NUnitTest parentTest, NUnitITest nunitTest)
        {
            string kind;
            ICodeElementInfo codeElement;

            switch (nunitTest.TestType)
            {
#if NUNIT248
                case @"Test Case":
#else
                case @"NUnitTestMethod":
#endif
                    kind = TestKinds.Test;
                    codeElement = ParseTestCaseName(assembly, nunitTest.TestName.FullName);
                    break;

#if NUNIT248
                case @"Test Fixture":
#else
                case @"NUnitTestFixture":
#endif
                    kind = TestKinds.Fixture;
                    codeElement = ParseTestFixtureName(assembly, nunitTest.TestName.FullName);
                    break;

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

        private static void BuildTestChildren(IAssemblyInfo assembly, NUnitTest parentTest, NUnitITest parentNUnitTest)
        {
            if (parentNUnitTest.Tests != null)
            {
                foreach (NUnitITest childNUnitTest in parentNUnitTest.Tests)
                    BuildTest(assembly, parentTest, childNUnitTest);
            }
        }

        private static void PopulateMetadata(NUnitTest test)
        {
            NUnitITest nunitTest = test.Test;

            if (!String.IsNullOrEmpty(nunitTest.Description))
                test.Metadata.Add(MetadataKeys.Description, nunitTest.Description);

            if (!String.IsNullOrEmpty(nunitTest.IgnoreReason))
                test.Metadata.Add(MetadataKeys.IgnoreReason, nunitTest.IgnoreReason);

            foreach (string category in nunitTest.Categories)
                test.Metadata.Add(MetadataKeys.Category, category);

            foreach (DictionaryEntry entry in nunitTest.Properties)
            {
                string keyString = entry.Key.ToString();
                if (!keyString.StartsWith("_"))
                {
                    ICollection values = entry.Value as ICollection;
                    if (values != null)
                    {
                        foreach (object value in values)
                            test.Metadata.Add(keyString, value != null ? value.ToString() : null);
                    }
                    else
                    {
                        test.Metadata.Add(keyString, entry.Value != null ? entry.Value.ToString() : null);
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
