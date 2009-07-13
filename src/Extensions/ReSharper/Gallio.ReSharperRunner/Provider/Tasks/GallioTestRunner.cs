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
using System.Text;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Model.Filters;
using Gallio.Common.Markup;
using Gallio.Common.Markup.Tags;
using Gallio.Model.Schema;
using Gallio.ReSharperRunner.Provider.Facade;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using HashSetOfString = Gallio.Common.Collections.HashSet<string>;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// Gallio test runner for ReSharper.
    /// </summary>
    /// <todo author="jeff">
    /// Handle test steps.
    /// Handle metadata.
    /// Handle custom log streams and attachments.
    /// </todo>
    /// <remarks>
    /// This type is decoupled from the ReSharper interfaces by means of a proxy facade.
    /// </remarks>
    internal sealed class GallioTestRunner
    {
        private readonly IFacadeTaskServer server;
        private readonly IFacadeLogger logger;
        private readonly FacadeTaskExecutorConfiguration config;
        private readonly string sessionId;

        private readonly HashSetOfString assemblyLocations;
        private readonly Dictionary<string, GallioTestItemTask> testTasks;
        private readonly Dictionary<string, TestMonitor> testMonitors;
        private readonly HashSetOfString explicitTestIds;

        public GallioTestRunner(IFacadeTaskServer server, IFacadeLogger logger, FacadeTaskExecutorConfiguration config)
        {
            this.server = server;
            this.logger = logger;
            this.config = config;
            sessionId = server.SessionId;

            assemblyLocations = new HashSetOfString();
            testTasks = new Dictionary<string, GallioTestItemTask>();
            testMonitors = new Dictionary<string, TestMonitor>();
            explicitTestIds = new HashSetOfString();
        }

        public FacadeTaskResult Run(FacadeTask facadeTask)
        {
            RecursiveProcessTask(facadeTask);
            return RunTests();
        }

        private void RecursiveProcessTask(FacadeTask task)
        {
            ProcessTask(task);

            foreach (FacadeTask child in task.Children)
                RecursiveProcessTask(child);
        }

        private void ProcessTask(FacadeTask task)
        {
            GallioTestItemTask itemTask = task as GallioTestItemTask;
            if (itemTask != null)
            {
                testTasks[itemTask.TestId] = itemTask;
                return;
            }

            GallioTestAssemblyTask assemblyTask = task as GallioTestAssemblyTask;
            if (assemblyTask != null)
            {
                assemblyLocations.Add(assemblyTask.AssemblyLocation);
                return;
            }

            GallioTestExplicitTask explicitTask = task as GallioTestExplicitTask;
            if (explicitTask != null)
            {
                explicitTestIds.Add(explicitTask.TestId);
                return;
            }
        }

        private FacadeTaskResult RunTests()
        {
            ITestRunner runner = TestRunnerUtils.CreateTestRunnerByName(StandardTestRunnerFactoryNames.IsolatedAppDomain);
            ILogger logger = new FacadeLoggerWrapper(this.logger);

            // Set parameters.
            TestPackage testPackage = new TestPackage();
            foreach (string assemblyLocation in assemblyLocations)
                testPackage.AddFile(new FileInfo(assemblyLocation));
            testPackage.ShadowCopy = config.ShadowCopy;
            testPackage.ApplicationBaseDirectory = new DirectoryInfo(config.AssemblyFolder);
            testPackage.WorkingDirectory = new DirectoryInfo(config.AssemblyFolder);

            TestRunnerOptions testRunnerOptions = new TestRunnerOptions();

            TestExplorationOptions testExplorationOptions = new TestExplorationOptions();

            TestExecutionOptions testExecutionOptions = new TestExecutionOptions();
            testExecutionOptions.FilterSet = new FilterSet<ITestDescriptor>(new IdFilter<ITestDescriptor>(new OrFilter<string>(GenericCollectionUtils.ConvertAllToArray<string, Filter<string>>(
                explicitTestIds, delegate(string testId)
                {
                    return new EqualityFilter<string>(testId);
                }))));

            // Install the listeners.
            runner.Events.TestStepStarted += TestStepStarted;
            runner.Events.TestStepFinished += TestStepFinished;
            runner.Events.TestStepLifecyclePhaseChanged += TestStepLifecyclePhaseChanged;

            // Run the tests.
            try
            {
                try
                {
                    runner.Initialize(testRunnerOptions, logger, CreateProgressMonitor());
                    Report report = runner.Run(testPackage, testExplorationOptions, testExecutionOptions, CreateProgressMonitor());

                    if (sessionId != null)
                        SessionCache.SaveSerializedReport(sessionId, report);

                    return FacadeTaskResult.Success;
                }
                catch (Exception ex)
                {
                    if (sessionId != null)
                        SessionCache.ClearSerializedReport(sessionId);

                    logger.Log(LogSeverity.Error, "A fatal exception occurred during test execution.", ex);
                    return FacadeTaskResult.Exception;
                }
                finally
                {
                    SubmitFailureForRemainingPendingTasks();
                }
            }
            finally
            {
                runner.Dispose(CreateProgressMonitor());
            }
        }

        private void SubmitFailureForRemainingPendingTasks()
        {
            foreach (string testId in testTasks.Keys)
            {
                TestMonitor testMonitor = GetTestMonitor(testId);
                testMonitor.SubmitFailureIfNotFinished();
            }
        }

        private void TestStepStarted(object sender, TestStepStartedEventArgs e)
        {
            TestMonitor testMonitor = GetTestMonitor(e.Test.Id);
            if (testMonitor != null)
                testMonitor.TestStepStarted(e);
        }

        private void TestStepLifecyclePhaseChanged(object sender, TestStepLifecyclePhaseChangedEventArgs e)
        {
            TestMonitor testMonitor = GetTestMonitor(e.Test.Id);
            if (testMonitor != null)
                testMonitor.TestStepLifecyclePhaseChanged(e);
        }

        private void TestStepFinished(object sender, TestStepFinishedEventArgs e)
        {
            TestMonitor testMonitor = GetTestMonitor(e.Test.Id);
            if (testMonitor != null)
                testMonitor.TestStepFinished(e);
        }

        private TestMonitor GetTestMonitor(string testId)
        {
            TestMonitor testMonitor;
            lock (testMonitors)
            {
                if (!testMonitors.TryGetValue(testId, out testMonitor))
                {
                    GallioTestItemTask testTask;
                    if (testTasks.TryGetValue(testId, out testTask))
                    {
                        testMonitor = new TestMonitor(server, testTask);
                        testMonitors.Add(testId, testMonitor);
                    }
                }
            }

            return testMonitor;
        }

        private static IProgressMonitor CreateProgressMonitor()
        {
            return NullProgressMonitor.CreateInstance();
        }

        private sealed class TestMonitor
        {
            private readonly IFacadeTaskServer server;
            private readonly GallioTestItemTask testTask;

            private int stepCount;
            private int nestingCount;

            private readonly ExceptionVisitor exceptionVisitor;
            private readonly List<KeyValuePair<FacadeTaskOutputType, string>> combinedOutput;
            private readonly List<FacadeTaskException> pendingExceptions;
            private TestOutcome combinedOutcome;
            private string pendingWarnings;
            private string pendingFailures;
            private string pendingBanner;
            private bool finished;

            public TestMonitor(IFacadeTaskServer server, GallioTestItemTask testTask)
            {
                this.server = server;
                this.testTask = testTask;

                combinedOutcome = TestOutcome.Passed;
                combinedOutput = new List<KeyValuePair<FacadeTaskOutputType, string>>();
                pendingExceptions = new List<FacadeTaskException>();

                exceptionVisitor = new ExceptionVisitor(exception =>
                {
                    do
                    {
                        pendingExceptions.Add(new FacadeTaskException(
                            exception.Type, exception.Message, exception.StackTrace.ToString()));
                        exception = exception.InnerException;
                    } while (exception != null);
                });
            }

            public void TestStepStarted(TestStepStartedEventArgs e)
            {
                lock (this)
                {
                    nestingCount += 1;
                    server.TaskStarting(testTask);
                }
            }

            public void TestStepLifecyclePhaseChanged(TestStepLifecyclePhaseChangedEventArgs e)
            {
                lock (this)
                {
                    string message = e.LifecyclePhase;
                    if (!e.TestStepRun.Step.IsPrimary)
                        message += " - " + e.TestStepRun.Step.Name;

                    server.TaskProgress(testTask, message);
                }
            }

            public void TestStepFinished(TestStepFinishedEventArgs e)
            {
                lock (this)
                {
                    server.TaskProgress(testTask, "");

                    nestingCount -= 1;
                    stepCount += 1;

                    OutputPendingContents();

                    TestStepRun run = e.TestStepRun;

                    if (run.Step.IsPrimary)
                        combinedOutcome = combinedOutcome.CombineWith(run.Result.Outcome);

                    string banner = String.Format("### Step {0}: {1} ###\n\n", run.Step.Name, run.Result.Outcome.DisplayName);
                    if (stepCount != 1)
                        Output(FacadeTaskOutputType.StandardOutput, banner);
                    else
                        pendingBanner = banner;

                    foreach (StructuredStream stream in run.TestLog.Streams)
                    {
                        OutputLogStreamContents(stream);
                        CaptureExceptions(stream);
                    }

                    if (nestingCount == 0)
                        SubmitCombinedResult();
                }
            }

            public void SubmitFailureIfNotFinished()
            {
                if (!finished)
                {
                    Output(FacadeTaskOutputType.StandardError, "The test did not run or did not complete normally due to an error.\n" +
                        "Refer to the Gallio Test Report and ReSharper Log for more details.");
                    combinedOutcome = TestOutcome.Error;

                    OutputPendingContents();
                    SubmitCombinedResult();
                }
            }

            private void OutputLogStreamContents(StructuredStream stream)
            {
                string contents = string.Concat("*** ", stream.Name, " ***\n", stream.ToString(), "\n");

                // ReSharper formats the TaskExplain contents only when the task result is of a particular value.
                // It will render it in a colored box based on the result code.
                // Unfortunately it can't really capture the richness of Gallio outcomes right now.
                switch (stream.Name)
                {
                    case MarkupStreamNames.ConsoleOutput:
                    default:
                        Output(FacadeTaskOutputType.StandardOutput, contents);
                        break;

                    case MarkupStreamNames.ConsoleError:
                        Output(FacadeTaskOutputType.StandardError, contents);
                        break;

                    case MarkupStreamNames.DebugTrace:
                        Output(FacadeTaskOutputType.DebugTrace, contents);
                        break;

                    case MarkupStreamNames.Warnings:
                        pendingWarnings = contents;
                        break;

                    case MarkupStreamNames.Failures:
                        pendingFailures = contents;
                        break;
                }
            }

            private void CaptureExceptions(StructuredStream stream)
            {
                stream.Body.Accept(exceptionVisitor);
            }

            private void Output(FacadeTaskOutputType outputType, string text)
            {
                combinedOutput.Add(new KeyValuePair<FacadeTaskOutputType, string>(outputType, text));
            }

            private void OutputPendingContents()
            {
                if (pendingBanner != null)
                {
                    combinedOutput.Insert(0, new KeyValuePair<FacadeTaskOutputType, string>(FacadeTaskOutputType.StandardOutput, pendingBanner));
                    pendingBanner = null;
                }

                // We cannot report pending warnings/failures from prior steps using TaskExplain.
                OutputPendingWarnings();
                OutputPendingFailures();
            }

            private void OutputPendingWarnings()
            {
                if (pendingWarnings != null)
                {
                    Output(FacadeTaskOutputType.StandardError, pendingWarnings);
                    pendingWarnings = null;
                }
            }

            private void OutputPendingFailures()
            {
                if (pendingFailures != null)
                {
                    Output(FacadeTaskOutputType.StandardError, pendingFailures);
                    pendingFailures = null;
                }
            }

            private void OutputPendingExceptions()
            {
                if (pendingExceptions.Count != 0)
                {
                    server.TaskException(testTask, pendingExceptions.ToArray());
                    pendingExceptions.Clear();
                }
            }

            private void SubmitCombinedResult()
            {
                foreach (KeyValuePair<FacadeTaskOutputType, string> message in combinedOutput)
                    server.TaskOutput(testTask, message.Value, message.Key);

                FacadeTaskResult taskResult = GetTaskResultForOutcome(combinedOutcome);

                if (stepCount > 1 || taskResult != FacadeTaskResult.Skipped)
                    OutputPendingWarnings();
                else if (pendingWarnings != null)
                    server.TaskExplain(testTask, pendingWarnings);

                if (stepCount > 1 || taskResult != FacadeTaskResult.Error && taskResult != FacadeTaskResult.Exception)
                    OutputPendingFailures();
                else if (pendingFailures != null)
                    server.TaskExplain(testTask, pendingFailures);

                OutputPendingExceptions();

                server.TaskFinished(testTask, combinedOutcome.DisplayName, taskResult);
                finished = true;
            }

            private static FacadeTaskResult GetTaskResultForOutcome(TestOutcome outcome)
            {
                switch (outcome.Status)
                {
                    case TestStatus.Passed:
                        return FacadeTaskResult.Success;
                    case TestStatus.Failed:
                        return FacadeTaskResult.Error;
                    case TestStatus.Inconclusive: // FIXME: not very accurate
                    case TestStatus.Skipped:
                        return FacadeTaskResult.Skipped;
                    default:
                        throw new ArgumentException("outcome");
                }
            }
        }

        private sealed class ExceptionVisitor : BaseTagVisitor
        {
            private readonly Action<ExceptionData> publishException;

            private ExceptionDataBuilder currentBuilder;
            private string currentMarkerClass;
            private string currentSectionName;

            public ExceptionVisitor(Action<ExceptionData> publishException)
            {
                this.publishException = publishException;
            }

            public override void VisitMarkerTag(MarkerTag tag)
            {
                string oldMarkerClass = currentMarkerClass;

                if (IsRecognizedMarkerClass(tag.Class))
                    currentMarkerClass = tag.Class;

                if (currentMarkerClass == Marker.ExceptionClass ||
                    currentBuilder == null && currentMarkerClass == Marker.StackTraceClass)
                {
                    ExceptionDataBuilder oldBuilder = currentBuilder;
                    currentBuilder = new ExceptionDataBuilder();

                    base.VisitMarkerTag(tag);

                    // Handle the case where the stack trace is not part of an exception but appears
                    // within a section that provides additional information since we don't have an exception
                    // type or anything else.  In particular, this is the case for AssertionFailures.
                    if (currentBuilder.Message.Length == 0 && currentSectionName != null)
                        currentBuilder.Message.Append(currentSectionName);

                    ExceptionData currentException = currentBuilder.ToExceptionData();
                    if (oldBuilder != null)
                        oldBuilder.Inner = currentException;
                    else
                        publishException(currentException);

                    currentBuilder = oldBuilder;
                }
                else
                {
                    base.VisitMarkerTag(tag);
                }

                currentMarkerClass = oldMarkerClass;
            }

            private static bool IsRecognizedMarkerClass(string markerClass)
            {
                switch (markerClass)
                {
                    case Marker.ExceptionClass:
                    case Marker.StackTraceClass:
                    case Marker.ExceptionMessageClass:
                    case Marker.ExceptionTypeClass:
                        return true;

                    default:
                        return false;
                }
            }

            public override void VisitSectionTag(SectionTag tag)
            {
                string oldSectionName = currentSectionName;
                currentSectionName = tag.Name;

                base.VisitSectionTag(tag);

                currentSectionName = oldSectionName;
            }

            public override void VisitTextTag(TextTag tag)
            {
                if (currentBuilder == null)
                    return;

                switch (currentMarkerClass)
                {
                    case Marker.ExceptionTypeClass:
                        currentBuilder.Type.Append(tag.Text);
                        break;

                    case Marker.ExceptionMessageClass:
                        currentBuilder.Message.Append(tag.Text);
                        break;

                    case Marker.StackTraceClass:
                        currentBuilder.StackTrace.Append(tag.Text);
                        break;
                }
            }
        }

        private sealed class ExceptionDataBuilder
        {
            public ExceptionDataBuilder()
            {
                Type = new StringBuilder();
                Message = new StringBuilder();
                StackTrace = new StringBuilder();
            }

            public StringBuilder Type { get; private set; }
            public StringBuilder Message { get; private set; }
            public StringBuilder StackTrace { get; private set; }
            public ExceptionData Inner { get; set; }

            public ExceptionData ToExceptionData()
            {
                return new ExceptionData(Type.ToString(), Message.ToString(), StackTrace.ToString(), Inner);
            }
        }
    }
}
