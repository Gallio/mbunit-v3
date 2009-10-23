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
using System.IO;
using Gallio.Common.Markup;
using Gallio.Common.Messaging;
using Gallio.Common.Reflection;
using Gallio.Model.Isolation;
using Gallio.Model.Messages;
using Gallio.Model.Messages.Execution;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Model
{
    /// <summary>
    /// A test driver implementation used when no test framework supports a given test file.
    /// </summary>
    public class FallbackTestDriver : BaseTestDriver
    {
        private readonly TestFrameworkOptions testFrameworkOptions;

        /// <summary>
        /// Creates the fallback test driver.
        /// </summary>
        /// <param name="testFrameworkOptions">The test framework options.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFrameworkOptions"/> is null.</exception>
        public FallbackTestDriver(TestFrameworkOptions testFrameworkOptions)
        {
            if (testFrameworkOptions == null)
                throw new ArgumentNullException("testFrameworkOptions");

            this.testFrameworkOptions = testFrameworkOptions;
        }

        /// <summary>
        /// Gets the fallback explanation.
        /// </summary>
        public string FallbackExplanation
        {
            get
            {
                return testFrameworkOptions.Properties.GetValue(FallbackTestFramework.FallbackExplanationKey)
                    ?? "This test is not supported by any available test framework.";
            }
        }

        /// <inheritdoc />
        protected override void DescribeImpl(IReflectionPolicy reflectionPolicy, IList<ICodeElementInfo> codeElements,
            TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Describing unsupported tests.", Math.Max(codeElements.Count, 1)))
            {
                PublishTestModelFromCodeElements(codeElements, messageSink, progressMonitor);
            }
        }

        /// <inheritdoc />
        protected override void ExploreImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage,
            TestExplorationOptions testExplorationOptions, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            using (progressMonitor.BeginTask("Exploring unsupported tests.", Math.Max(testPackage.Files.Count, 1)))
            {
                PublishTestModelFromFiles(testPackage.Files, messageSink, progressMonitor);
            }
        }

        /// <inheritdoc />
        protected override void RunImpl(ITestIsolationContext testIsolationContext, TestPackage testPackage,
            TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions,
            IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            double workItems = Math.Max(testPackage.Files.Count, 1);
            using (progressMonitor.BeginTask("Running unsupported tests.", workItems * 2))
            {
                TestModel testModel = PublishTestModelFromFiles(testPackage.Files, messageSink, progressMonitor);

                if (testModel != null)
                    RunTestModel(testModel, messageSink, progressMonitor, workItems);
            }
        }

        private TestModel PublishTestModelFromCodeElements(IEnumerable<ICodeElementInfo> codeElements, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            TestModel testModel = new TestModel();
            var tests = new Dictionary<ICodeElementInfo, Test>();

            foreach (ICodeElementInfo codeElement in codeElements)
            {
                if (progressMonitor.IsCanceled)
                    return null;

                testModel.AddAnnotation(new Annotation(AnnotationType.Warning, codeElement, FallbackExplanation));

                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
                if (assembly != null)
                {
                    Test assemblyTest;
                    if (!tests.TryGetValue(assembly, out assemblyTest))
                    {
                        assemblyTest = CreateTest(Path.GetFileName(assembly.Path), assembly);
                        testModel.RootTest.AddChild(assemblyTest);
                        tests.Add(assembly, assemblyTest);

                        ITypeInfo type = ReflectionUtils.GetType(codeElement);
                        if (type != null)
                        {
                            Test typeTest;
                            if (!tests.TryGetValue(type, out typeTest))
                            {
                                typeTest = CreateTest(type.Name, type);
                                assemblyTest.AddChild(typeTest);
                                tests.Add(type, typeTest);
                            }
                        }
                    }
                }

                progressMonitor.Worked(1);
            }

            TestModelSerializer.PublishTestModel(testModel, messageSink);
            return testModel;
        }

        private TestModel PublishTestModelFromFiles(IEnumerable<FileInfo> files, IMessageSink messageSink, IProgressMonitor progressMonitor)
        {
            TestModel testModel = new TestModel();

            foreach (FileInfo file in files)
            {
                if (progressMonitor.IsCanceled)
                    return null;

                messageSink.Publish(new AnnotationDiscoveredMessage()
                {
                    Annotation = new AnnotationData(AnnotationType.Warning,
                        new CodeLocation(file.FullName, 0, 0),
                        CodeReference.Unknown,
                        string.Format("File '{0}' is not supported by any installed test framework.  It will be ignored.", file.Name), null)
                });

                Test fileTest = CreateTest(file.Name, null);
                fileTest.Metadata.Add(MetadataKeys.File, file.FullName);
                testModel.RootTest.AddChild(fileTest);

                progressMonitor.Worked(1);
            }

            TestModelSerializer.PublishTestModel(testModel, messageSink);
            return testModel;
        }

        private void RunTestModel(TestModel testModel, IMessageSink messageSink, IProgressMonitor progressMonitor, double totalWork)
        {
            double workUnitsPerStep = totalWork / (1 + testModel.RootTest.Children.Count);
            TestStep rootTestStep = new TestStep(testModel.RootTest, null);

            messageSink.Publish(new TestStepStartedMessage()
            {
                Step = new TestStepData(rootTestStep)
            });

            foreach (Test test in testModel.RootTest.Children)
            {
                if (progressMonitor.IsCanceled)
                    return;

                TestStep testStep = new TestStep(test, rootTestStep);

                messageSink.Publish(new TestStepStartedMessage()
                {
                    Step = new TestStepData(testStep)
                });

                messageSink.Publish(new TestStepLogStreamWriteMessage()
                {
                    StepId = testStep.Id,
                    StreamName = MarkupStreamNames.Warnings,
                    Text = FallbackExplanation
                });

                messageSink.Publish(new TestStepFinishedMessage()
                {
                    StepId = testStep.Id,
                    Result = new TestResult(TestOutcome.Ignored)
                });

                progressMonitor.Worked(workUnitsPerStep);
            }

            messageSink.Publish(new TestStepFinishedMessage()
            {
                StepId = rootTestStep.Id,
                Result = new TestResult(TestOutcome.Skipped)
            });

            progressMonitor.Worked(workUnitsPerStep);
        }

        private Test CreateTest(string testName, ICodeElementInfo codeElement)
        {
            var test = new Test(testName, codeElement);
            test.Kind = TestKinds.Unsupported;
            test.Metadata.Add(MetadataKeys.IgnoreReason, FallbackExplanation);
            return test;
        }
    }
}
