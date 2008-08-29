// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using csUnit;
using csUnit.Core;
using csUnit.Interfaces;
using Gallio.CSUnitAdapter.Properties;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.CSUnitAdapter.Model
{
    internal class CSUnitTestExplorer : BaseTestExplorer
    {
        private const string CSUnitAssemblyDisplayName = @"csUnit";

        public readonly Dictionary<Version, ITest> frameworkTests;
        public readonly Dictionary<IAssemblyInfo, ITest> assemblyTests;

        public CSUnitTestExplorer(TestModel testModel) 
            : base(testModel)
        {
            frameworkTests = new Dictionary<Version, ITest>();
            assemblyTests = new Dictionary<IAssemblyInfo, ITest>();
        }

        #region ITestExplorer Members

        /// <inheritdoc />
        public override void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
        {
            Version frameworkVersion = GetFrameworkVersion(assembly);

            if (frameworkVersion != null)
            {
                ITest frameworkTest = GetFrameworkTest(frameworkVersion, TestModel.RootTest);
                ITest assemblyTest = GetAssemblyTest(assembly, frameworkTest);

                if (consumer != null && assemblyTest != null)
                    consumer(assemblyTest);
            }
        }

        #endregion

        private static Version GetFrameworkVersion(IAssemblyInfo assembly)
        {
            AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, CSUnitAssemblyDisplayName);
            return frameworkAssemblyName != null ? frameworkAssemblyName.Version : null;
        }

        private ITest GetFrameworkTest(Version frameworkVersion, ITest rootTest)
        {
            ITest frameworkTest;
            if (!frameworkTests.TryGetValue(frameworkVersion, out frameworkTest))
            {
                frameworkTest = CreateFrameworkTest(frameworkVersion);
                if (frameworkTest != null)
                {
                    rootTest.AddChild(frameworkTest);

                    frameworkTests.Add(frameworkVersion, frameworkTest);
                }
            }
            return frameworkTest;
        }

        private ITest GetAssemblyTest(IAssemblyInfo assembly, ITest frameworkTest)
        {
            ITest assemblyTest;
            if (!assemblyTests.TryGetValue(assembly, out assemblyTest))
            {
                assemblyTest = CreateAssemblyTest(assembly);
                if (assemblyTest != null)
                {
                    frameworkTest.AddChild(assemblyTest);
                    
                    assemblyTests.Add(assembly, assemblyTest);
                }
            }
            return assemblyTest;
        }

        private static ITest CreateFrameworkTest(Version frameworkVersion)
        {
            string name = String.Format(Resources.CSUnitTestExplorer_FrameworkNameWithVersionFormat, frameworkVersion);
            
            BaseTest frameworkTest = new BaseTest(name, null);
            frameworkTest.BaselineLocalId = Resources.CSUnitTestFramework_FrameworkName;
            frameworkTest.Kind = TestKinds.Framework;

            return frameworkTest;
        }

        private ITest CreateAssemblyTest(IAssemblyInfo assembly)
        {
            // Resolve test assembly.
            string location;
            try
            {
                location = assembly.Resolve(true).Location;
            }
            catch (Exception ex)
            {
                TestModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly,
                    "Could not resolve the location of an csUnit test assembly.", ex));
                return null;
            }
            try
            {
                // Load the assembly using the native CSUnit loader.
                Loader loader = new Loader(location);

                // TODO: Pure reflective loader for cases like ReSharper where the assembly has not been compiled yet.

                CSUnitAssemblyTest assemblyTest = new CSUnitAssemblyTest(assembly, location);
                assemblyTest.BaselineLocalId = assembly.Name; // used to do reverse lookup
                assemblyTest.Kind = TestKinds.Assembly;

                PopulateAssemblyMetadata(assembly, assemblyTest.Metadata);

                foreach (ITestFixtureInfo testFixture in loader.TestFixtureInfos)
                {
                    ITest fixture = CreateTestFixture(assemblyTest, testFixture);

                    assemblyTest.AddChild(fixture);
                }
                return assemblyTest;
            }
            catch (Exception ex)
            {
                TestModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly,
                    "An exception was thrown while exploring an csUnit test assembly.", ex));
                return null;
            }
        }

        private static ITest CreateTestFixture(CSUnitAssemblyTest parent, ITestFixtureInfo testFixture)
        {
            ICodeElementInfo codeElement = ((IAssemblyInfo)parent.CodeElement).GetType(testFixture.FullName);
            
            CSUnitTest fixture = new CSUnitTest(codeElement.Name, codeElement);
            fixture.BaselineLocalId = testFixture.FullName; // used to do reverse lookup
            fixture.Kind = TestKinds.Fixture;

            PopulateFixtureMetadata(fixture.CodeElement, fixture.Metadata);

            if (testFixture.TestMethods != null)
            {
                foreach (ITestMethodInfo testMethod in testFixture.TestMethods)
                {
                    ITest method = CreateTest(fixture, testMethod);

                    fixture.AddChild(method);
                }
            }
            return fixture;
        }

        private static ITest CreateTest(CSUnitTest parent, ITestMethodInfo testMethod)
        {
            ICodeElementInfo codeElement = ((ITypeInfo)parent.CodeElement).GetMethod(testMethod.Name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
            
            CSUnitTest method = new CSUnitTest(codeElement.Name, codeElement);
            method.BaselineLocalId = testMethod.FullName; // used to do reverse lookup
            method.Kind = TestKinds.Test;
            method.IsTestCase = true;

            PopulateMethodMetadata(method.CodeElement, method.Metadata);

            return method;
        }

        public static void PopulateAssemblyMetadata(IAssemblyInfo codeElement, MetadataMap metadata)
        {
            ModelUtils.PopulateMetadataFromAssembly(codeElement, metadata);

            // Add documentation.
            string xmlDocumentation = codeElement.GetXmlDocumentation();
            if (!String.IsNullOrEmpty(xmlDocumentation))
            {
                metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
            }
        }

        public static void PopulateFixtureMetadata(ICodeElementInfo codeElement, MetadataMap metadata)
        {
            foreach (TestFixtureAttribute attr in AttributeUtils.GetAttributes<TestFixtureAttribute>(codeElement, true))
            {
                // Add categories 
                string categories = attr.Categories;
                if (!String.IsNullOrEmpty(categories))
                {
                    foreach (string category in categories.Split(','))
                    {
                        metadata.Add(MetadataKeys.CategoryName, category);
                    }
                }
            }

            PopulateCommonMetadata(codeElement, metadata);
        }

        public static void PopulateMethodMetadata(ICodeElementInfo codeElement, MetadataMap metadata)
        {
            foreach (TestAttribute attr in AttributeUtils.GetAttributes<TestAttribute>(codeElement, true))
            {
                // Add timeout 
                if (attr.Timeout > 0)
                {
                    TimeSpan timeout = TimeSpan.FromMilliseconds(attr.Timeout);
                    metadata.Add("Timeout", timeout.Seconds.ToString("0.000"));
                }

                // Add categories 
                string categories = attr.Categories;
                if (!String.IsNullOrEmpty(categories))
                {
                    foreach (string category in categories.Split(','))
                    {
                        metadata.Add(MetadataKeys.CategoryName, category);
                    }
                }
            }

            // Add expected exception type
            foreach (ExpectedExceptionAttribute attr in AttributeUtils.GetAttributes<ExpectedExceptionAttribute>(codeElement, true))
            {
                metadata.Add(MetadataKeys.ExpectedException, attr.ExceptionType.FullName);
            }

            PopulateCommonMetadata(codeElement, metadata);
        }

        private static void PopulateCommonMetadata(ICodeElementInfo codeElement, MetadataMap metadata)
        {
            // Add ignore reason.
            foreach (IgnoreAttribute attr in AttributeUtils.GetAttributes<IgnoreAttribute>(codeElement, true))
            {
                metadata.Add(MetadataKeys.IgnoreReason, attr.Reason ?? "<unknown>");
            }

            // Add documentation.
            string xmlDocumentation = codeElement.GetXmlDocumentation();
            if (!String.IsNullOrEmpty(xmlDocumentation))
            {
                metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
            }
        }     
    }

}
