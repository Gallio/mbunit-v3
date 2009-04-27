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
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Gallio.Reflection;
using Gallio.Runtime;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Default implementation of a test framework manager.
    /// </summary>
    public class DefaultTestFrameworkManager : ITestFrameworkManager
    {
        private readonly ComponentHandle<ITestFramework, TestFrameworkTraits>[] frameworkHandles;

        /// <summary>
        /// Creates a test framework manager.
        /// </summary>
        /// <param name="frameworkHandles">The test framework handles</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworkHandles"/> is null</exception>
        public DefaultTestFrameworkManager(ComponentHandle<ITestFramework, TestFrameworkTraits>[] frameworkHandles)
        {
            if (frameworkHandles == null || Array.IndexOf(frameworkHandles, null) >= 0)
                throw new ArgumentNullException("frameworkHandles");

            this.frameworkHandles = frameworkHandles;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> FrameworkHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<ITestFramework, TestFrameworkTraits>>(frameworkHandles); }
        }

        /// <inheritdoc />
        public ITestExplorer GetTestExplorer(Predicate<TestFrameworkTraits> frameworkFilter)
        {
            if (frameworkFilter == null)
                return new FilteredTestExplorer(frameworkHandles);

            var filteredFrameworkHandles = new List<ComponentHandle<ITestFramework, TestFrameworkTraits>>();
            foreach (var frameworkHandle in frameworkHandles)
                if (frameworkFilter(frameworkHandle.GetTraits()))
                    filteredFrameworkHandles.Add(frameworkHandle);

            return new FilteredTestExplorer(filteredFrameworkHandles);
        }

        private sealed class FilteredTestExplorer : ITestExplorer
        {
            private readonly IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> frameworkHandles;

            public FilteredTestExplorer(IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> frameworkHandles)
            {
                this.frameworkHandles = frameworkHandles;
            }

            public void ConfigureTestDomain(TestDomainSetup testDomainSetup)
            {
                if (testDomainSetup == null)
                    throw new ArgumentNullException("testDomainSetup");

                var aggregateServices = new AggregateTestExplorer();
                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    if (frameworkTraits.RequiresConfigureTestDomain)
                        aggregateServices.RegisterFramework(frameworkHandle.GetComponent());
                }

                aggregateServices.ConfigureTestDomain(testDomainSetup);
            }

            public bool IsTest(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                if (reflectionPolicy == null)
                    throw new ArgumentNullException("reflectionPolicy");
                if (codeElement == null)
                    throw new ArgumentNullException("codeElement");

                var aggregateServices = CreateAggregateTestExplorerForCodeElement(codeElement);
                return aggregateServices.IsTest(reflectionPolicy, codeElement);
            }

            public bool IsTestPart(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                if (reflectionPolicy == null)
                    throw new ArgumentNullException("reflectionPolicy");
                if (codeElement == null)
                    throw new ArgumentNullException("codeElement");

                var aggregateServices = CreateAggregateTestExplorerForCodeElement(codeElement);
                return aggregateServices.IsTestPart(reflectionPolicy, codeElement);
            }

            public void Explore(TestModel testModel, TestSource testSource, Action<ITest> consumer)
            {
                if (testModel == null)
                    throw new ArgumentNullException("testModel");
                if (testSource == null)
                    throw new ArgumentNullException("testSource");

                var aggregateServices = CreateAggregateTestExplorerForTestSource(testSource);
                aggregateServices.Explore(testModel, testSource, consumer);
            }

            private AggregateTestExplorer CreateAggregateTestExplorerForTestSource(TestSource testSource)
            {
                var aggregateServices = new AggregateTestExplorer();

                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    if (IsFrameworkNeededForTestSource(frameworkTraits, testSource))
                        aggregateServices.RegisterFramework(frameworkHandle.GetComponent());
                }

                return aggregateServices;
            }

            private AggregateTestExplorer CreateAggregateTestExplorerForCodeElement(ICodeElementInfo codeElement)
            {
                var aggregateServices = new AggregateTestExplorer();

                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
                if (assembly != null)
                    PopulateAggregateServicesByFrameworkAssembly(aggregateServices, assembly);

                return aggregateServices;
            }

            private void PopulateAggregateServicesByFrameworkAssembly(AggregateTestExplorer aggregateTestExplorer, IAssemblyInfo assembly)
            {
                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    if (ContainsAssemblyReference(assembly, frameworkTraits.FrameworkAssemblyNames))
                        aggregateTestExplorer.RegisterFramework(frameworkHandle.GetComponent());
                }
            }

            private static bool IsFrameworkNeededForTestSource(TestFrameworkTraits frameworkTraits, TestSource testSource)
            {
                foreach (IAssemblyInfo assembly in testSource.Assemblies)
                {
                    if (ContainsAssemblyReference(assembly, frameworkTraits.FrameworkAssemblyNames))
                        return true;
                }

                foreach (ITypeInfo type in testSource.Types)
                {
                    if (ContainsAssemblyReference(type.Assembly, frameworkTraits.FrameworkAssemblyNames))
                        return true;
                }

                foreach (FileInfo file in testSource.Files)
                {
                    string extension = file.Extension;
                    if (Array.IndexOf(frameworkTraits.TestFileExtensions, extension) >= 0)
                        return true;
                }

                return false;
            }

            private static bool ContainsAssemblyReference(IAssemblyInfo assembly, string[] assemblyNames)
            {
                foreach (AssemblyName referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    foreach (string assemblyName in assemblyNames)
                    {
                        if (referencedAssemblyName.Name == assemblyName)
                            return true;
                    }
                }

                return false;
            }
        }

        private sealed class AggregateTestExplorer : ITestExplorer
        {
            private readonly List<ITestExplorer> explorers;

            public AggregateTestExplorer()
            {
                explorers = new List<ITestExplorer>();
            }

            public void RegisterFramework(ITestFramework framework)
            {
                framework.RegisterTestExplorers(explorers);
            }

            public void ConfigureTestDomain(TestDomainSetup testDomainSetup)
            {
                foreach (ITestExplorer explorer in explorers)
                {
                    try
                    {
                        explorer.ConfigureTestDomain(testDomainSetup);
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("A test framework failed to configure the test domain.  It has been skipped.", ex);
                    }
                }
            }

            public bool IsTest(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                foreach (ITestExplorer explorer in explorers)
                {
                    try
                    {
                        if (explorer.IsTest(reflectionPolicy, codeElement))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("A test framework failed to determine whether a code element represents a test.  It has been skipped.", ex);
                    }
                }

                return false;
            }

            public bool IsTestPart(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                foreach (ITestExplorer explorer in explorers)
                {
                    try
                    {
                        if (explorer.IsTestPart(reflectionPolicy, codeElement))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("A test framework failed to determine whether a code element represents a part of a test.  It has been skipped.", ex);
                    }
                }

                return false;
            }

            public void Explore(TestModel testModel, TestSource testSource, Action<ITest> consumer)
            {
                foreach (ITestExplorer explorer in explorers)
                {
                    try
                    {
                        explorer.Explore(testModel, testSource, consumer);
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("A test framework failed to explore tests.  It has been skipped.", ex);
                    }
                }
            }
        }
    }
}
