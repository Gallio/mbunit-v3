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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Gallio.Collections;
using Gallio.Hosting;
using Gallio.Model.Data;
using Gallio.Model;
using Gallio.Model.Reflection;
using Gallio.Plugin.NUnitAdapter.Properties;
using NUnit.Core;
using ITest=Gallio.Model.ITest;

namespace Gallio.Plugin.NUnitAdapter.Model
{
    /// <summary>
    /// The NUnit test framework template binding.
    /// This binding performs full exploration of all tests in MUnit test
    /// assemblies during test construction.
    /// </summary>
    public class NUnitFrameworkTemplateBinding : BaseTemplateBinding
    {
        /// <summary>
        /// The metadata key used for recording NUnit's internal test type
        /// when it couldn't be mapped to one of the standard types.
        /// </summary>
        private const string NUnitTestTypeKey = "NUnit:TestType";

        private TestRunner runner;
        private Dictionary<string, IAssemblyInfo> resolvedAssembliesByLocation;

        /// <summary>
        /// Creates a template binding.
        /// </summary>
        /// <param name="template">The template that was bound</param>
        /// <param name="scope">The scope in which the binding occurred</param>
        /// <param name="arguments">The template arguments</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="template"/>,
        /// <paramref name="scope"/> or <paramref name="arguments"/> is null</exception>
        public NUnitFrameworkTemplateBinding(NUnitFrameworkTemplate template, TemplateBindingScope scope,
            IDictionary<ITemplateParameter, IDataFactory> arguments)
            : base(template, scope, arguments)
        {
        }

        /// <summary>
        /// Gets the test runner.
        /// </summary>
        public TestRunner Runner
        {
            get { return runner; }
        }

        /// <summary>
        /// Gets the list of assemblies.
        /// </summary>
        public IList<IAssemblyInfo> Assemblies
        {
            get { return ((NUnitFrameworkTemplate) Template).Assemblies; }
        }

        /// <inheritdoc />
        public override void BuildTests(TestTreeBuilder builder, ITest parent)
        {
            LoadTestPackageIfNeeded();

            // Note: The NUnit test tree when constructed this way
            //       includes a root test node simply called "Tests".
            //       There's no point showing this to the user do instead
            //       we assimilate it in the framework-level test.
            NUnit.Core.ITest rootTest = runner.Test;
            BuildFrameworkTest(parent, rootTest);
        }

        private void LoadTestPackageIfNeeded()
        {
            if (runner != null)
                return;

            try
            {
                // Note: If we don't initialize the host, then we can't enumerate tests!
                //       Interestingly we don't get any runtime errors if we forget...
                CoreExtensions.Host.InitializeService();

                TestPackage package = new TestPackage(@"Tests");

                // Note: Don't build nodes for namespaces.  Grouping by namespace is a
                //       presentation concern of the test runner, not strictly a structural one
                //       so we turn this feature off.
                package.Settings.Add(@"AutoNamespaceSuites", false);

                resolvedAssembliesByLocation = new Dictionary<string, IAssemblyInfo>();

                foreach (IAssemblyInfo assemblyInfo in Assemblies)
                {
                    try
                    {
                        Assembly assembly = assemblyInfo.Resolve();

                        string location = assembly.Location;
                        resolvedAssembliesByLocation[location] = assemblyInfo;
                        package.Assemblies.Add(location);
                    }
                    catch (Exception)
                    {
                        // Ignore assemblies that cannot be resolved.
                    }
                }

                runner = new RemoteTestRunner();
                if (!runner.Load(package))
                    throw new ModelException(Resources.NUnitFrameworkTemplateBinding_CannotLoadNUnitTestAssemblies);
            }
            catch (Exception)
            {
                runner = null;
                resolvedAssembliesByLocation = null;
                throw;
            }
        }

        private void BuildFrameworkTest(ITest parentTest, NUnit.Core.ITest nunitRootTest)
        {
            NUnitTest test = new NUnitTest(Template.Name, null, this, nunitRootTest);
            test.Kind = ComponentKind.Framework;
            PopulateMetadata(test, nunitRootTest);

            parentTest.AddChild(test);
            BuildChildren(test);
        }

        private void BuildTests(NUnitTest parentTest, NUnit.Core.ITest nunitTest)
        {
            string kind;
            ICodeElementInfo codeElement;
            bool unrecognizedTestType = false;

            switch (nunitTest.TestType)
            {
                case @"Test Case":
                    kind = ComponentKind.Test;
                    codeElement = ParseTestCaseName(parentTest.CodeElement, nunitTest.TestName.FullName);
                    break;

                case @"Test Fixture":
                    kind = ComponentKind.Fixture;
                    codeElement = ParseTestFixtureName(parentTest.CodeElement, nunitTest.TestName.FullName);
                    break;

                case @"Test Suite":
                    kind = ComponentKind.Suite;
                    codeElement = ParseTestSuiteName(nunitTest.TestName.FullName, ref kind);
                    break;

                default:
                    kind = nunitTest.IsSuite ? ComponentKind.Suite : ComponentKind.Test;
                    codeElement = null;
                    unrecognizedTestType = true;
                    break;
            }

            // The NUnit name for an assembly level test suite is the assembly's
            // full path name which I find somewhat cluttered. -- Jeff.
            string name = nunitTest.TestName.Name;
            if (codeElement is IAssemblyInfo)
                name = codeElement.Name;

            // Build the test.
            NUnitTest test = new NUnitTest(name, codeElement, this, nunitTest);
            test.Kind = kind;
            test.IsTestCase = !nunitTest.IsSuite;

            if (unrecognizedTestType)
                test.Metadata.Add(NUnitTestTypeKey, nunitTest.TestType);

            PopulateMetadata(test, nunitTest);

            parentTest.AddChild(test);
            BuildChildren(test);
        }

        private void BuildChildren(NUnitTest test)
        {
            if (test.Test != null && test.Test.Tests != null)
            {
                foreach (NUnit.Core.ITest childNUnitTest in test.Test.Tests)
                    BuildTests(test, childNUnitTest);
            }
        }

        private static void PopulateMetadata(NUnitTest test, NUnit.Core.ITest nunitTest)
        {
            if (!String.IsNullOrEmpty(nunitTest.Description))
                test.Metadata.Add(MetadataKeys.Description, nunitTest.Description);

            if (!String.IsNullOrEmpty(nunitTest.IgnoreReason))
                test.Metadata.Add(MetadataKeys.IgnoreReason, nunitTest.IgnoreReason);

            foreach (string category in nunitTest.Categories)
                test.Metadata.Add(MetadataKeys.CategoryName, category);

            foreach (DictionaryEntry entry in nunitTest.Properties)
                test.Metadata.Add(entry.Key.ToString(), entry.Value != null ? entry.Value.ToString() : null);

            ICodeElementInfo codeElement = test.CodeElement;
            if (codeElement != null)
            {
                // Add documentation.
                string xmlDocumentation = codeElement.GetXmlDocumentation();
                if (xmlDocumentation != null)
                    test.Metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

                // Add assembly-level metadata.
                IAssemblyInfo assemblyInfo = codeElement as IAssemblyInfo;
                if (assemblyInfo != null)
                    ModelUtils.PopulateMetadataFromAssembly(assemblyInfo, test.Metadata);
            }
        }

        /// <summary>
        /// Parses a code element from an NUnit test case name.
        /// The name generally consists of the fixture type full-name followed by
        /// a dot and the test method name.
        /// </summary>
        private static ICodeElementInfo ParseTestCaseName(ICodeElementInfo parent, string name)
        {
            if (IsProbableIdentifier(name))
            {
                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(parent);
                if (assembly != null)
                {
                    int lastDot = name.LastIndexOf('.');
                    if (lastDot > 0 && lastDot < name.Length - 1)
                    {
                        string typeName = name.Substring(0, lastDot);
                        string methodName = name.Substring(lastDot + 1);

                        ITypeInfo type = assembly.GetType(typeName);
                        if (type != null)
                            return type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Parses a code reference from an NUnit test fixture name.
        /// The name generally consists of the fixture type full-name.
        /// </summary>
        private static ICodeElementInfo ParseTestFixtureName(ICodeElementInfo parent, string name)
        {
            if (IsProbableIdentifier(name))
            {
                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(parent);
                if (assembly != null)
                    return assembly.GetType(name);
            }

            return null;
        }

        /// <summary>
        /// Parses a code reference from an NUnit test suite name.
        /// The name generally consists of the assembly filename or namespace name
        /// but it might be user-generated also.
        /// </summary>
        private ICodeElementInfo ParseTestSuiteName(string name, ref string kind)
        {
            IAssemblyInfo assembly;
            if (resolvedAssembliesByLocation.TryGetValue(name, out assembly))
            {
                kind = ComponentKind.Assembly;
                return assembly;
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
