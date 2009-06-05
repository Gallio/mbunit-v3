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
using Gallio.Common.Collections;
using Gallio.Common.Policies;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;

namespace Gallio.Model
{
    /// <summary>
    /// Default implementation of a test framework manager.
    /// </summary>
    public class DefaultTestFrameworkManager : ITestFrameworkManager
    {
        private readonly ComponentHandle<ITestFramework, TestFrameworkTraits>[] frameworkHandles;
        private readonly IFileTypeManager fileTypeManager;

        /// <summary>
        /// Creates a test framework manager.
        /// </summary>
        /// <param name="frameworkHandles">The test framework handles.</param>
        /// <param name="fileTypeManager">The file type manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frameworkHandles"/> 
        /// or <paramref name="fileTypeManager "/>is null.</exception>
        public DefaultTestFrameworkManager(ComponentHandle<ITestFramework, TestFrameworkTraits>[] frameworkHandles,
            IFileTypeManager fileTypeManager)
        {
            if (frameworkHandles == null || Array.IndexOf(frameworkHandles, null) >= 0)
                throw new ArgumentNullException("frameworkHandles");
            if (fileTypeManager == null)
                throw new ArgumentNullException("fileTypeManager");

            this.frameworkHandles = frameworkHandles;
            this.fileTypeManager = fileTypeManager;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> FrameworkHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<ITestFramework, TestFrameworkTraits>>(frameworkHandles); }
        }

        /// <inheritdoc />
        public ITestExplorer GetTestExplorer(Predicate<string> frameworkIdFilter)
        {
            if (frameworkIdFilter == null)
                return new FilteredTestExplorer(frameworkHandles, fileTypeManager);

            var filteredFrameworkHandles = new List<ComponentHandle<ITestFramework, TestFrameworkTraits>>();
            foreach (var frameworkHandle in frameworkHandles)
                if (frameworkIdFilter(frameworkHandle.Id))
                    filteredFrameworkHandles.Add(frameworkHandle);

            return new FilteredTestExplorer(filteredFrameworkHandles, fileTypeManager);
        }

        private sealed class FilteredTestExplorer : ITestExplorer
        {
            private readonly IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> frameworkHandles;
            private readonly IFileTypeManager fileTypeManager;

            public FilteredTestExplorer(IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> frameworkHandles,
                IFileTypeManager fileTypeManager)
            {
                this.frameworkHandles = frameworkHandles;
                this.fileTypeManager = fileTypeManager;
            }

            public void ConfigureTestDomain(TestDomainSetup testDomainSetup)
            {
                if (testDomainSetup == null)
                    throw new ArgumentNullException("testDomainSetup");

                var aggregateTestExplorer = new AggregateTestExplorer();
                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    if (frameworkTraits.RequiresConfigureTestDomain)
                        aggregateTestExplorer.RegisterFramework(frameworkHandle.GetComponent());
                }

                aggregateTestExplorer.ConfigureTestDomain(testDomainSetup);
            }

            public bool IsTest(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                if (reflectionPolicy == null)
                    throw new ArgumentNullException("reflectionPolicy");
                if (codeElement == null)
                    throw new ArgumentNullException("codeElement");

                var aggregateTestExplorer = CreateAggregateTestExplorerForCodeElement(codeElement);
                return aggregateTestExplorer.IsTest(reflectionPolicy, codeElement);
            }

            public bool IsTestPart(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                if (reflectionPolicy == null)
                    throw new ArgumentNullException("reflectionPolicy");
                if (codeElement == null)
                    throw new ArgumentNullException("codeElement");

                var aggregateTestExplorer = CreateAggregateTestExplorerForCodeElement(codeElement);
                return aggregateTestExplorer.IsTestPart(reflectionPolicy, codeElement);
            }

            public void Explore(TestModel testModel, TestSource testSource, Action<ITest> consumer)
            {
                if (testModel == null)
                    throw new ArgumentNullException("testModel");
                if (testSource == null)
                    throw new ArgumentNullException("testSource");

                var aggregateTestExplorer = CreateAggregateTestExplorerForTestSource(testSource);
                aggregateTestExplorer.Explore(testModel, testSource, consumer);
            }

            private AggregateTestExplorer CreateAggregateTestExplorerForTestSource(TestSource testSource)
            {
                var aggregateTestExplorer = new AggregateTestExplorer();

                /* TODO: Needs to be redesigned to work from a higher level.
                var frameworksToFileTypes = new MultiMap<TestFrameworkTraits, FileType>();
                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    foreach (string fileTypeId in frameworkTraits.FileTypes)
                    {
                        FileType fileType = fileTypeManager.GetFileTypeById(fileTypeId);
                        if (fileType != null)
                            frameworksToFileTypes.Add(frameworkTraits, fileType);
                    }
                }

                var frameworksToFiles = new MultiMap<TestFrameworkTraits, FileInfo>();
                foreach (var file in testSource.Files)
                {
                    FileType candidateFileType = IdentifyFileType(file);
                    foreach (var pair in frameworksToFileTypes)
                    {
                        if (ContainsFileType(pair.Value, candidateFileType))
                            frameworksToFiles.Add(pair.Key, file);
                    }
                }
                 */

                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    if (/*frameworksToFiles.ContainsKey(frameworkTraits)
                        ||*/ IsFrameworkNeededForAssembliesOrTypes(frameworkTraits, testSource))
                    {
                        aggregateTestExplorer.RegisterFramework(frameworkHandle.GetComponent());
                    }
                }

                return aggregateTestExplorer;
            }

            private FileType IdentifyFileType(FileInfo file)
            {
                try
                {
                    return fileTypeManager.IdentifyFileType(file);
                }
                catch (IOException)
                {
                    return fileTypeManager.UnknownFileType;
                }
            }

            private static bool ContainsFileType(IEnumerable<FileType> fileTypes, FileType candidateFileType)
            {
                foreach (FileType fileType in fileTypes)
                {
                    if (candidateFileType.IsSameOrSubtypeOf(fileType))
                        return true;
                }

                return false;
            }

            private AggregateTestExplorer CreateAggregateTestExplorerForCodeElement(ICodeElementInfo codeElement)
            {
                var aggregateTestExplorer = new AggregateTestExplorer();

                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
                if (assembly != null)
                    PopulateAggregateTestExplorerByFrameworkAssembly(aggregateTestExplorer, assembly);

                return aggregateTestExplorer;
            }

            private void PopulateAggregateTestExplorerByFrameworkAssembly(AggregateTestExplorer aggregateTestExplorer, IAssemblyInfo assembly)
            {
                foreach (var frameworkHandle in frameworkHandles)
                {
                    TestFrameworkTraits frameworkTraits = frameworkHandle.GetTraits();
                    if (ContainsAssemblyReference(assembly, frameworkTraits.FrameworkAssemblies))
                        aggregateTestExplorer.RegisterFramework(frameworkHandle.GetComponent());
                }
            }

            private static bool IsFrameworkNeededForAssembliesOrTypes(TestFrameworkTraits frameworkTraits, TestSource testSource)
            {
                foreach (IAssemblyInfo assembly in testSource.Assemblies)
                {
                    if (ContainsAssemblyReference(assembly, frameworkTraits.FrameworkAssemblies))
                        return true;
                }

                foreach (ITypeInfo type in testSource.Types)
                {
                    if (ContainsAssemblyReference(type.Assembly, frameworkTraits.FrameworkAssemblies))
                        return true;
                }

                return false;
            }

            private static bool ContainsAssemblyReference(IAssemblyInfo assembly, AssemblySignature[] assemblySignatures)
            {
                foreach (AssemblyName referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    foreach (AssemblySignature assemblySignature in assemblySignatures)
                    {
                        if (assemblySignature.IsMatch(referencedAssemblyName))
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
