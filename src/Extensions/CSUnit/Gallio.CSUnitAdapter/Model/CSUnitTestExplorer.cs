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
using csUnit;
using csUnit.Core;
using csUnit.Interfaces;
using Gallio.Common.Collections;
using Gallio.CSUnitAdapter.Properties;
using Gallio.Model;
using Gallio.Common.Reflection;

namespace Gallio.CSUnitAdapter.Model
{
    internal class CSUnitTestExplorer : BaseTestExplorer
    {
        private const string FrameworkName = "csUnit";
        private const string CSUnitAssemblyDisplayName = @"csUnit";

        public override void Explore(TestModel testModel, TestSource testSource, Action<ITest> consumer)
        {
            var state = new ExplorerState(testModel);

            foreach (IAssemblyInfo assembly in testSource.Assemblies)
                state.ExploreAssembly(assembly, consumer);
        }

        private sealed class ExplorerState
        {
            private readonly TestModel testModel;
            private readonly Dictionary<Version, ITest> frameworkTests;
            private readonly Dictionary<IAssemblyInfo, ITest> assemblyTests;

            public ExplorerState(TestModel testModel)
            {
                this.testModel = testModel;
                frameworkTests = new Dictionary<Version, ITest>();
                assemblyTests = new Dictionary<IAssemblyInfo, ITest>();
            }

            public void ExploreAssembly(IAssemblyInfo assembly, Action<ITest> consumer)
            {
                Version frameworkVersion = GetFrameworkVersion(assembly);

                if (frameworkVersion != null)
                {
                    ITest frameworkTest = GetFrameworkTest(frameworkVersion, testModel.RootTest);
                    ITest assemblyTest = GetAssemblyTest(assembly, frameworkTest);

                    if (consumer != null && assemblyTest != null)
                        consumer(assemblyTest);
                }
            }

            private static Version GetFrameworkVersion(IAssemblyInfo assembly)
            {
                if (assembly != null)
                {
                    AssemblyName frameworkAssemblyName = ReflectionUtils.FindAssemblyReference(assembly, CSUnitAssemblyDisplayName);
                    if (frameworkAssemblyName != null)
                    {
                        return frameworkAssemblyName.Version;
                    }
                }
                return null;
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
                if (assemblyTests.TryGetValue(assembly, out assemblyTest))
                    return assemblyTest;

                try
                {
                    Assembly loadedAssembly = assembly.Resolve(false);

                    if (loadedAssembly != null)
                        assemblyTest = BuildAssemblyTest_Native(assembly, loadedAssembly.Location);
                    else
                        assemblyTest = BuildAssemblyTest_Reflective(assembly);
                }
                catch (Exception ex)
                {
                    testModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly,
                        "An exception was thrown while exploring a csUnit test assembly.", ex));
                    return null;
                }

                if (assemblyTest != null)
                {
                    frameworkTest.AddChild(assemblyTest);
                    
                    assemblyTests.Add(assembly, assemblyTest);
                }
                return assemblyTest;
            }

            private ITest BuildAssemblyTest_Native(IAssemblyInfo assembly, string location)
            {
                if (String.IsNullOrEmpty(location))
                    throw new ArgumentNullException("location");

                // Load the assembly using the native CSUnit loader.
                using (Loader loader = new Loader(location))
                {
                    // Construct the test tree
                    return CreateAssemblyTest(assembly, location, delegate(ITest assemblyTest)
                    {
                        TestFixtureInfoCollection collection = loader.TestFixtureInfos;
                        if (collection == null)
                            return;

                        foreach (ITestFixtureInfo fixtureInfo in collection)
                        {
                            try
                            {
                                ITypeInfo fixtureType = assembly.GetType(fixtureInfo.FullName);

                                assemblyTest.AddChild(CreateFixtureFromType(fixtureType, delegate(ITest fixtureTest)
                                {
                                    if (fixtureInfo.TestMethods == null)
                                        return;

                                    foreach (ITestMethodInfo testMethodInfo in fixtureInfo.TestMethods)
                                    {
                                        try
                                        {
                                            IMethodInfo methodType = fixtureType.GetMethod(testMethodInfo.Name,
                                                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

                                            fixtureTest.AddChild(CreateTestFromMethod(methodType, null));
                                        }
                                        catch (Exception ex)
                                        {
                                            testModel.AddAnnotation(new Annotation(AnnotationType.Error, fixtureType,
                                                "An exception was thrown while exploring a csUnit test case.", ex));
                                        }
                                    }
                                }));
                            }
                            catch (Exception ex)
                            {
                                testModel.AddAnnotation(new Annotation(AnnotationType.Error, assembly,
                                    "An exception was thrown while exploring a csUnit test fixture.", ex));
                            }
                        }
                    });
                }
            }

            private ITest BuildAssemblyTest_Reflective(IAssemblyInfo assembly)
            {
                // Construct the test tree
                return CreateAssemblyTest(assembly, String.Empty, delegate(ITest assemblyTest)
                {
                    foreach (ITypeInfo fixtureType in assembly.GetExportedTypes())
                    {
                        if (!IsTestFixture(fixtureType))
                            continue;

                        assemblyTest.AddChild(CreateFixtureFromType(fixtureType, delegate(ITest fixtureTest)
                        {
                            foreach (IMethodInfo methodType in fixtureType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                            {
                                if (!IsTestCase(methodType))
                                    continue;

                                fixtureTest.AddChild(CreateTestFromMethod(methodType, null));
                            }
                        }));
                    }
                });
            }

            private static bool IsTestFixture(ITypeInfo fixture)
            {
                if (fixture.IsAbstract)
                    return false;

                if (AttributeUtils.HasAttribute<TestFixtureAttribute>(fixture, true))
                    return true;

                if (fixture.Name.EndsWith("Test"))
                    return true;

                return false;
            }

            private static bool IsTestCase(IMethodInfo method)
            {
                if (method.ReturnType.FullName != "System.Void")
                    return false;

                //if (method.IsGenericMethod)
                //    return false;

                if (AttributeUtils.HasAttribute<FixtureSetUpAttribute>(method, true) ||
                    AttributeUtils.HasAttribute<FixtureTearDownAttribute>(method, true))
                    return false;

                if (AttributeUtils.HasAttribute<SetUpAttribute>(method, true) ||
                    AttributeUtils.HasAttribute<TearDownAttribute>(method, true))
                    return false;

                if (method.Name.ToLower().Equals("setup") ||
                    method.Name.ToLower().Equals("teardown"))
                    return false;

                if (AttributeUtils.HasAttribute<TestAttribute>(method, true))
                    return true;

                if (method.Parameters.Count > 0)
                    return false; // Parameterized tests must have an attribute

                if (method.Name.ToLower().StartsWith("test"))
                    return true;

                return false;
            }

            private static ITest CreateFrameworkTest(Version frameworkVersion)
            {
                string name = String.Format(Resources.CSUnitTestExplorer_FrameworkNameWithVersionFormat, frameworkVersion);

                BaseTest frameworkTest = new BaseTest(name, null);
                frameworkTest.LocalIdHint = FrameworkName;
                frameworkTest.Kind = TestKinds.Framework;
                frameworkTest.Metadata.Add(MetadataKeys.Framework, FrameworkName);

                return frameworkTest;
            }

            private static ITest CreateAssemblyTest(IAssemblyInfo assembly, string assemblyLocation, Action<ITest> consumer)
            {
                CSUnitAssemblyTest assemblyTest = new CSUnitAssemblyTest(assembly, assemblyLocation);
                assemblyTest.LocalIdHint = assembly.Name; // used to do reverse lookup
                assemblyTest.Kind = TestKinds.Assembly;

                PopulateAssemblyMetadata(assembly, assemblyTest.Metadata);

                if (consumer != null)
                    consumer(assemblyTest);

                return assemblyTest;
            }

            private static ITest CreateFixtureFromType(ITypeInfo fixtureType, Action<ITest> consumer)
            {
                CSUnitTest fixtureTest = new CSUnitTest(fixtureType.Name, fixtureType);
                fixtureTest.LocalIdHint = fixtureType.FullName; // used to do reverse lookup
                fixtureTest.Kind = TestKinds.Fixture;

                PopulateFixtureMetadata(fixtureType, fixtureTest.Metadata);

                if (consumer != null)
                    consumer(fixtureTest);

                return fixtureTest;
            }

            private static ITest CreateTestFromMethod(IMethodInfo methodType, Action<BaseTest> consumer)
            {
                CSUnitTest method = new CSUnitTest(methodType.Name, methodType);
                method.LocalIdHint = methodType.DeclaringType.FullName + @"." + methodType.Name; // used to do reverse lookup
                method.Kind = TestKinds.Test;
                method.IsTestCase = true;

                PopulateMethodMetadata(methodType, method.Metadata);

                if (consumer != null)
                    consumer(method);

                return method;
            }

            #region Populate Metadata methods

            private static void PopulateAssemblyMetadata(IAssemblyInfo codeElement, PropertyBag metadata)
            {
                ModelUtils.PopulateMetadataFromAssembly(codeElement, metadata);

                // Add documentation.
                string xmlDocumentation = codeElement.GetXmlDocumentation();
                if (!String.IsNullOrEmpty(xmlDocumentation))
                {
                    metadata.Add(MetadataKeys.XmlDocumentation, xmlDocumentation);
                }
            }

            private static void PopulateFixtureMetadata(ICodeElementInfo codeElement, PropertyBag metadata)
            {
                foreach (TestFixtureAttribute attr in AttributeUtils.GetAttributes<TestFixtureAttribute>(codeElement, true))
                {
                    // Add categories 
                    string categories = attr.Categories;
                    if (!String.IsNullOrEmpty(categories))
                    {
                        foreach (string category in categories.Split(','))
                        {
                            metadata.Add(MetadataKeys.Category, category);
                        }
                    }
                }

                PopulateCommonMetadata(codeElement, metadata);
            }

            private static void PopulateMethodMetadata(ICodeElementInfo codeElement, PropertyBag metadata)
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
                            metadata.Add(MetadataKeys.Category, category);
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

            private static void PopulateCommonMetadata(ICodeElementInfo codeElement, PropertyBag metadata)
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

            #endregion
        }
    }
}
