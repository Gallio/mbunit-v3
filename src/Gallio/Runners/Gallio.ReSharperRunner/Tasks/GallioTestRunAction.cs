// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Core.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner;
using Gallio.Runner.Monitors;
using Gallio.Runner.Reports;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Gallio.ReSharperRunner.Tasks
{
    /// <summary>
    /// Runs tests.
    /// </summary>
    /// <todo author="jeff">
    /// Handle test instances.
    /// Handle test steps.
    /// Handle metadata.
    /// Handle custom log streams and attachments.
    /// </todo>
    internal class GallioTestRunAction : GallioRemoteAction
    {
        private IRemoteTaskServer server;

        private HashSet<string> assemblyLocations;
        private Dictionary<string, GallioTestItemTask> testTasks;
        private HashSet<string> explicitTestIds;

        public override TaskResult ExecuteRecursive(IRemoteTaskServer server, TaskExecutionNode node)
        {
            this.server = server;

            assemblyLocations = new HashSet<string>();
            testTasks = new Dictionary<string, GallioTestItemTask>();
            explicitTestIds = new HashSet<string>();

            ProcessNodeRecursively(node);
            RunTests();

            return TaskResult.Success;
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
            using (ITestRunner runner = TestRunnerFactory.CreateLocalTestRunner())
            {
                // Set parameters.
                TestPackageConfig packageConfig = new TestPackageConfig();
                packageConfig.AssemblyFiles.AddRange(assemblyLocations);

                runner.TestExecutionOptions.Filter = new IdFilter<ITest>(new OrFilter<string>(GenericUtils.ConvertAllToArray<string, Filter<string>>(
                    explicitTestIds, delegate(string testId)
                    {
                        return new EqualityFilter<string>(testId);
                    })));

                // Install the listeners.
                ReportMonitor reportMonitor = new ReportMonitor();
                reportMonitor.Attach(runner);
                reportMonitor.TestStepStarting += TestStepStarting;
                reportMonitor.TestStepFinished += TestStepFinished;

                // Run the tests.
                runner.LoadTestPackage(packageConfig, CreateProgressMonitor());
                runner.BuildTestModel(CreateProgressMonitor());
                runner.RunTests(CreateProgressMonitor());
            }
        }

        private void TestStepStarting(object sender, TestStepRunEventArgs e)
        {
            GallioTestItemTask testTask;
            if (testTasks.TryGetValue(e.TestData.Id, out testTask))
            {
                server.TaskStarting(testTask);
            }
        }

        private void TestStepFinished(object sender, TestStepRunEventArgs e)
        {
            GallioTestItemTask testTask;
            if (testTasks.TryGetValue(e.TestData.Id, out testTask))
            {
                foreach (ExecutionLogStream stream in e.TestStepRun.ExecutionLog.Streams)
                    SubmitLogStreamContents(testTask, stream);

                SubmitTestResult(testTask, e.TestStepRun.Result);
            }
        }

        private void SubmitLogStreamContents(GallioTestItemTask testTask, ExecutionLogStream stream)
        {
            switch (stream.Name)
            {
                case LogStreamNames.ConsoleOutput:
                default:
                    server.TaskOutput(testTask, stream.ToString(), TaskOutputType.STDOUT);
                    break;

                case LogStreamNames.ConsoleError:
                    server.TaskOutput(testTask, stream.ToString(), TaskOutputType.STDERR);
                    break;

                case LogStreamNames.DebugTrace:
                    server.TaskOutput(testTask, stream.ToString(), TaskOutputType.DEBUGTRACE);
                    break;

                case LogStreamNames.Warnings:
                    server.TaskExplain(testTask, stream.ToString());
                    break;
            
                case LogStreamNames.Failures:
                    server.TaskError(testTask, stream.ToString());
                    break;
            }
        }

        private void SubmitTestResult(GallioTestItemTask testTask, TestResult result)
        {
            switch (result.Status)
            {
                case TestStatus.Canceled:
                    server.TaskFinished(testTask, "The test was canceled.", TaskResult.Skipped);
                    break;

                case TestStatus.Error:
                    server.TaskFinished(testTask, "The test was aborted due to an error.", TaskResult.Error);
                    break;

                case TestStatus.Ignored:
                    server.TaskFinished(testTask, "The test was ignored.", TaskResult.Skipped);
                    break;

                case TestStatus.NotRun:
                    server.TaskFinished(testTask, "The test was not run.", TaskResult.Skipped);
                    break;

                case TestStatus.Skipped:
                    server.TaskFinished(testTask, "The test was skipped.", TaskResult.Skipped);
                    break;

                case TestStatus.Executed:
                    switch (result.Outcome)
                    {
                        case TestOutcome.Passed:
                            server.TaskFinished(testTask, "The test passed.", TaskResult.Success);
                            break;

                        case TestOutcome.Failed:
                            server.TaskFinished(testTask, "The test failed.", TaskResult.Exception);
                            break;

                        case TestOutcome.Inconclusive:
                            server.TaskFinished(testTask, "The test yielded an inconclusive result.", TaskResult.Skipped); // FIXME
                            break;
                    }
                    break;
            }
        }

        private static IProgressMonitor CreateProgressMonitor()
        {
            return new NullProgressMonitor();
        }
    }
}
