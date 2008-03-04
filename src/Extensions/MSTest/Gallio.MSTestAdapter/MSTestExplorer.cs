// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using System.Collections.Generic;
using Gallio.Reflection;
using Gallio.Model;

namespace Gallio.MSTestAdapter
{
    /// <summary>
    /// Explores tests in MSTest assemblies.
    /// </summary>
    internal class MSTestExplorer : BaseTestExplorer
    {
        private const string MSTestAssemblyDisplayName = @"Microsoft.VisualStudio.QualityTools.UnitTestFramework";
        public readonly Dictionary<Version, ITest> frameworkTests;
        public readonly Dictionary<IAssemblyInfo, ITest> assemblyTests;
        public readonly Dictionary<ITypeInfo, ITest> typeTests;

        public MSTestExplorer(TestModel testModel)
            : base(testModel)
        {
            frameworkTests = new Dictionary<Version, ITest>();
            assemblyTests = new Dictionary<IAssemblyInfo, ITest>();
            typeTests = new Dictionary<ITypeInfo, ITest>();
        }

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, System.Action<ITest> consumer)
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
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, MSTestAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private ITest GetFrameworkTest(Version frameworkVersion, ITest rootTest)
        {
            ITest frameworkTest;
            if (!frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
            {
                frameworkTest = CreateFrameworkTest(frameworkVersion);
                rootTest.AddChild(frameworkTest);

                frameworkTests.Add(frameworkVersion, frameworkTest);
            }

            return frameworkTest;
        }

        private static ITest CreateFrameworkTest(Version frameworkVersion)
        {
            //TODO: Use resource strings
            BaseTest frameworkTest = new BaseTest(String.Format("MSTest v{0}", frameworkVersion), null);
            frameworkTest.BaselineLocalId = "MSTest";
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
            MSTest assemblyTest = new MSTest(assembly.Name, assembly);
            assemblyTest.Kind = TestKinds.Assembly;

            ModelUtils.PopulateMetadataFromAssembly(assembly, assemblyTest.Metadata);

            return assemblyTest;
        }

        private ITest TryGetTypeTest(ITypeInfo type, ITest assemblyTest)
        {
            ITest typeTest;
            if (!typeTests.TryGetValue(type, out typeTest))
            {
                try
                {
                    if (IsFixture(type))
                    {
                        typeTest = CreateTypeTest(type);
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Localize this string
                    typeTest = new ErrorTest(type,
                        String.Format("An exception occurred while generating an MSTest test from '{0}'.", type),
                        ex);
                }

                if (typeTest != null)
                {
                    assemblyTest.AddChild(typeTest);
                    typeTests.Add(type, typeTest);
                }
            }

            return typeTest;
        }
        
        private static MSTest CreateTypeTest(ITypeInfo typeInfo)
        {
            MSTest typeTest = new MSTest(typeInfo.Name, typeInfo);
            typeTest.Kind = TestKinds.Fixture;

            foreach (IMethodInfo method in typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                IEnumerable<IAttributeInfo> methodAttributes = method.GetAttributeInfos(null, true);
                foreach (IAttributeInfo methodAttribute in methodAttributes)
                {
                    if (methodAttribute.Type.FullName.CompareTo(MSTestAttributes.TestMethodAttribute) == 0)
                    {
                        MSTest testMethod = CreateMethodTest(typeInfo, method);
                        typeTest.AddChild(testMethod);
                        break;
                    }
                }
            }

            // Add XML documentation.
            string xmlDocumentation = typeInfo.GetXmlDocumentation();
            if (xmlDocumentation != null)
                typeTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);

            return typeTest;
        }

        private static MSTest CreateMethodTest(ITypeInfo typeInfo, IMethodInfo methodInfo)
        {
            MSTest methodTest = new MSTest(methodInfo.Name, methodInfo);
            methodTest.Kind = TestKinds.Test;
            methodTest.IsTestCase = true;

            // Add skip reason.
            string skipReason = null;
            if (IsIgnored(methodInfo, out skipReason))
            {
                if (skipReason != null)
                    methodTest.Metadata.SetValue(MetadataKeys.IgnoreReason, skipReason);
            }

            // Add traits.
            //if (XunitMethodUtility.HasTraits(methodInfo))
            //{
            //    foreach (KeyValuePair<string, string> entry in XunitMethodUtility.GetTraits(methodInfo))
            //    {
            //        methodTest.Metadata.Add(entry.Key ?? @"", entry.Value ?? @"");
            //    }
            //}

            // Add XML documentation.
            string xmlDocumentation = methodInfo.GetXmlDocumentation();
            if (xmlDocumentation != null)
                methodTest.Metadata.SetValue(MetadataKeys.XmlDocumentation, xmlDocumentation);

            return methodTest;
        }

        private static bool IsFixture(ITypeInfo type)
        {
            IEnumerable<IAttributeInfo> attributes = type.GetAttributeInfos(null, true);
            foreach (IAttributeInfo attribute in attributes)
            {
                if (attribute.Type.FullName.CompareTo(MSTestAttributes.TestClassAttribute) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsIgnored(IMethodInfo methodInfo, out string skipReason)
        {
            IEnumerable<IAttributeInfo> attributes = methodInfo.GetAttributeInfos(null, true);
            foreach (IAttributeInfo attribute in attributes)
            {
                if (attribute.Type.FullName.CompareTo(MSTestAttributes.IgnoreAttribute) == 0)
                {
                    //TODO: Get the real reason
                    skipReason = "Ignored";
                    return true;
                }
            }

            skipReason = null;
            return false;
        }
    }
}