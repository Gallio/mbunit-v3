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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Messaging;
using Gallio.Common.Policies;
using Gallio.Common.Reflection;
using Gallio.Model.Isolation;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.FileTypes;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

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
        public ITestDriver GetTestDriver(Predicate<string> frameworkIdFilter, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (frameworkIdFilter == null)
                return new FilteredTestDriver(frameworkHandles, fileTypeManager, logger);

            var filteredFrameworkHandles = new List<ComponentHandle<ITestFramework, TestFrameworkTraits>>();
            foreach (var frameworkHandle in frameworkHandles)
                if (frameworkIdFilter(frameworkHandle.Id))
                    filteredFrameworkHandles.Add(frameworkHandle);

            return new FilteredTestDriver(filteredFrameworkHandles, fileTypeManager, logger);
        }

        private sealed class FilteredTestDriver : BaseTestDriver
        {
            private readonly IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> frameworkHandles;
            private readonly IFileTypeManager fileTypeManager;
            private readonly ILogger logger;

            public FilteredTestDriver(IList<ComponentHandle<ITestFramework, TestFrameworkTraits>> frameworkHandles,
                IFileTypeManager fileTypeManager, ILogger logger)
            {
                this.frameworkHandles = frameworkHandles;
                this.fileTypeManager = fileTypeManager;
                this.logger = logger;
            }

            protected override bool IsTestImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                Action<ICodeElementInfo> unsupportedCodeElementHandler = null;

                return ForEachDriver(PartitionCodeElementsByFramework(new[] { codeElement }, unsupportedCodeElementHandler), (driver, items, driverCount) =>
                {
                    return driver.IsTest(reflectionPolicy, codeElement);
                });
            }

            protected override bool IsTestPartImpl(IReflectionPolicy reflectionPolicy, ICodeElementInfo codeElement)
            {
                Action<ICodeElementInfo> unsupportedCodeElementHandler = null;

                return ForEachDriver(PartitionCodeElementsByFramework(new[] { codeElement }, unsupportedCodeElementHandler), (driver, items, driverCount) =>
                {
                    return driver.IsTestPart(reflectionPolicy, codeElement);
                });
            }

            protected override void DescribeImpl(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements, TestExplorationOptions testExplorationOptions, Common.Messaging.IMessageSink messageSink, Gallio.Runtime.ProgressMonitoring.IProgressMonitor progressMonitor)
            {
                Action<ICodeElementInfo> unsupportedCodeElementHandler = null;

                using (progressMonitor.BeginTask("Describing tests.", 1))
                {
                    ForEachDriver(PartitionCodeElementsByFramework(codeElements, unsupportedCodeElementHandler), (driver, items, driverCount) =>
                    {
                        driver.Describe(reflectionPolicy, items, testExplorationOptions, messageSink,
                            progressMonitor.CreateSubProgressMonitor(1.0 / driverCount));
                        return false;
                    });
                }
            }

            protected override void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage, TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
            {
                Action<FileInfo> unsupportedFileHandler = file => AddUnsupportedFileAsAnnotation(file, messageSink);

                using (progressMonitor.BeginTask("Exploring tests.", 1))
                {
                    ForEachDriver(PartitionFilesByFramework(testPackage.Files, unsupportedFileHandler), (driver, items, driverCount) =>
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
                Action<FileInfo> unsupportedFileHandler = file => AddUnsupportedFileAsAnnotation(file, messageSink);

                using (progressMonitor.BeginTask("Running tests.", 1))
                {
                    if (testIsolationContext.RequiresSingleThreadedExecution && ! testExecutionOptions.SingleThreaded)
                    {
                        testExecutionOptions = testExecutionOptions.Copy();
                        testExecutionOptions.SingleThreaded = true;
                    }

                    ForEachDriver(PartitionFilesByFramework(testPackage.Files, unsupportedFileHandler), (driver, items, driverCount) =>
                    {
                        TestPackage testPackageForDriver = CreateTestPackageWithFiles(testPackage, items);
                        driver.Run(testIsolationContext, testPackageForDriver, testExplorationOptions, testExecutionOptions,
                            messageSink,
                            progressMonitor.CreateSubProgressMonitor(1.0 / driverCount));
                        return false;
                    });
                }
            }

            private static void AddUnsupportedFileAsAnnotation(FileInfo file, IMessageSink messageSink)
            {
                messageSink.Publish(new AnnotationDiscoveredMessage()
                {
                    Annotation = new AnnotationData(AnnotationType.Warning,
                        new CodeLocation(file.FullName, 0, 0),
                        CodeReference.Unknown,
                        string.Format("File '{0}' is not supported by any installed test framework.  It will be ignored.", file.Name), null)
                });
            }

            private static TestPackage CreateTestPackageWithFiles(TestPackage testPackage, IList<FileInfo> files)
            {
                TestPackage result = testPackage.Copy();
                result.ClearFiles();
                foreach (var file in files)
                    result.AddFile(file);
                return result;
            }

            private bool ForEachDriver<T>(MultiMap<ComponentHandle<ITestFramework, TestFrameworkTraits>, T> frameworkPartitions,
                Func<ITestDriver, IList<T>, int, bool> func)
            {
                var driverFactoryPartitions = new MultiMap<TestDriverFactory, ComponentHandle<ITestFramework, TestFrameworkTraits>>();
                var driverItemPartitions = new MultiMap<TestDriverFactory, T>();

                foreach (var frameworkPartition in frameworkPartitions)
                {
                    if (frameworkPartition.Value.Count != 0)
                    {
                        TestDriverFactory driverFactory = frameworkPartition.Key.GetComponent().GetTestDriverFactory();

                        driverFactoryPartitions.Add(driverFactory, frameworkPartition.Key);

                        foreach (var item in frameworkPartition.Value)
                            driverItemPartitions.Add(driverFactory, item);
                    }
                }

                int driverCount = driverFactoryPartitions.Count;

                foreach (var driverPartition in driverFactoryPartitions)
                {
                    var frameworks = driverPartition.Value;
                    var items = driverItemPartitions[driverPartition.Key];

                    try
                    {
                        TestDriverFactory driverFactory = driverPartition.Key;
                        ITestDriver driver = driverFactory(frameworks, logger);
                        if (func(driver, items, driverCount))
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

            private MultiMap<ComponentHandle<ITestFramework, TestFrameworkTraits>, ICodeElementInfo>
                PartitionCodeElementsByFramework(IEnumerable<ICodeElementInfo> codeElements,
                Action<ICodeElementInfo> unsupportedCodeElementHandler)
            {
                var partitions = new MultiMap<ComponentHandle<ITestFramework, TestFrameworkTraits>, ICodeElementInfo>();

                foreach (var codeElement in codeElements)
                {
                    IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
                    if (assembly != null)
                    {
                        IList<AssemblyName> assemblyReferences = assembly.GetReferencedAssemblies();

                        bool supported = false;
                        foreach (var frameworkHandle in frameworkHandles)
                        {
                            if (frameworkHandle.GetTraits().IsFrameworkCompatibleWithAssemblyReferences(assemblyReferences))
                            {
                                partitions.Add(frameworkHandle, codeElement);
                                supported = true;
                            }
                        }

                        if (!supported && unsupportedCodeElementHandler != null)
                            unsupportedCodeElementHandler(codeElement);
                    }
                }

                return partitions;
            }

            private MultiMap<ComponentHandle<ITestFramework, TestFrameworkTraits>, FileInfo> PartitionFilesByFramework(IEnumerable<FileInfo> files,
                Action<FileInfo> unsupportedFileHandler)
            {
                var partitions = new MultiMap<ComponentHandle<ITestFramework, TestFrameworkTraits>, FileInfo>();

                foreach (var file in files)
                {
                    try
                    {
                        bool supported = false;
                        using (LazyFileInspector fileInspector = new LazyFileInspector(file))
                        {
                            FileType fileType = fileTypeManager.IdentifyFileType(fileInspector);
                            IList<AssemblyName> assemblyReferences = null;

                            foreach (var frameworkHandle in frameworkHandles)
                            {
                                TestFrameworkTraits traits = frameworkHandle.GetTraits();
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

                                partitions.Add(frameworkHandle, file);
                                supported = true;
                            }
                        }

                        if (!supported && unsupportedFileHandler != null)
                            unsupportedFileHandler(file);
                    }
                    catch (IOException)
                    {
                        // Ignore the file.
                    }
                }

                return partitions;
            }
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
    }
}
