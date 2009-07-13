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
using System.Diagnostics;
using Gallio.Common.Concurrency;
using Gallio.Common.Messaging;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using Gallio.Model.Messages.Exploration;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model.Isolation;
using Gallio.Model.Messages.Execution;

namespace Gallio.Runner
{
    /// <summary>
    /// A default implementation of <see cref="ITestRunner" />.
    /// </summary>
    public class DefaultTestRunner : ITestRunner
    {
        private readonly ITestIsolationProvider testIsolationProvider;
        private readonly ITestFrameworkManager testFrameworkManager;
        private readonly TestRunnerEventDispatcher eventDispatcher;
        private readonly List<ITestRunnerExtension> extensions;

        private TappedLogger tappedLogger;
        private TestRunnerOptions testRunnerOptions;

        private State state;
        private ITestIsolationContext testIsolationContext;

        private enum State
        {
            Created,
            Initialized,
            Disposed
        }

        /// <summary>
        /// Creates a test runner.
        /// </summary>
        /// <param name="testIsolationProvider">The test isolation provider.</param>
        /// <param name="testFrameworkManager">The test framework manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testIsolationProvider"/> 
        /// or <paramref name="testFrameworkManager"/> is null.</exception>
        public DefaultTestRunner(ITestIsolationProvider testIsolationProvider, ITestFrameworkManager testFrameworkManager)
        {
            if (testIsolationProvider == null)
                throw new ArgumentNullException("testIsolationProvider");
            if (testFrameworkManager == null)
                throw new ArgumentNullException("testFrameworkManager");

            this.testIsolationProvider = testIsolationProvider;
            this.testFrameworkManager = testFrameworkManager;

            eventDispatcher = new TestRunnerEventDispatcher();
            state = State.Created;
            extensions = new List<ITestRunnerExtension>();
        }

        /// <summary>
        /// Gets the logger, or null if the test runner has not been initialized.
        /// </summary>
        protected ILogger Logger
        {
            get { return tappedLogger; }
        }

        /// <summary>
        /// Gets the test runner options, or null if the test runner has not been initialized.
        /// </summary>
        protected TestRunnerOptions TestRunnerOptions
        {
            get { return testRunnerOptions; }
        }

        /// <inheritdoc />
        public ITestRunnerEvents Events
        {
            get { return eventDispatcher; }
        }

        /// <inheritdoc />
        public void RegisterExtension(ITestRunnerExtension extension)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            ThrowIfDisposed();
            if (state != State.Created)
                throw new InvalidOperationException("Extensions cannot be registered after the test runner has been initialized.");

            foreach (ITestRunnerExtension currentExtension in extensions)
            {
                if (currentExtension.GetType() == extension.GetType()
                    && currentExtension.Parameters == extension.Parameters)
                    throw new InvalidOperationException(string.Format("There is already an extension of type '{0}' registered with parameters '{1}'.",
                        extension.GetType(), extension.Parameters));
            }

            extensions.Add(extension);
        }

        /// <inheritdoc />
        public void Initialize(TestRunnerOptions testRunnerOptions, ILogger logger, IProgressMonitor progressMonitor)
        {
            if (testRunnerOptions == null)
                throw new ArgumentNullException("testRunnerOptions");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state != State.Created)
                throw new InvalidOperationException("The test runner has already been initialized.");

            testRunnerOptions = testRunnerOptions.Copy();

            this.testRunnerOptions = testRunnerOptions;
            tappedLogger = new TappedLogger(this, logger);

            int extensionCount = extensions.Count;
            using (progressMonitor.BeginTask("Initializing the test runner.", 1 + extensionCount))
            {
                foreach (ITestRunnerExtension extension in extensions)
                {
                    string extensionName = extension.GetType().Name; // TODO: improve me

                    progressMonitor.SetStatus(String.Format("Installing extension '{0}'.", extensionName));
                    try
                    {
                        // Note: We don't pass the tapped logger to the extensions because the
                        //       extensions frequently write to the console a bunch of information we
                        //       already have represented in the report.  We are more interested in what
                        //       the test driver has to tell us.
                        extension.Install(eventDispatcher, logger);
                        progressMonitor.Worked(1);
                    }
                    catch (Exception ex)
                    {
                        throw new RunnerException(String.Format("Failed to install extension '{0}'.", extensionName), ex);
                    }
                    progressMonitor.SetStatus("");
                }

                try
                {
                    eventDispatcher.NotifyInitializeStarted(new InitializeStartedEventArgs(testRunnerOptions));

                    progressMonitor.SetStatus("Initializing the test isolation context.");

                    TestIsolationOptions testIsolationOptions = new TestIsolationOptions();
                    testIsolationOptions.Properties.AddAll(testRunnerOptions.Properties);
                    testIsolationContext = testIsolationProvider.CreateContext(testIsolationOptions, tappedLogger);

                    progressMonitor.Worked(1);
                }
                catch (Exception ex)
                {
                    eventDispatcher.NotifyInitializeFinished(new InitializeFinishedEventArgs(false));
                    throw new RunnerException("A fatal exception occurred while initializing the test isolation context.", ex);
                }

                state = State.Initialized;
                eventDispatcher.NotifyInitializeFinished(new InitializeFinishedEventArgs(true));
            }
        }

        /// <inheritdoc />
        public Report Explore(TestPackage testPackage, TestExplorationOptions testExplorationOptions, IProgressMonitor progressMonitor)
        {
            if (testPackage == null)
                throw new ArgumentNullException("testPackageConfig");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state != State.Initialized)
                throw new InvalidOperationException("The test runner must be initialized before this operation is performed.");

            testPackage = testPackage.Copy();
            testExplorationOptions = testExplorationOptions.Copy();

            using (progressMonitor.BeginTask("Exploring the tests.", 10))
            {
                Report report = new Report()
                {
                    TestPackage = new TestPackageData(testPackage),
                    TestModel = new TestModelData()
                };
                var reportLockBox = new LockBox<Report>(report);

                using (new LogReporter(reportLockBox, eventDispatcher))
                {
                    bool success;
                    eventDispatcher.NotifyExploreStarted(new ExploreStartedEventArgs(testPackage,
                        testExplorationOptions, reportLockBox));
                    try
                    {
                        using (Listener listener = new Listener(eventDispatcher, reportLockBox))
                        {
                            ITestDriver testDriver = testFrameworkManager.GetTestDriver(testPackage.IsFrameworkRequested);

                            using (testIsolationContext.BeginBatch(progressMonitor.SetStatus))
                            {
                                testDriver.Explore(testIsolationContext, testPackage, testExplorationOptions,
                                    listener, progressMonitor.CreateSubProgressMonitor(10));
                            }
                        }

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;

                        tappedLogger.Log(LogSeverity.Error,
                            "A fatal exception occurred while exploring tests.  Possible causes include invalid test runner parameters.",
                            ex);
                        report.TestModel.Annotations.Add(new AnnotationData(AnnotationType.Error,
                            CodeLocation.Unknown, CodeReference.Unknown, "A fatal exception occurred while exploring tests.  See log for details.", null));
                    }

                    eventDispatcher.NotifyExploreFinished(new ExploreFinishedEventArgs(success, report));
                }

                return report;
            }
        }

        /// <inheritdoc />
        public Report Run(TestPackage testPackage, TestExplorationOptions testExplorationOptions, TestExecutionOptions testExecutionOptions, IProgressMonitor progressMonitor)
        {
            if (testPackage == null)
                throw new ArgumentNullException("testPackageConfig");
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");
            if (testExecutionOptions == null)
                throw new ArgumentNullException("testExecutionOptions");
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");

            ThrowIfDisposed();
            if (state != State.Initialized)
                throw new InvalidOperationException("The test runner must be initialized before this operation is performed.");

            testPackage = testPackage.Copy();
            testExplorationOptions = testExplorationOptions.Copy();
            testExecutionOptions = testExecutionOptions.Copy();

            using (progressMonitor.BeginTask("Running the tests.", 10))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                Report report = new Report()
                {
                    TestPackage = new TestPackageData(testPackage),
                    TestModel = new TestModelData(),
                    TestPackageRun = new TestPackageRun()
                    {
                        StartTime = DateTime.Now
                    }
                };
                var reportLockBox = new LockBox<Report>(report);

                using (new LogReporter(reportLockBox, eventDispatcher))
                {
                    eventDispatcher.NotifyRunStarted(new RunStartedEventArgs(testPackage, testExplorationOptions,
                        testExecutionOptions, reportLockBox));

                    bool success;
                    try
                    {
                        using (Listener listener = new Listener(eventDispatcher, reportLockBox))
                        {
                            ITestDriver testDriver = testFrameworkManager.GetTestDriver(testPackage.IsFrameworkRequested);

                            using (testIsolationContext.BeginBatch(progressMonitor.SetStatus))
                            {
                                testDriver.Run(testIsolationContext, testPackage, testExplorationOptions,
                                    testExecutionOptions, listener, progressMonitor.CreateSubProgressMonitor(10));
                            }
                        }

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        success = false;

                        tappedLogger.Log(LogSeverity.Error,
                            "A fatal exception occurred while running tests.  Possible causes include invalid test runner parameters and stack overflows.",
                            ex);
                        report.TestModel.Annotations.Add(new AnnotationData(AnnotationType.Error,
                            CodeLocation.Unknown, CodeReference.Unknown, "A fatal exception occurred while running tests.  See log for details.", null));
                    }
                    finally
                    {
                        report.TestPackageRun.EndTime = DateTime.Now;
                        report.TestPackageRun.Statistics.Duration = stopwatch.Elapsed.TotalSeconds;
                    }

                    eventDispatcher.NotifyRunFinished(new RunFinishedEventArgs(success, report));
                }

                return report;
            }
        }

        /// <inheritdoc />
        public void Dispose(IProgressMonitor progressMonitor)
        {
            if (progressMonitor == null)
                throw new ArgumentNullException("progressMonitor");
            if (state == State.Disposed)
                return;

            using (progressMonitor.BeginTask("Disposing the test runner.", 10))
            {
                bool success;
                try
                {
                    eventDispatcher.NotifyDisposeStarted(new DisposeStartedEventArgs());

                    if (testIsolationContext != null)
                    {
                        progressMonitor.SetStatus("Disposing the test isolation context.");

                        testIsolationContext.Dispose();
                        testIsolationContext = null;
                    }

                    progressMonitor.Worked(10);
                    success = true;
                }
                catch (Exception ex)
                {
                    if (tappedLogger != null)
                        tappedLogger.Log(LogSeverity.Warning, "An exception occurred while disposing the test isolation context.  This may indicate that the test isolation context previously encountered another fault from which it could not recover.", ex);
                    success = false;
                }

                state = State.Disposed;
                eventDispatcher.NotifyDisposeFinished(new DisposeFinishedEventArgs(success));
            }
        }

        private void OnLog(LogSeverity logSeverity, string message, ExceptionData exceptionData)
        {
            eventDispatcher.NotifyLogMessage(new LogMessageEventArgs(logSeverity, message, exceptionData));
        }

        private void ThrowIfDisposed()
        {
            if (state == State.Disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private sealed class Listener : IMessageSink, IDisposable
        {
            private readonly TestRunnerEventDispatcher eventDispatcher;
            private readonly LockBox<Report> reportBox;
            private readonly MessageConsumer consumer;

            private Dictionary<string, TestStepState> states;

            private readonly Dictionary<string, string> redirectedStepIds;

            private TestStepData rootTestStepData;
            private TestResult rootTestStepResult;
            private Stopwatch rootTestStepStopwatch;

            public Listener(TestRunnerEventDispatcher eventDispatcher, LockBox<Report> reportBox)
            {
                this.eventDispatcher = eventDispatcher;
                this.reportBox = reportBox;

                states = new Dictionary<string, TestStepState>();

                redirectedStepIds = new Dictionary<string, string>();

                rootTestStepStopwatch = Stopwatch.StartNew();
                rootTestStepResult = new TestResult()
                {
                    Outcome = TestOutcome.Passed
                };

                consumer = new MessageConsumer()
                    .Handle<TestDiscoveredMessage>(HandleTestDiscoveredMessage)
                    .Handle<AnnotationDiscoveredMessage>(HandleAnnotationDiscoveredMessage)
                    .Handle<TestStepStartedMessage>(HandleTestStepStartedMessage)
                    .Handle<TestStepLifecyclePhaseChangedMessage>(HandleTestStepLifecyclePhaseChangedMessage)
                    .Handle<TestStepMetadataAddedMessage>(HandleTestStepMetadataAddedMessage)
                    .Handle<TestStepFinishedMessage>(HandleTestStepFinishedMessage)
                    .Handle<TestStepLogAttachMessage>(HandleTestStepLogAttachMessage)
                    .Handle<TestStepLogStreamWriteMessage>(HandleTestStepLogStreamWriteMessage)
                    .Handle<TestStepLogStreamEmbedMessage>(HandleTestStepLogStreamEmbedMessage)
                    .Handle<TestStepLogStreamBeginSectionBlockMessage>(HandleTestStepLogStreamBeginSectionBlockMessage)
                    .Handle<TestStepLogStreamBeginMarkerBlockMessage>(HandleTestStepLogStreamBeginMarkerBlockMessage)
                    .Handle<TestStepLogStreamEndBlockMessage>(HandleTestStepLogStreamEndBlockMessage);
            }

            public void Dispose()
            {
                reportBox.Write(report =>
                {
                    FinishRoot(report);

                    states = null;
                });
            }

            public void Publish(Message message)
            {
                message.Validate();

                eventDispatcher.NotifyMessageReceived(new MessageReceivedEventArgs(message));

                consumer.Consume(message);
            }

            public void HandleTestDiscoveredMessage(TestDiscoveredMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    TestData mergedTest = report.TestModel.MergeSubtree(message.ParentTestId, message.Test);

                    eventDispatcher.NotifyTestDiscovered(
                        new TestDiscoveredEventArgs(report, mergedTest));
                });
            }

            public void HandleAnnotationDiscoveredMessage(AnnotationDiscoveredMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    report.TestModel.Annotations.Add(message.Annotation);

                    eventDispatcher.NotifyAnnotationDiscovered(
                        new AnnotationDiscoveredEventArgs(report, message.Annotation));
                });
            }

            public void HandleTestStepStartedMessage(TestStepStartedMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    if (message.Step.ParentId == null)
                    {
                        if (IsRootStarted)
                        {
                            redirectedStepIds.Add(message.Step.Id, rootTestStepData.Id);
                        }
                        else
                        {
                            StartRoot(report, message.Step);
                        }
                    }
                    else
                    {
                        TestStepData step = RedirectParentIdOfStep(message.Step);
                        StartStep(report, step);
                    }
                });
            }

            public void HandleTestStepFinishedMessage(TestStepFinishedMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    if (redirectedStepIds.ContainsKey(message.StepId))
                    {
                        rootTestStepResult.AssertCount += message.Result.AssertCount;
                        rootTestStepResult.Outcome = rootTestStepResult.Outcome.CombineWith(message.Result.Outcome);
                    }
                    else
                    {
                        FinishStep(report, message.StepId, message.Result);
                    }
                });
            }

            private bool IsRootStarted
            {
                get { return rootTestStepData != null; }
            }

            private void StartRoot(Report report, TestStepData step)
            {
                rootTestStepData = step;

                StartStep(report, step);
            }

            private void FinishRoot(Report report)
            {
                if (rootTestStepData != null)
                {
                    rootTestStepResult.Duration = rootTestStepStopwatch.Elapsed.TotalSeconds;
                    FinishStep(report, rootTestStepData.Id, rootTestStepResult);
                    rootTestStepData = null;
                }
            }

            private void StartStep(Report report, TestStepData step)
            {
                TestData testData = GetTestData(report, step.TestId);
                TestStepRun testStepRun = new TestStepRun(step);
                testStepRun.StartTime = DateTime.Now;

                if (step.ParentId != null)
                {
                    TestStepState parentState = GetTestStepState(step.ParentId);
                    parentState.TestStepRun.Children.Add(testStepRun);
                }
                else
                {
                    report.TestPackageRun.RootTestStepRun = testStepRun;
                }

                TestStepState state = new TestStepState(testData, testStepRun);
                states.Add(step.Id, state);

                eventDispatcher.NotifyTestStepStarted(
                    new TestStepStartedEventArgs(report, testData, testStepRun));
            }

            private void FinishStep(Report report, string stepId, TestResult result)
            {
                TestStepState state = GetTestStepState(stepId);
                state.TestStepRun.EndTime = DateTime.Now;
                state.TestStepRun.Result = result;
                report.TestPackageRun.Statistics.MergeStepStatistics(state.TestStepRun);

                state.logWriter.Close();

                eventDispatcher.NotifyTestStepFinished(
                    new TestStepFinishedEventArgs(report, state.TestData, state.TestStepRun));
            }

            public void HandleTestStepLifecyclePhaseChangedMessage(TestStepLifecyclePhaseChangedMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);

                    eventDispatcher.NotifyTestStepLifecyclePhaseChanged(
                        new TestStepLifecyclePhaseChangedEventArgs(report, state.TestData, state.TestStepRun, message.LifecyclePhase));
                });
            }

            public void HandleTestStepMetadataAddedMessage(TestStepMetadataAddedMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.TestStepRun.Step.Metadata.Add(message.MetadataKey, message.MetadataValue);

                    eventDispatcher.NotifyTestStepMetadataAdded(
                        new TestStepMetadataAddedEventArgs(report, state.TestData, state.TestStepRun, message.MetadataKey, message.MetadataValue));
                });
            }

            public void HandleTestStepLogAttachMessage(TestStepLogAttachMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.logWriter.Attach(message.Attachment);

                    eventDispatcher.NotifyTestStepLogAttach(
                        new TestStepLogAttachEventArgs(report, state.TestData, state.TestStepRun, message.Attachment));
                });
            }

            public void HandleTestStepLogStreamWriteMessage(TestStepLogStreamWriteMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.logWriter[message.StreamName].Write(message.Text);

                    eventDispatcher.NotifyTestStepLogStreamWrite(
                        new TestStepLogStreamWriteEventArgs(report, state.TestData, state.TestStepRun, message.StreamName, message.Text));
                });
            }

            public void HandleTestStepLogStreamEmbedMessage(TestStepLogStreamEmbedMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.logWriter[message.StreamName].EmbedExisting(message.AttachmentName);

                    eventDispatcher.NotifyTestStepLogStreamEmbed(
                        new TestStepLogStreamEmbedEventArgs(report, state.TestData, state.TestStepRun, message.StreamName, message.AttachmentName));
                });
            }

            public void HandleTestStepLogStreamBeginSectionBlockMessage(TestStepLogStreamBeginSectionBlockMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.logWriter[message.StreamName].BeginSection(message.SectionName);

                    eventDispatcher.NotifyTestStepLogStreamBeginSectionBlock(
                        new TestStepLogStreamBeginSectionBlockEventArgs(report, state.TestData, state.TestStepRun, message.StreamName, message.SectionName));
                });
            }

            public void HandleTestStepLogStreamBeginMarkerBlockMessage(TestStepLogStreamBeginMarkerBlockMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.logWriter[message.StreamName].BeginMarker(message.Marker);

                    eventDispatcher.NotifyTestStepLogStreamBeginMarkerBlock(
                        new TestStepLogStreamBeginMarkerBlockEventArgs(report, state.TestData, state.TestStepRun, message.StreamName, message.Marker));
                });
            }

            public void HandleTestStepLogStreamEndBlockMessage(TestStepLogStreamEndBlockMessage message)
            {
                reportBox.Write(report =>
                {
                    ThrowIfDisposed();

                    string stepId = RedirectStepId(message.StepId);
                    TestStepState state = GetTestStepState(stepId);
                    state.logWriter[message.StreamName].End();

                    eventDispatcher.NotifyTestStepLogStreamEndBlock(
                        new TestStepLogStreamEndBlockEventArgs(report, state.TestData, state.TestStepRun, message.StreamName));
                });
            }

            private static TestData GetTestData(Report report, string testId)
            {
                TestData testData = report.TestModel.GetTestById(testId);
                if (testData == null)
                    throw new InvalidOperationException("The test id was not recognized.  It may belong to an earlier test run that has since completed.");
                return testData;
            }

            private TestStepState GetTestStepState(string testStepId)
            {
                TestStepState testStepData;
                if (!states.TryGetValue(testStepId, out testStepData))
                    throw new InvalidOperationException("The test step id was not recognized.  It may belong to an earlier test run that has since completed.");
                return testStepData;
            }

            private TestStepData RedirectParentIdOfStep(TestStepData step)
            {
                string targetStepId;
                if (step.ParentId != null && redirectedStepIds.TryGetValue(step.ParentId, out targetStepId))
                {
                    TestStepData targetStep = new TestStepData(step.Id, step.Name, step.FullName, step.TestId)
                    {
                        CodeLocation = step.CodeLocation,
                        CodeReference = step.CodeReference,
                        IsDynamic = step.IsDynamic,
                        IsPrimary = step.IsPrimary,
                        IsTestCase = step.IsTestCase,
                        Metadata = step.Metadata,
                        ParentId = targetStepId
                    };
                    return targetStep;
                }

                return step;
            }

            private string RedirectStepId(string stepId)
            {
                string targetStepId;
                if (redirectedStepIds.TryGetValue(stepId, out targetStepId))
                    return targetStepId;
                return stepId;
            }

            private void ThrowIfDisposed()
            {
                if (states == null)
                    throw new ObjectDisposedException(GetType().Name);
            }

            private sealed class TestStepState
            {
                public readonly TestData TestData;
                public readonly TestStepRun TestStepRun;
                public readonly StructuredDocumentWriter logWriter;

                public TestStepState(TestData testData, TestStepRun testStepRun)
                {
                    TestData = testData;
                    TestStepRun = testStepRun;

                    logWriter = new StructuredDocumentWriter();
                    testStepRun.TestLog = logWriter.Document;
                }
            }
        }

        private sealed class TappedLogger : BaseLogger
        {
            private readonly DefaultTestRunner runner;
            private readonly ILogger inner;

            public TappedLogger(DefaultTestRunner runner, ILogger inner)
            {
                this.runner = runner;
                this.inner = inner;
            }

            protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
            {
                inner.Log(severity, message, exceptionData);
                runner.OnLog(severity, message, exceptionData);
            }
        }

        private sealed class LogReporter : IDisposable
        {
            private readonly LockBox<Report> reportLockBox;
            private readonly TestRunnerEventDispatcher eventDispatcher;

            public LogReporter(LockBox<Report> reportLockBox, TestRunnerEventDispatcher eventDispatcher)
            {
                this.reportLockBox = reportLockBox;
                this.eventDispatcher = eventDispatcher;

                eventDispatcher.LogMessage += OnLog;
            }

            public void Dispose()
            {
                eventDispatcher.LogMessage -= OnLog;
            }

            private void OnLog(object sender, LogMessageEventArgs e)
            {
                reportLockBox.Write(report => report.AddLogEntry(new LogEntry()
                {
                    Severity = e.Severity,
                    Message = e.Message,
                    Details = e.ExceptionData != null ? e.ExceptionData.ToString() : null
                }));
            }
        }
    }
}
