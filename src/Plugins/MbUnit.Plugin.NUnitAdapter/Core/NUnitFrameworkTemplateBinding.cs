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
using MbUnit.Framework;
using MbUnit.Framework.Kernel.DataBinding;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Utilities;
using MbUnit.Plugin.NUnitAdapter.Properties;
using NUnit.Core;
using ITest=MbUnit.Framework.Kernel.Model.ITest;

namespace MbUnit.Plugin.NUnitAdapter.Core
{
    /// <summary>
    /// The NUnit test framework template binding.
    /// This binding performs full exploration of all tests in MUnit test
    /// assemblies during test construction.
    /// </summary>
    public class NUnitFrameworkTemplateBinding : BaseTemplateBinding
    {
        private const string TestTypeMetadataKey = "NUnit.TestType";

        private TestRunner runner;

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
        public IList<Assembly> Assemblies
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
                foreach (Assembly assembly in Assemblies)
                    package.Assemblies.Add(assembly.Location);

                //runner = new SimpleTestRunner();
                runner = new RemoteTestRunner();
                if (!runner.Load(package))
                    throw new ModelException(Resources.NUnitFrameworkTemplateBinding_CannotLoadNUnitTestAssemblies);
            }
            catch (Exception)
            {
                runner = null;
                throw;
            }
        }

        private void BuildFrameworkTest(ITest parentTest, NUnit.Core.ITest nunitRootTest)
        {
            NUnitTest test = new NUnitTest(Template.Name, CodeReference.Unknown, this, nunitRootTest);
            test.Kind = ComponentKind.Framework;
            PopulateMetadata(test, nunitRootTest);

            parentTest.AddChild(test);
            BuildChildren(test);
        }

        private void BuildTests(NUnitTest parentTest, NUnit.Core.ITest nunitTest)
        {
            string kind;
            CodeReference codeReference;
            switch (nunitTest.TestType)
            {
                case @"Test Case":
                    kind = ComponentKind.Test;
                    codeReference = ParseTestCaseName(parentTest.CodeReference, nunitTest.TestName.FullName);
                    break;

                case @"Test Fixture":
                    kind = ComponentKind.Fixture;
                    codeReference = ParseTestFixtureName(parentTest.CodeReference, nunitTest.TestName.FullName);
                    break;

                case @"Test Suite":
                    kind = ComponentKind.Suite;
                    codeReference = ParseCodeReferenceFromTestSuiteName(parentTest.CodeReference, nunitTest.TestName.FullName, ref kind);
                    break;

                default:
                    kind = nunitTest.IsSuite ? ComponentKind.Suite : ComponentKind.Test;
                    codeReference = CodeReference.Unknown;
                    break;
            }

            NUnitTest test = new NUnitTest(nunitTest.TestName.FullName, codeReference, this, nunitTest);
            test.Kind = kind;
            test.IsTestCase = !nunitTest.IsSuite;
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
                test.Metadata.Entries.Add(MetadataKeys.Description, nunitTest.Description);

            if (!String.IsNullOrEmpty(nunitTest.IgnoreReason))
                test.Metadata.Entries.Add(MetadataKeys.IgnoreReason, nunitTest.IgnoreReason);

            foreach (string category in nunitTest.Categories)
                test.Metadata.Entries.Add(MetadataKeys.CategoryName, category);

            foreach (DictionaryEntry entry in nunitTest.Properties)
                test.Metadata.Entries.Add(entry.Key.ToString(), entry.Value != null ? entry.Value.ToString() : null);

            test.Metadata.Entries.Add(TestTypeMetadataKey, nunitTest.TestType);

            string xmlDocumentation = GetXmlDocumentation(test);
            if (xmlDocumentation != null)
                test.Metadata.Entries.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);

            // Add assembly-level metadata.
            if (test.CodeReference.Kind == CodeReferenceKind.Assembly)
                ReflectionUtils.PopulateMetadataFromAssembly(test.CodeReference.ResolveAssembly(), test.Metadata);
        }

        private static string GetXmlDocumentation(NUnitTest test)
        {
            try
            {
                switch (test.CodeReference.Kind)
                {
                    case CodeReferenceKind.Type:
                        Type type = test.CodeReference.ResolveType();
                        return Runtime.XmlDocumentationResolver.GetXmlDocumentation(type);

                    case CodeReferenceKind.Member:
                        MemberInfo member = test.CodeReference.ResolveMember();
                        return Runtime.XmlDocumentationResolver.GetXmlDocumentation(member);
                }
            }
            catch (Exception)
            {
                // Ignore any problems.  We don't care if this fails.
            }

            return null;
        }

        /// <summary>
        /// Parses a code reference from an NUnit test case name.
        /// The name generally consists of the fixture type full-name followed by
        /// a dot and the test method name.
        /// </summary>
        private static CodeReference ParseTestCaseName(CodeReference parent, string name)
        {
            if (parent.AssemblyName != null)
            {
                string namespaceName;
                string typeName;
                string methodName;
                if (SplitMethodName(name, out namespaceName, out typeName, out methodName))
                    return new CodeReference(parent.AssemblyName, namespaceName, typeName, methodName, null);
            }

            return new CodeReference(parent.AssemblyName, parent.NamespaceName, parent.TypeName, parent.MemberName, null);
        }

        /// <summary>
        /// Parses a code reference from an NUnit test fixture name.
        /// The name generally consists of the fixture type full-name.
        /// </summary>
        private CodeReference ParseTestFixtureName(CodeReference parent, string name)
        {
            if (parent.AssemblyName != null)
            {
                string namespaceName;
                string typeName;
                if (SplitTypeName(name, out namespaceName, out typeName))
                    return new CodeReference(parent.AssemblyName, namespaceName, typeName, null, null);
            }

            return new CodeReference(parent.AssemblyName, parent.NamespaceName, parent.TypeName, null, null);
        }

        /// <summary>
        /// Parses a code reference from an NUnit test suite name.
        /// The name generally consists of the assembly filename or namespace name
        /// but it might be user-generated also.
        /// </summary>
        private CodeReference ParseCodeReferenceFromTestSuiteName(CodeReference parent, string name, ref string kind)
        {
            Assembly assembly = GenericUtils.Find(Assemblies, delegate(Assembly candidate)
            {
                return candidate.Location == name;
            });

            if (assembly != null)
            {
                kind = ComponentKind.Assembly;
                return CodeReference.CreateFromAssembly(assembly);
            }

            if (parent.AssemblyName != null && IsProbableIdentifier(name))
            {
                kind = ComponentKind.Namespace;
                return new CodeReference(parent.AssemblyName, name, null, null, null);
            }

            return new CodeReference(parent.AssemblyName, parent.NamespaceName, parent.TypeName, null, null);
        }

        private static bool SplitTypeName(string name, out string namespaceName, out string typeName)
        {
            if (IsProbableIdentifier(name))
            {
                int lastDot = name.LastIndexOf('.');
                if (lastDot > 0 && lastDot < name.Length - 1)
                {
                    namespaceName = name.Substring(0, lastDot);
                    typeName = name; //name.Substring(lastDot + 1);
                    return true;
                }
            }

            namespaceName = null;
            typeName = null;
            return false;
        }

        private static bool SplitMethodName(string name, out string namespaceName,
            out string typeName, out string methodName)
        {
            if (IsProbableIdentifier(name))
            {
                int lastDot = name.LastIndexOf('.');
                if (lastDot > 0 && lastDot < name.Length - 1)
                {
                    string firstPart = name.Substring(0, lastDot);
                    if (SplitTypeName(firstPart, out namespaceName, out typeName))
                    {
                        methodName = name.Substring(lastDot + 1);
                        return true;
                    }
                }
            }

            namespaceName = null;
            typeName = null;
            methodName = null;
            return false;
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
