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
using System.Collections.Generic;
using System.Reflection;
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Helpers;
using Gallio.Model.Tree;
using Gallio.XunitAdapter.Properties;
using Xunit.Sdk;
using ITypeInfo = Gallio.Common.Reflection.ITypeInfo;
using XunitMethodUtility = Xunit.Sdk.MethodUtility;
using XunitTypeUtility = Xunit.Sdk.TypeUtility;

namespace Gallio.XunitAdapter.Model
{
    /// <summary>
    /// Explores tests in Xunit assemblies.
    /// </summary>
    internal class XunitTestExplorer : TestExplorer
    {
        private const string FrameworkName = "xUnit.net";
        private const string XunitAssemblyDisplayName = @"xunit";

        private readonly Dictionary<Version, Test> frameworkTests;
        private readonly Dictionary<IAssemblyInfo, Test> assemblyTests;
        private readonly Dictionary<ITypeInfo, Test> typeTests;

        public XunitTestExplorer()
        {
            frameworkTests = new Dictionary<Version, Test>();
            assemblyTests = new Dictionary<IAssemblyInfo, Test>();
            typeTests = new Dictionary<ITypeInfo, Test>();
        }

        protected override void ExploreImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
        {
            IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
            Version frameworkVersion = GetFrameworkVersion(assembly);

            if (frameworkVersion != null)
            {
                Test frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
                Test assemblyTest = GetAssemblyTest(assembly, frameworkTest, false);

                ITypeInfo type = ReflectionUtils.GetType(codeElement);
                if (type != null)
                {
                    TryGetTypeTest(type, assemblyTest);
                }
            }
        }

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, XunitAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private Test GetFrameworkTest(Version frameworkVersion, Test rootTest)
        {
            Test frameworkTest;
            if (! frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
            {
                frameworkTest = CreateFrameworkTest(frameworkVersion);
                rootTest.AddChild(frameworkTest);

                frameworkTests.Add(frameworkVersion, frameworkTest);
            }

            return frameworkTest;
        }

        private static Test CreateFrameworkTest(Version frameworkVersion)
        {
            Test frameworkTest = new Test(String.Format(Resources.XunitTestExplorer_FrameworkNameWithVersionFormat, frameworkVersion), null);
            frameworkTest.LocalIdHint = FrameworkName;
            frameworkTest.Kind = TestKinds.Framework;
            frameworkTest.Metadata.Add(MetadataKeys.Framework, FrameworkName);

            return frameworkTest;
        }

        private Test GetAssemblyTest(IAssemblyInfo assembly, Test frameworkTest, bool populateRecursively)
        {
            Test assemblyTest;
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

        private static Test CreateAssemblyTest(IAssemblyInfo assembly)
        {
            Test assemblyTest = new Test(assembly.Name, assembly);
            assemblyTest.Kind = TestKinds.Assembly;

            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            return assemblyTest;
        }

        private Test TryGetTypeTest(ITypeInfo type, Test assemblyTest)
        {
            Test typeTest;
            if (!typeTests.TryGetValue(type, out typeTest))
            {
                try
                {
                    XunitTypeInfoAdapter xunitTypeInfo = new XunitTypeInfoAdapter(type);
                    ITestClassCommand command = TestClassCommandFactory.Make(xunitTypeInfo);
                    if (command != null)
                        typeTest = CreateTypeTest(xunitTypeInfo, command);
                }
                catch (Exception ex)
                {
                    TestModel.AddAnnotation(new Annotation(AnnotationType.Error, type, "An exception was thrown while exploring an xUnit.net test type.", ex));
                }

                if (typeTest != null)
                {
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
