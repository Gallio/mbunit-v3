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
using System.Text;
using System.Xml;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Diagnostics;
using Gallio.Model.Execution;
using Gallio.Model.Filters;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using Gallio.Runner;
using Gallio.Runner.Events;
using Gallio.Runner.Reports;
using Gallio.Runtime;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using JetBrains.ReSharper.TaskRunnerFramework;
using HashSetOfString = Gallio.Collections.HashSet<string>;

namespace Gallio.ReSharperRunner.Provider.Tasks
{
    /// <summary>
    /// This is the root task for running Gallio tests.
    /// It must always appear first in a task sequence followed by
    /// any number <see cref="GallioTestItemTask" /> instances that describe the work to
    /// be done.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Equality comparison is used by ReSharper to coalesce sequences of tasks into a tree.
    /// Sequential tasks form a chain of nested nodes.  When identical tasks are found they are
    /// combined and subsequent tasks in the sequence become children of the common ancestor.
    /// </para>
    /// <para>
    /// To ensure that we have full control over the node structure, we introduce a root task
    /// whose purpose is to gather all of the constituent tasks under a common parent.
    /// </para>
    /// </remarks>
    /// <todo author="jeff">
    /// Handle test steps.
    /// Handle metadata.
    /// Handle custom log streams and attachments.
    /// </todo>
    [Serializable]
    public class GallioTestRunTask : GallioRemoteTask, IEquatable<GallioTestRunTask>
    {
        /// <summary>
        /// Gets a shared instance of the task.
        /// </summary>
        public static readonly GallioTestRunTask Instance = new GallioTestRunTask();

        public GallioTestRunTask()
        {
        }

        public GallioTestRunTask(XmlElement element)
            : base(element)
        {
        }

        public bool Equals(GallioTestRunTask other)
        {
            return other != null;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GallioTestRunTask);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override TaskResult ExecuteRecursive(IRemoteTaskServer server, TaskExecutionNode node)
        {
            new Runner(server).Run(node);
            return TaskResult.Success;
        }

        private sealed class Runner
        {
            private readonly IRemoteTaskServer server;
            private readonly string sessionId;

            private readonly HashSetOfString assemblyLocations;
            private readonly Dictionary<string, GallioTestItemTask> testTasks;
            private readonly Dictionary<string, TestMonitor> testMonitors;
            private readonly HashSetOfString explicitTestIds;

            public Runner(IRemoteTaskServer server)
            {
                this.server = server;
                this.sessionId = GetSessionId(server);

                assemblyLocations = new HashSetOfString();
                testTasks = new Dictionary<string, GallioTestItemTask>();
                testMonitors = new Dictionary<string, TestMonitor>();
                explicitTestIds = new HashSetOfString();
            }

            public void Run(TaskExecutionNode node)
            {
                ProcessNodeRecursively(node);
                RunTests();
            }

            private void ProcessNodeRecursively(TaskExecutionNode node)
            {
                ProcessTask(node.RemoteTask);

                foreach (TaskExecutionNode childNode in node.Children)
                    ProcessNodeRecursively(childNode);
            }

            private void ProcessTask(RemoteTask task)
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

            private void RunTests()
            {
                ILogger logger = RuntimeAccessor.Logger;
                ITestRunner runner = TestRunnerUtils.CreateTestRunnerByName(StandardTestRunnerFactoryNames.Local);

                // Set parameters.
                TestPackageConfig packageConfig = new TestPackageConfig();
                packageConfig.AssemblyFiles.AddRange(assemblyLocations);

                TestRunnerOptions testRunnerOptions = new TestRunnerOptions();

                TestExplorationOptions testExplorationOptions = new TestExplorationOptions();

                TestExecutionOptions testExecutionOptions = new TestExecutionOptions();
                testExecutionOptions.Filter = new IdFilter<ITest>(new OrFilter<string>(GenericUtils.ConvertAllToArray<string, Filter<string>>(
                    explicitTestIds, delegate(string testId)
                    {
                        return new EqualityFilter<string>(testId);
                    })));

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
                        runner.Load(packageConfig, CreateProgressMonitor());
                        runner.Explore(testExplorationOptions, CreateProgressMonitor());
                        runner.Run(testExecutionOptions, CreateProgressMonitor());
                    }
                    finally
                    {
                        SubmitFailureForRemainingPendingTasks();

                        if (sessionId != null)
                        {
                            runner.Report.Read(report => SessionCache.SaveSerializedReport(sessionId, report));
                        }
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

            private static string GetSessionId(IRemoteTaskServer server)
            {
                // TODO: Should ask for a better way of doing this.
                object taskRunnerProxy = server.WithoutProxy;
#if RESHARPER_31
                PropertyInfo property = taskRunnerProxy.GetType().GetProperty("SessionId");
                return (string)property.GetValue(taskRunnerProxy, null);
#else
                return ((TaskRunnerProxy) taskRunnerProxy).SessionId;
#endif
            }
        }

        private sealed class TestMonitor
        {
            private readonly IRemoteTaskServer server;
            private readonly GallioTestItemTask testTask;

            private int stepCount;
            private int nestingCount;

            private readonly ExceptionVisitor exceptionVisitor;
            private readonly List<KeyValuePair<TaskOutputType, string>> combinedOutput;
            private readonly List<TaskException> pendingExceptions;
            private TestOutcome combinedOutcome;
            private string pendingWarnings;
            private string pendingFailures;
            private string pendingBanner;
            private bool finished;

            public TestMonitor(IRemoteTaskServer server, GallioTestItemTask testTask)
            {
                this.server = server;
                this.testTask = testTask;

                combinedOutcome = TestOutcome.Passed;
                combinedOutput = new List<KeyValuePair<TaskOutputType,string>>();
                pendingExceptions = new List<TaskException>();

                exceptionVisitor = new ExceptionVisitor(exception =>
                {
                    do
                    {
                        pendingExceptions.Add(TaskExceptionFactory.CreateTaskException(exception.Type, exception.Message,
                            exception.StackTrace.ToString()));
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
                        Output(TaskOutputType.STDOUT, banner);
                    else
                        pendingBanner = banner;

                    foreach (StructuredTestLogStream stream in run.TestLog.Streams)
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
                    Output(TaskOutputType.STDERR, "The test did not run or did not complete normally due to an error.\n" +
                        "Refer to the Gallio report for more details.");
                    combinedOutcome = TestOutcome.Error;

                    OutputPendingContents();
                    SubmitCombinedResult();
                }
            }

            private void OutputLogStreamContents(StructuredTestLogStream stream)
            {
                string contents = string.Concat("*** ", stream.Name, " ***\n", stream.ToString(), "\n");

                // ReSharper formats the TaskExplain contents only when the task result is of a particular value.
                // It will render it in a colored box based on the result code.
                // Unfortunately it can't really capture the richness of Gallio outcomes right now.
                switch (stream.Name)
                {
                    case TestLogStreamNames.ConsoleOutput:
                    default:
                        Output(TaskOutputType.STDOUT, contents);
                        break;

                    case TestLogStreamNames.ConsoleError:
                        Output(TaskOutputType.STDERR, contents);
                        break;

                    case TestLogStreamNames.DebugTrace:
                        Output(TaskOutputType.DEBUGTRACE, contents);
                        break;

                    case TestLogStreamNames.Warnings:
                        pendingWarnings = contents;
                        break;

                    case TestLogStreamNames.Failures:
                        pendingFailures = contents;
                        break;
                }
            }

            private void CaptureExceptions(StructuredTestLogStream stream)
            {
                stream.Body.Accept(exceptionVisitor);
            }

            private void Output(TaskOutputType outputType, string text)
            {
                combinedOutput.Add(new KeyValuePair<TaskOutputType, string>(outputType, text));
            }

            private void OutputPendingContents()
            {
                if (pendingBanner != null)
                {
                    combinedOutput.Insert(0, new KeyValuePair<TaskOutputType, string>(TaskOutputType.STDOUT, pendingBanner));
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
                    Output(TaskOutputType.STDERR, pendingWarnings);
                    pendingWarnings = null;
                }
            }

            private void OutputPendingFailures()
            {
                if (pendingFailures != null)
                {
                    Output(TaskOutputType.STDERR, pendingFailures);
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
                foreach (KeyValuePair<TaskOutputType, string> message in combinedOutput)
                    server.TaskOutput(testTask, message.Value, message.Key);

                TaskResult taskResult = GetTaskResultForOutcome(combinedOutcome);

                if (stepCount > 1 || taskResult != TaskResult.Skipped)
                    OutputPendingWarnings();
                else if (pendingWarnings != null)
                    server.TaskExplain(testTask, pendingWarnings);

                if (stepCount > 1 || taskResult != TaskResult.Error && taskResult != TaskResult.Exception)
                    OutputPendingFailures();
                else if (pendingFailures != null)
                    server.TaskExplain(testTask, pendingFailures);

                OutputPendingExceptions();

                server.TaskFinished(testTask, combinedOutcome.DisplayName, taskResult);
                finished = true;
            }

            private static TaskResult GetTaskResultForOutcome(TestOutcome outcome)
            {
                switch (outcome.Status)
                {
                    case TestStatus.Passed:
                        return TaskResult.Success;
                    case TestStatus.Failed:
                        return TaskResult.Error;
                    case TestStatus.Inconclusive: // FIXME: not very accurate
                    case TestStatus.Skipped:
                        return TaskResult.Skipped;
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
