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
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Messaging;
using Gallio.Common.Reflection;
using Gallio.Model.Isolation;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using System.Text;

namespace Gallio.Model
{
    /// <summary>
    /// Default implementation of a test framework manager.
    /// </summary>
    public class DefaultTestFrameworkManager : ITestFrameworkManager
    {
        private static readonly object DefaultFallbackSelectionKey = new object();

        private readonly ComponentHandle<ITestFramework, TestFrameworkTraits>[] testFrameworkHandles;
        private readonly List<ComponentHandle<ITestFramework, TestFrameworkTraits>> testFrameworkHandlesWithoutFallback;
        private readonly ComponentHandle<ITestFramework, TestFrameworkTraits> fallbackTestFrameworkHandle;
        private readonly IFileTypeManager fileTypeManager;

        /// <summary>
        /// Creates a test framework manager.
        /// </summary>
        /// <param name="testFrameworkHandles">The test framework handles.</param>
        /// <param name="fallbackTestFrameworkHandle">The fallback test framework handle.</param>
        /// <param name="fileTypeManager">The file type manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFrameworkHandles"/>,
        /// <paramref name="fallbackTestFrameworkHandle"/> or <paramref name="fileTypeManager "/>is null.</exception>
        public DefaultTestFrameworkManager(ComponentHandle<ITestFramework, TestFrameworkTraits>[] testFrameworkHandles,
            ComponentHandle<ITestFramework, TestFrameworkTraits> fallbackTestFrameworkHandle,
            IFileTypeManager fileTypeManager)
        {
            if (testFrameworkHandles == null || Array.IndexOf(testFrameworkHandles, null) >= 0)
                throw new ArgumentNullException("testFrameworkHandles");
            if (fallbackTestFrameworkHandle == null)
                throw new ArgumentNullException("fallbackTestFrameworkHandle");
            if (fileTypeManager == null)
                throw new ArgumentNullException("fileTypeManager");

            this.testFrameworkHandles = testFrameworkHandles;
            this.fallbackTestFrameworkHandle = fallbackTestFrameworkHandle;
            this.fileTypeManager = fileTypeManager;

            testFrameworkHandlesWithoutFallback = new List<ComponentHandle<ITestFramework, TestFrameworkTraits>>(testFrameworkHandles.Length);
            foreach (var testFrameworkHandle in testFrameworkHandles)
                if (testFrameworkHandle.Id != fallbackTestFrameworkHandle.Id)
                    testFrameworkHandlesWithoutFallback.Add(testFrameworkHandle);
        }

        /// <inheritdoc />
        public IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> TestFrameworkHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<ITestFramework, TestFrameworkTraits>>(testFrameworkHandles); }
        }

        /// <inheritdoc />
        public ComponentHandle<ITestFramework, TestFrameworkTraits> FallbackTestFrameworkHandle
        {
            get { return fallbackTestFrameworkHandle; }
        }

        /// <inheritdoc />
        public ITestDriver GetTestDriver(TestFrameworkSelector selector, ILogger logger)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (logger == null)
                throw new ArgumentNullException("logger");

            return new FilteredTestDriver(this,
                GetFilteredTestFrameworkHandlesWithoutFallback(selector.Filter),
                selector.FallbackMode, selector.Options, logger);
        }

        /// <inheritdoc />
        public IMultiMap<TestFrameworkSelection, FileInfo> SelectTestFrameworksForFiles(TestFrameworkSelector selector, ICollection<FileInfo> files)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (files == null || files.Contains(null))
                throw new ArgumentNullException("files");

            return SelectTestFrameworksForFilesImpl(GetFilteredTestFrameworkHandlesWithoutFallback(selector.Filter),
                selector.FallbackMode, selector.Options, files);
        }

        /// <inheritdoc />
        public IMultiMap<TestFrameworkSelection, ICodeElementInfo> SelectTestFrameworksForCodeElements(TestFrameworkSelector selector, ICollection<ICodeElementInfo> codeElements)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");
            if (codeElements == null || codeElements.Contains(null))
                throw new ArgumentNullException("codeElements");

            return SelectTestFrameworksForCodeElementsImpl(GetFilteredTestFrameworkHandlesWithoutFallback(selector.Filter),
                selector.FallbackMode, selector.Options, codeElements);
        }

        private IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> GetFilteredTestFrameworkHandlesWithoutFallback(
            Predicate<ComponentHandle<ITestFramework, TestFrameworkTraits>> filter)
        {
            if (filter == null)
                return testFrameworkHandlesWithoutFallback;

            var filteredTestFrameworkHandlesWithoutFallback = new List<ComponentHandle<ITestFramework, TestFrameworkTraits>>(testFrameworkHandles.Length);
            foreach (var testFrameworkHandle in testFrameworkHandlesWithoutFallback)
                if (filter(testFrameworkHandle))
                    filteredTestFrameworkHandlesWithoutFallback.Add(testFrameworkHandle);
            return filteredTestFrameworkHandlesWithoutFallback;
        }

        private MultiMap<TestFrameworkSelection, ICodeElementInfo>
            SelectTestFrameworksForCodeElementsImpl(
            IEnumerable<ComponentHandle<ITestFramework, TestFrameworkTraits>> filteredTestFrameworkHandlesWithoutFallback,
            TestFrameworkFallbackMode testFrameworkFallbackMode,
            TestFrameworkOptions testFrameworkOptions,
            IEnumerable<ICodeElementInfo> codeElements)
        {
            var selections = new Dictionary<object, TestFrameworkSelection>();
            var partitions = new MultiMap<TestFrameworkSelection, ICodeElementInfo>();

            foreach (var codeElement in codeElements)
            {
                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
                if (assembly != null)
                {
                    IList<AssemblyName> assemblyReferences = assembly.GetReferencedAssemblies();

                    bool supported = false;
                    foreach (var testFrameworkHandle in filteredTestFrameworkHandlesWithoutFallback)
                    {
                        if (testFrameworkHandle.GetTraits().IsFrameworkCompatibleWithAssemblyReferences(assemblyReferences))
                        {
                            TestFrameworkSelection selection = GetOrCreateSelectionIfAbsent(
                                selections, testFrameworkHandle,
                                () => new TestFrameworkSelection(testFrameworkHandle, testFrameworkOptions, false));

                            partitions.Add(selection, codeElement);
                            supported = true;
                        }
                    }

                    if (!supported)
                    {
                        TestFrameworkSelection selection = GetFallbackSelection(selections, assemblyReferences,
                            filteredTestFrameworkHandlesWithoutFallback, testFrameworkOptions, testFrameworkFallbackMode);
                        if (selection != null)
                            partitions.Add(selection, codeElement);
                    }
                }
            }

            return partitions;
        }

        private MultiMap<TestFrameworkSelection, FileInfo>
            SelectTestFrameworksForFilesImpl(
            IEnumerable<ComponentHandle<ITestFramework, TestFrameworkTraits>> filteredTestFrameworkHandlesWithoutFallback,
            TestFrameworkFallbackMode testFrameworkFallbackMode,
            TestFrameworkOptions testFrameworkOptions,
            IEnumerable<FileInfo> files)
        {
            var selections = new Dictionary<object, TestFrameworkSelection>();
            var partitions = new MultiMap<TestFrameworkSelection, FileInfo>();

            foreach (var file in files)
            {
                try
                {
                    bool supported = false;
                    IList<AssemblyName> assemblyReferences = null;
                    using (var fileInspector = new LazyFileInspector(file))
                    {
                        FileType fileType = fileTypeManager.IdentifyFileType(fileInspector);

                        foreach (var testFrameworkHandle in filteredTestFrameworkHandlesWithoutFallback)
                        {
                            TestFrameworkTraits traits = testFrameworkHandle.GetTraits();
                            if (!traits.IsFrameworkCompatibleWithFileType(fileType, fileTypeManager))
                                continue;

                            // If the file is an assembly, then filter further by assembly references.
                            if (fileType.IsSameOrSubtypeOf(fileTypeManager.GetFileTypeById("Assembly")))
                            {
                                if (assemblyReferences == null)
                                    assemblyReferences = GetAssemblyReferences(fileInspector);

                                if (!traits.IsFrameworkCompatibleWithAssemblyReferences(assemblyReferences))
                                    continue;
                            }

                            TestFrameworkSelection selection = GetOrCreateSelectionIfAbsent(
                                selections, testFrameworkHandle,
                                () => new TestFrameworkSelection(testFrameworkHandle, testFrameworkOptions, false));

                            partitions.Add(selection, file);
                            supported = true;
                        }
                    }

                    if (!supported)
                    {
                        TestFrameworkSelection selection = GetFallbackSelection(selections, assemblyReferences,
                            filteredTestFrameworkHandlesWithoutFallback, testFrameworkOptions, testFrameworkFallbackMode);
                        if (selection != null)
                            partitions.Add(selection, file);
                    }
                }
                catch (IOException)
                {
                    // Ignore the file.
                }
            }

            return partitions;
        }

        private TestFrameworkSelection GetFallbackSelection(
            Dictionary<object, TestFrameworkSelection> selections,
            IList<AssemblyName> assemblyReferences,
            IEnumerable<ComponentHandle<ITestFramework, TestFrameworkTraits>> filteredTestFrameworkHandlesWithoutFallback,
            TestFrameworkOptions testFrameworkOptions,
            TestFrameworkFallbackMode testFrameworkFallbackMode)
        {
            // Strict fallback mode.
            if (testFrameworkFallbackMode == TestFrameworkFallbackMode.Strict)
                return null;

            // Approximate fallback mode.
            if (assemblyReferences != null)
            {
                HashSet<string> matchingReferences = null;
                HashSet<string> matchingSignatures = null;

                foreach (var testFrameworkHandle in filteredTestFrameworkHandlesWithoutFallback)
                {
                    IList<AssemblySignature> assemblySignatures = testFrameworkHandle.GetTraits().FrameworkAssemblies;

                    foreach (AssemblyName assemblyName in assemblyReferences)
                    {
                        foreach (AssemblySignature assemblySignature in assemblySignatures)
                        {
                            if (assemblySignature.Name == assemblyName.Name)
                            {
                                if (matchingReferences == null)
                                {
                                    matchingReferences = new HashSet<string>();
                                    matchingSignatures = new HashSet<string>();
                                }

                                matchingReferences.Add(assemblyName.FullName);
                                matchingSignatures.Add(assemblySignature.ToString());
                            }
                        }
                    }
                }

                if (matchingReferences != null)
                {
                    StringBuilder fallbackExplanationBuilder = new StringBuilder();

                    fallbackExplanationBuilder.Append("Detected a probable test framework assembly version mismatch.\n");

                    fallbackExplanationBuilder.Append("Referenced test frameworks: ");
                    AppendQuotedItems(fallbackExplanationBuilder, matchingReferences);
                    fallbackExplanationBuilder.Append(".\n");

                    fallbackExplanationBuilder.Append("Supported test frameworks: ");
                    AppendQuotedItems(fallbackExplanationBuilder, matchingSignatures);
                    fallbackExplanationBuilder.Append(".");

                    string fallbackExplanation = fallbackExplanationBuilder.ToString();
                    return GetOrCreateSelectionIfAbsent(selections, fallbackExplanation, () =>
                    {
                        TestFrameworkOptions fallbackTestFrameworkOptions = testFrameworkOptions.Copy();
                        fallbackTestFrameworkOptions.AddProperty(FallbackTestFramework.FallbackExplanationKey,
                            fallbackExplanation);
                        return new TestFrameworkSelection(fallbackTestFrameworkHandle, fallbackTestFrameworkOptions,
                            true);
                    });
                }
            }

            if (testFrameworkFallbackMode == TestFrameworkFallbackMode.Approximate)
                return null;

            // Default fallback mode.
            return GetOrCreateSelectionIfAbsent(selections, DefaultFallbackSelectionKey,
                () => new TestFrameworkSelection(fallbackTestFrameworkHandle, testFrameworkOptions, true));
        }

        private static void AppendQuotedItems(StringBuilder builder, ICollection<string> items)
        {
            bool first = true;
            foreach (string item in items)
            {
                if (first)
                    first = false;
                else
                    builder.Append(", ");

                builder.Append("'").Append(item).Append("'");
            }
        }

        private static TestFrameworkSelection GetOrCreateSelectionIfAbsent(
            Dictionary<object, TestFrameworkSelection> selections,
            object key, Func<TestFrameworkSelection> factory)
        {
            TestFrameworkSelection selection;
            if (!selections.TryGetValue(key, out selection))
            {
                selection = factory();
                selections.Add(key, selection);
            }

            return selection;
        }

        private static IList<AssemblyName> GetAssemblyReferences(IFileInspector fileInspector)
        {
            Stream stream;
            if (fileInspector.TryGetStream(out stream))
            {
                AssemblyMetadata assemblyMetadata = AssemblyUtils.GetAssemblyMetadata(stream, AssemblyMetadataFields.AssemblyReferences);
                if (assemblyMetadata != null)
                    return assemblyMetadata.AssemblyReferences;
            }

            return EmptyArray<AssemblyName>.Instance;
        }

        private sealed class FilteredTestDriver : BaseTestDriver
        {
            private readonly DefaultTestFrameworkManager testFrameworkManager;
            private readonly IEnumerable<ComponentHandle<ITestFramework, TestFrameworkTraits>> testFrameworkHandles;
            private readonly TestFrameworkFallbackMode testFrameworkFallbackMode;
            private readonly TestFrameworkOptions testFrameworkOptions;
            private readonly ILogger logger;

            public FilteredTestDriver(
                DefaultTestFrameworkManager testFrameworkManager,
                IEnumerable<ComponentHandle<ITestFramework, TestFrameworkTraits>> testFrameworkHandles,
                TestFrameworkFallbackMode testFrameworkFallbackMode, TestFrameworkOptions testFrameworkOptions,
                ILogger logger)
            {
                this.testFrameworkManager = testFrameworkManager;
                this.testFrameworkHandles = testFrameworkHandles;
                this.testFrameworkFallbackMode = testFrameworkFallbackMode;
                this.testFrameworkOptions = testFrameworkOptions;
                this.logger = logger;
            }

            protected override bool IsTestImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                return ForEachDriver(testFrameworkManager.SelectTestFrameworksForCodeElementsImpl(
                    testFrameworkHandles, testFrameworkFallbackMode, testFrameworkOptions, new[] { codeElement }),
                    (driver, items, driverCount) =>
                {
                    return driver.IsTest(reflectionPolicy, codeElement);
                });
            }

            protected override bool IsTestPartImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                return ForEachDriver(testFrameworkManager.SelectTestFrameworksForCodeElementsImpl(
                    testFrameworkHandles, testFrameworkFallbackMode, testFrameworkOptions, new[] { codeElement }),
                    (driver, items, driverCount) =>
                {
                    return driver.IsTestPart(reflectionPolicy, codeElement);
                });
            }

            protected override void DescribeImpl(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements, TestExplorationOptions testExplorationOptions, Common.Messaging.IMessageSink messageSink, Gallio.Runtime.ProgressMonitoring.IProgressMonitor progressMonitor)
            {
                using (progressMonitor.BeginTask("Describing tests.", 1))
                {
                    ForEachDriver(testFrameworkManager.SelectTestFrameworksForCodeElementsImpl(
                        testFrameworkHandles, testFrameworkFallbackMode, testFrameworkOptions, codeElements),
                        (driver, items, driverCount) =>
                    {
                        driver.Describe(reflectionPolicy, items, testExplorationOptions, messageSink,
                            progressMonitor.CreateSubProgressMonitor(1.0 / driverCount));
                        return false;
                    });
                }
            }

            protected override void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
            {
                using (progressMonitor.BeginTask("Exploring tests.", 1))
                {
                    ForEachDriver(testFrameworkManager.SelectTestFrameworksForFilesImpl(
                        testFrameworkHandles, testFrameworkFallbackMode, testFrameworkOptions, testPackage.Files),
                        (driver, items, driverCount) =>
                    {
                        TestPackage testPackageForDriver = CreateTestPackageWithFiles(testPackage, items);
                        driver.Explore(testIsolationContext, testPackageForDriver, testExplorationOptions, messageSink,
                            progressMonitor.CreateSubProgressMonitor(1.0 / driverCount));
                        return false;
                    });
                }
            }

            protected override void RunImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
            {
                using (progressMonitor.BeginTask("Running tests.", 1))
                {
                    if (testIsolationContext.RequiresSingleThreadedExecution && !testExecutionOptions.SingleThreaded)
                    {
                        testExecutionOptions = testExecutionOptions.Copy();
                        testExecutionOptions.SingleThreaded = true;
                    }

                    ForEachDriver(testFrameworkManager.SelectTestFrameworksForFilesImpl(
                        testFrameworkHandles, testFrameworkFallbackMode, testFrameworkOptions, testPackage.Files),
                        (driver, items, driverCount) =>
                    {
                        TestPackage testPackageForDriver = CreateTestPackageWithFiles(testPackage, items);
                        driver.Run(testIsolationContext, testPackageForDriver, testExplorationOptions, testExecutionOptions,
                            messageSink,
                            progressMonitor.CreateSubProgressMonitor(1.0 / driverCount));
                        return false;
                    });
                }
            }

            private bool ForEachDriver<T>(MultiMap<TestFrameworkSelection, T> testFrameworkPartitions,
                Func<ITestDriver, IList<T>, int, bool> func)
                where T : class
            {
                var testDriverPartitions = new Dictionary<TestDriverFactory, TestDriverData<T>>();

                foreach (var testFrameworkPartition in testFrameworkPartitions)
                {
                    if (testFrameworkPartition.Value.Count != 0)
                    {
                        TestFrameworkSelection testFrameworkSelection = testFrameworkPartition.Key;
                        ComponentHandle<ITestFramework, TestFrameworkTraits> testFrameworkHandle = testFrameworkSelection.TestFrameworkHandle;
                        TestDriverFactory testDriverFactory = testFrameworkHandle.GetComponent().GetTestDriverFactory();

                        TestDriverData<T> testDriverData;
                        if (!testDriverPartitions.TryGetValue(testDriverFactory, out testDriverData))
                        {
                            testDriverData = new TestDriverData<T>(testFrameworkSelection.TestFrameworkOptions);
                            testDriverPartitions.Add(testDriverFactory, testDriverData);
                        }
                        else
                        {
                            testDriverData.ItemIndex = new HashSet<T>(ReferentialEqualityComparer<T>.Instance);
                            GenericCollectionUtils.AddAll(testDriverData.Items, testDriverData.ItemIndex);
                        }

                        testDriverData.TestFrameworkHandles.Add(testFrameworkHandle);

                        foreach (var item in testFrameworkPartition.Value)
                        {
                            if (testDriverData.ItemIndex == null)
                            {
                                testDriverData.Items.Add(item);
                            }
                            else if (! testDriverData.ItemIndex.Contains(item))
                            {
                                testDriverData.Items.Add(item);
                                testDriverData.ItemIndex.Add(item);
                            }
                        }
                    }
                }

                int testDriverCount = testDriverPartitions.Count;

                foreach (var testDriverPartition in testDriverPartitions)
                {
                    TestDriverFactory testDriverFactory = testDriverPartition.Key;
                    TestDriverData<T> testDriverData = testDriverPartition.Value;

                    try
                    {
                        ITestDriver driver = testDriverFactory(testDriverData.TestFrameworkHandles,
                            testDriverData.TestFrameworkOptions, logger);
                        if (func(driver, testDriverData.Items, testDriverCount))
                            return true;
                    }
                    catch (Exception ex)
                    {
                        //UnhandledExceptionPolicy.Report("An unhandled exception occurred while invoking a test driver.", ex);
                        throw new ModelException("An exception occurred while invoking a test driver.", ex);
                    }
                }

                return false;
            }

            private static TestPackage CreateTestPackageWithFiles(TestPackage testPackage, IList<FileInfo> files)
            {
                TestPackage result = testPackage.Copy();
                result.ClearFiles();
                foreach (var file in files)
                    result.AddFile(file);
                return result;
            }
        }

        private sealed class TestDriverData<T>
            where T : class
        {
            public TestDriverData(TestFrameworkOptions testFrameworkOptions)
            {
                TestFrameworkHandles = new List<ComponentHandle<ITestFramework, TestFrameworkTraits>>();
                TestFrameworkOptions = testFrameworkOptions;
                Items = new List<T>();
            }

            public readonly List<ComponentHandle<ITestFramework, TestFrameworkTraits>> TestFrameworkHandles;
            public readonly TestFrameworkOptions TestFrameworkOptions;
            public readonly List<T> Items;

            public HashSet<T> ItemIndex;
        }
    }
}
