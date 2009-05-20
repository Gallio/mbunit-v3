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
using System.Text;
using Gallio.Collections;
using Gallio.Model;
using Gallio.Model.Logging;
using Gallio.Model.Serialization;
using Gallio.Runner.Events;
using Gallio.Runner.Extensions;
using Gallio.Runner.Reports;
using Gallio.Runtime.Logging;
using Gallio.Runner;
using Gallio.Utilities;

namespace Gallio.TeamCityIntegration
{
    /// <summary>
    /// Monitors <see cref="ITestRunner" /> events and writes debug messages to the
    /// runner's logger.
    /// </summary>
    public class TeamCityExtension : TestRunnerExtension
    {
        private delegate string Continuation();

        private ServiceMessageWriter writer;
        private readonly string flowId;
        private readonly Stack<string> currentStepStack;
        private readonly MultiMap<string, Continuation> continuationMap;

        /// <summary>
        /// Creates a TeamCity logging extension.
        /// </summary>
        public TeamCityExtension()
            : this(null)
        {
        }

        internal TeamCityExtension(string flowId)
        {
            this.flowId = flowId ?? Hash64.CreateUniqueHash().ToString();

            currentStepStack = new Stack<string>();
            continuationMap = new MultiMap<string, Continuation>();
        }

        /// <inheritdoc />
        protected override void Initialize()
        {
            writer = new ServiceMessageWriter(output => Logger.Log(LogSeverity.Important, output));

            Events.InitializeStarted += delegate(object sender, InitializeStartedEventArgs e)
            {
                writer.WriteProgressMessage(flowId, "Initializing test runner.");
            };

            Events.ExploreStarted += delegate(object sender, ExploreStartedEventArgs e)
            {
                writer.WriteProgressStart(flowId, "Exploring tests.");
            };

            Events.ExploreFinished += delegate(object sender, ExploreFinishedEventArgs e)
            {
                writer.WriteProgressFinish(flowId, "Exploring tests."); // nb: message must be same as specified in progress start
            };

            Events.RunStarted += delegate(object sender, RunStartedEventArgs e)
            {
                writer.WriteProgressStart(flowId, "Running tests.");
            };

            Events.RunFinished += delegate(object sender, RunFinishedEventArgs e)
            {
                ClearStep();

                writer.WriteProgressFinish(flowId, "Running tests."); // nb: message must be same as specified in progress start
            };

            Events.DisposeFinished += delegate(object sender, DisposeFinishedEventArgs e)
            {
                writer.WriteProgressMessage(flowId, "Disposed test runner.");
            };

            Events.TestStepStarted += delegate(object sender, TestStepStartedEventArgs e)
            {
                TestStepData step = e.TestStepRun.Step;

                BeginStep(step, () =>
                {
                    string name = step.FullName;
                    if (name.Length != 0)
                    {
                        if (step.IsTestCase)
                        {
                            writer.WriteTestStarted(flowId, name, false);
                        }
                        else if (step.IsPrimary)
                        {
                            writer.WriteTestSuiteStarted(flowId, name);
                        }
                    }
                });
            };

            Events.TestStepFinished += delegate(object sender, TestStepFinishedEventArgs e)
            {
                TestStepRun stepRun = e.TestStepRun;
                TestStepData step = e.TestStepRun.Step;

                EndStep(step, () =>
                {
                    string name = step.FullName;
                    if (name.Length != 0)
                    {
                        if (step.IsTestCase)
                        {
                            TestOutcome outcome = stepRun.Result.Outcome;

                            var outputText = new StringBuilder();
                            var errorText = new StringBuilder();
                            var warningText = new StringBuilder();
                            var failureText = new StringBuilder();

                            foreach (StructuredTestLogStream stream in stepRun.TestLog.Streams)
                            {
                                switch (stream.Name)
                                {
                                    default:
                                    case TestLogStreamNames.ConsoleInput:
                                    case TestLogStreamNames.ConsoleOutput:
                                    case TestLogStreamNames.DebugTrace:
                                    case TestLogStreamNames.Default:
                                        AppendWithSeparator(outputText, stream.ToString());
                                        break;

                                    case TestLogStreamNames.ConsoleError:
                                        AppendWithSeparator(errorText, stream.ToString());
                                        break;

                                    case TestLogStreamNames.Failures:
                                        AppendWithSeparator(failureText, stream.ToString());
                                        break;

                                    case TestLogStreamNames.Warnings:
                                        AppendWithSeparator(warningText, stream.ToString());
                                        break;
                                }
                            }

                            if (outcome.Status != TestStatus.Skipped && warningText.Length != 0)
                                AppendWithSeparator(errorText, warningText.ToString());
                            if (outcome.Status != TestStatus.Failed && failureText.Length != 0)
                                AppendWithSeparator(errorText, failureText.ToString());

                            if (outputText.Length != 0)
                                writer.WriteTestStdOut(flowId, name, outputText.ToString());
                            if (errorText.Length != 0)
                                writer.WriteTestStdErr(flowId, name, errorText.ToString());

                            // TODO: Handle inconclusive.
                            if (outcome.Status == TestStatus.Failed)
                            {
                                writer.WriteTestFailed(flowId, name, outcome.ToString(), failureText.ToString());
                            }
                            else if (outcome.Status == TestStatus.Skipped)
                            {
                                writer.WriteTestIgnored(flowId, name, warningText.ToString());
                            }

                            writer.WriteTestFinished(flowId, name, TimeSpan.FromSeconds(stepRun.Result.Duration));
                        }
                        else if (step.IsPrimary)
                        {
                            writer.WriteTestSuiteFinished(flowId, name);
                        }
                    }
                });
            };
        }

        private void ClearStep()
        {
            currentStepStack.Clear();
            continuationMap.Clear();
        }

        private void BeginStep(TestStepData step, Action action)
        {
            string nextContinuationId = BeginStepContinuation(step, action);
            ResumeContinuation(nextContinuationId);
        }

        private void EndStep(TestStepData step, Action action)
        {
            string nextContinuationId = EndStepContinuation(step, action);
            ResumeContinuation(nextContinuationId);
        }

        private string BeginStepContinuation(TestStepData step, Action action)
        {
            if (currentStepStack.Count == 0 || step.ParentId == currentStepStack.Peek())
            {
                currentStepStack.Push(step.Id);
                action();
                return step.Id;
            }

            SaveContinuation(SanitizeContinuationId(step.ParentId), () => BeginStepContinuation(step, action));
            return null;
        }

        private string EndStepContinuation(TestStepData step, Action action)
        {
            if (step.Id == currentStepStack.Peek())
            {
                currentStepStack.Pop();
                action();
                return SanitizeContinuationId(step.ParentId);
            }

            SaveContinuation(step.Id, () => EndStepContinuation(step, action));
            return null;
        }

        private static string SanitizeContinuationId(string id)
        {
            return id ?? "<null>";
        }

        private void SaveContinuation(string continuationId, Continuation action)
        {
            continuationMap.Add(continuationId, action);
        }

        private void ResumeContinuation(string continuationId)
        {
            while (continuationId != null)
            {
                IList<Continuation> continuations = continuationMap[continuationId];
                if (continuations.Count == 0)
                    break;

                continuationMap.Remove(continuationId);

                string nextContinuationId = null;
                foreach (Continuation continuation in continuations)
                {
                    if (nextContinuationId == null)
                    {
                        nextContinuationId = continuation();
                    }
                    else
                    {
                        continuationMap.Add(continuationId, continuation);
                    }
                }

                continuationId = nextContinuationId;
            }
        }

        private static void AppendWithSeparator(StringBuilder builder, string text)
        {
            if (text.Length != 0)
            {
                if (builder.Length != 0)
                    builder.Append("\n\n");

                builder.Append(text);

                while (builder.Length > 0 && char.IsWhiteSpace(builder[builder.Length - 1]))
                    builder.Length -= 1;
            }
        }
    }
}
