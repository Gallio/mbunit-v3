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
using Gallio.Model;
using Gallio.Model.Reflection;
using Gallio.Plugin.XunitAdapter.Properties;

using Xunit.Sdk;
using ITypeInfo = Gallio.Model.Reflection.ITypeInfo;
using XunitMethodUtility = Xunit.Sdk.MethodUtility;
using XunitTypeUtility = Xunit.Sdk.TypeUtility;

namespace Gallio.Plugin.XunitAdapter.Model
{
    /// <summary>
    /// Explores tests in Xunit assemblies.
    /// </summary>
    internal class XunitTestExplorer : BaseTestExplorer
    {
        private const string XunitAssemblyDisplayName = @"xunit";

        public readonly Dictionary<Version, ITest> frameworkTests;
        public readonly Dictionary<IAssemblyInfo, ITest> assemblyTests;
        public readonly Dictionary<ITypeInfo, ITest> typeTests;

        public XunitTestExplorer(TestModel testModel) : base(testModel)
        {
            frameworkTests = new Dictionary<Version, ITest>();
            assemblyTests = new Dictionary<IAssemblyInfo, ITest>();
            typeTests = new Dictionary<ITypeInfo, ITest>();
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            Version frameworkVersion = GetFrameworkVersion(assembly);

            if (frameworkVersion != null)
            {
                ITest frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
                ITest assemblyTest = GetAssemblyTest(assembly, frameworkTest, true);

                if (consumer != null)
                    consumer(assemblyTest);
            }
        }

        /// <inheritdoc />
        public override void ExploreType(ITypeInfo type, Action<ITest> consumer)
        {
            IAssemblyInfo assembly = type.Assembly;
            Version frameworkVersion = GetFrameworkVersion(assembly);

            if (frameworkVersion != null)
            {
                ITest frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
                ITest assemblyTest = GetAssemblyTest(assembly, frameworkTest, false);

                ITest typeTest = TryGetTypeTest(type, assemblyTest);
                if (typeTest != null && consumer != null)
                    consumer(typeTest);
            }
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, XunitAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private ITest GetFrameworkTest(Version frameworkVersion, ITest rootTest)
        {
            ITest frameworkTest;
            if (! frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
            {
                frameworkTest = CreateFrameworkTest(frameworkVersion);
                rootTest.AddChild(frameworkTest);

                frameworkTests.Add(frameworkVersion, frameworkTest);
            }

            return frameworkTest;
        }

        private static ITest CreateFrameworkTest(Version frameworkVersion)
        {
            BaseTest frameworkTest = new BaseTest(String.Format(Resources.XunitTestExplorer_FrameworkNameWithVersionFormat, frameworkVersion), null);
            frameworkTest.BaselineLocalId = Resources.XunitTestFramework_XunitFrameworkName;
            frameworkTest.Kind = TestKinds.Framework;

            return frameworkTest;
        }

        private ITest GetAssemblyTest(IAssemblyInfo assembly, ITest frameworkTest, bool populateRecursively)
        {
            ITest assemblyTest;
            if (!assemblyTests.TryGetValue(assembly, out assemblyTest))
            {
                assemblyTest = CreateAssemblyTest(assembly);
                frameworkTest.AddChild(assemblyTest);

                assemblyTests.Add(assembly, assemblyTest);
            }

            if (populateRecursively)
            {
                foreach (ITypeInfo type in assembly.GetExportedTypes())
                    TryGetTypeTest(type, assemblyTest);
            }

            return assemblyTest;
        }

        private static ITest CreateAssemblyTest(IAssemblyInfo assembly)
        {
            BaseTest assemblyTest = new BaseTest(assembly.Name, assembly);
            assemblyTest.Kind = TestKinds.Assembly;

            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            return assemblyTest;
        }

        private ITest TryGetTypeTest(ITypeInfo type, ITest assemblyTest)
        {
            ITest typeTest;
            if (!typeTests.TryGetValue(type, out typeTest))
            {
                XunitTypeInfoAdapter xunitTypeInfo = new XunitTypeInfoAdapter(type);
                ITestClassCommand command = TestClassCommandFactory.Make(xunitTypeInfo);
                if (command != null)
                {
                    typeTest = CreateTypeTest(xunitTypeInfo, command);
                    assemblyTest.AddChild(typeTest);

                    typeTests.Add(type, typeTest);
                }
            }

            return typeTest;
        }

        private static XunitTest CreateTypeTest(XunitTypeInfoAdapter typeInfo, ITestClassCommand testClassCommand)
        {
            XunitTest typeTest = new XunitTest(typeInfo.Target.Name, typeInfo.Target, typeInfo, null);
            typeTest.Kind = TestKinds.Fixture;

            foreach (XunitMethodInfoAdapter methodInfo in testClassCommand.EnumerateTestMethods())
                typeTest.AddChild(CreateMethodTest(typeInfo, methodInfo));

            // Add XML documentation.
            string xmlDocumentation = typeInfo.Target.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);

            return typeTest;
        }

        private static XunitTest CreateMethodTest(XunitTypeInfoAdapter typeInfo, XunitMethodInfoAdapter methodInfo)
        {
            XunitTest methodTest = new XunitTest(methodInfo.Name, methodInfo.Target, typeInfo, methodInfo);
            methodTest.Kind = TestKinds.Test;
            methodTest.IsTestCase = true;

            // Add skip reason.
            if (XunitMethodUtility.IsSkip(methodInfo))
            {
                string skipReason = XunitMethodUtility.GetSkipReason(methodInfo);
                if (skipReason != null)
                    methodTest.Metadata.SetValue(MetadataKeys.IgnoreReason, skipReason);
            }

            // Add traits.
            if (XunitMethodUtility.HasTraits(methodInfo))
            {
                foreach (KeyValuePair<string, string> entry in XunitMethodUtility.GetTraits(methodInfo))
                {
                    methodTest.Metadata.Add(entry.Key ?? @"", entry.Value ?? @"");
                }
            }

            // Add XML documentation.
            string xmlDocumentation = methodInfo.Target.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);

            return methodTest;
        }
    }
}
