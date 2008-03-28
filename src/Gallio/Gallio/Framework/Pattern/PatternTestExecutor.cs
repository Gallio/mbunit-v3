// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using System.Collections.Generic;
using System.Threading;
using Gallio.Concurrency;
using Gallio.Framework.Data.Binders;
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Hosting.ProgressMonitoring;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Encapsulates the algorithm for recursively running a <see cref="PatternTest" />.
    /// </summary>
    internal class PatternTestExecutor
    {
        public delegate TestOutcome PatternTestHandlerDecorator(Sandbox sandbox, ref IPatternTestHandler handler);

        private readonly TestExecutionOptions options;
        private readonly IProgressMonitor progressMonitor;
        private readonly IFormatter formatter;
        private readonly IConverter converter;

        public PatternTestExecutor(TestExecutionOptions options, IProgressMonitor progressMonitor,
            IFormatter formatter, IConverter converter)
        {
            this.options = options;
            this.progressMonitor = progressMonitor;
            this.formatter = formatter;
            this.converter = converter;
        }

        public TestOutcome RunTest(ITestCommand testCommand, ITestStep parentTestStep,
            Sandbox parentSandbox, PatternTestHandlerDecorator testHandlerDecorator)
        {
            if (progressMonitor.IsCanceled)
                return TestOutcome.Canceled;

            if (!testCommand.AreDependenciesSatisfied())
            {
                ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
                Log.Warnings.WriteLine("Skipped due to an unsatisfied test dependency.");
                context.FinishStep(TestOutcome.Skipped, null);
                return TestOutcome.Skipped;
            }

            progressMonitor.SetStatus(testCommand.Test.Name);

            PatternTest test = (PatternTest) testCommand.Test;
            try
            {
                using (Sandbox sandbox = parentSandbox.CreateChild())
                {
                    return RunTestBody(testCommand, parentTestStep, sandbox, testHandlerDecorator, test);
                }
            }
            catch (Exception ex)
            {
                return ReportTestError(testCommand, parentTestStep, ex, String.Format("An exception occurred while preparing to run test '{0}'.", test.FullName));
            }
            finally
            {
                progressMonitor.SetStatus("");
                progressMonitor.Worked(1);
            }
        }

        private TestOutcome RunTestBody(ITestCommand testCommand, ITestStep parentTestStep,
            Sandbox sandbox, PatternTestHandlerDecorator testHandlerDecorator, PatternTest test)
        {
            TestOutcome outcome = TestOutcome.Error;

            DoWithTimeout(sandbox, test.Timeout, delegate
            {
                DoWithApartmentState(test.ApartmentState, delegate
                {
                    IPatternTestHandler testHandler = test.TestActions;

                    if (testHandlerDecorator != null)
                        outcome = testHandlerDecorator(sandbox, ref testHandler);
                    else
                        outcome = TestOutcome.Passed;

                    if (outcome.Status == TestStatus.Passed)
                    {
                        PatternTestStep primaryTestStep = new PatternTestStep(test, parentTestStep, test.Name, test.CodeElement, true);
                        PatternTestState testState = new PatternTestState(primaryTestStep, testHandler, converter, formatter, testCommand.IsExplicit);

                        outcome = outcome.CombineWith(DoBeforeTest(sandbox, testState));
                        if (outcome.Status == TestStatus.Passed)
                        {
                            bool reusePrimaryTestStep = !testState.BindingContext.HasBindings;
                            if (!reusePrimaryTestStep)
                                primaryTestStep.IsTestCase = false;

                            Context context = Context.PrepareContext(testCommand.StartStep(primaryTestStep), sandbox);
                            outcome = outcome.CombineWith(DoInitializeTest(context, testState));

                            if (outcome.Status == TestStatus.Passed)
                                outcome = outcome.CombineWith(RunTestInstances(testCommand, context, testState, reusePrimaryTestStep));

                            outcome = outcome.CombineWith(DoDisposeTest(context, testState));

                            context.FinishStep(outcome);
                        }

                        outcome = outcome.CombineWith(DoAfterTest(sandbox, testState));
                    }
                });
            });

            return outcome;
        }

        private TestOutcome RunTestInstances(ITestCommand testCommand, Context primaryContext,
            PatternTestState testState, bool reusePrimaryTestStep)
        {
            try
            {
                TestOutcome outcome = TestOutcome.Passed;
                foreach (DataBindingItem item in testState.BindingContext.GetItems(! options.SkipDynamicTests))
                {
                    outcome = outcome.CombineWith(RunTestInstance(testCommand, primaryContext, testState, item, reusePrimaryTestStep));
                }

                return reusePrimaryTestStep ? outcome : GeneralizeInheritedOutcome(outcome);
            }
            catch (Exception ex)
            {
                Log.Failures.WriteException(ex, String.Format("An exception occurred while getting data items for test '{0}'.", testState.Test.FullName));
                return TestOutcome.Error;
            }
        }

        private TestOutcome RunTestInstance(ITestCommand testCommand, Context primaryContext,
            PatternTestState testState, DataBindingItem bindingItem, bool reusePrimaryTestStep)
        {
            try
            {
                using (bindingItem)
                {
                    PatternTestInstanceActions decoratedTestInstanceActions =
                        PatternTestInstanceActions.CreateDecorator(testState.TestHandler.TestInstanceHandler);

                    TestOutcome outcome = DoDecorateTestInstance(primaryContext.Sandbox, testState, decoratedTestInstanceActions);
                    if (outcome.Status == TestStatus.Passed)
                    {
                        PatternTestStep testStep;
                        if (reusePrimaryTestStep)
                        {
                            testStep = testState.PrimaryTestStep;
                        }
                        else
                        {
                            testStep = new PatternTestStep(testState.Test, testState.PrimaryTestStep,
                                testState.Test.Name, testState.Test.CodeElement, false);
                            testStep.IsDynamic = bindingItem.GetRow().IsDynamic;
                        }

                        PatternTestInstanceState testInstanceState = new PatternTestInstanceState(testStep, decoratedTestInstanceActions, testState, bindingItem);

                        outcome = outcome.CombineWith(DoBeforeTestInstance(primaryContext.Sandbox, testInstanceState));
                        if (outcome.Status == TestStatus.Passed)
                        {
                            progressMonitor.SetStatus(testStep.Name);

                            Context context = reusePrimaryTestStep
                                ? primaryContext
                                : Context.PrepareContext(testCommand.StartStep(testStep), primaryContext.Sandbox.CreateChild());

                            outcome = outcome.CombineWith(RunTestInstanceWithContext(testCommand, context, testInstanceState));

                            if (!reusePrimaryTestStep)
                                context.FinishStep(outcome);

                            progressMonitor.SetStatus("");
                        }

                        outcome = outcome.CombineWith(DoAfterTestInstance(primaryContext.Sandbox, testInstanceState));
                    }

                    return outcome;
                }
            }
            catch (Exception ex)
            {
                string message = String.Format("An exception occurred while preparing an instance of test '{0}'.", testState.Test.FullName);

                if (reusePrimaryTestStep)
                {
                    Log.Failures.WriteException(ex, message);
                    return TestOutcome.Error;
                }
                else
                {
                    return ReportTestError(testCommand, testState.PrimaryTestStep, ex, message);
                }
            }
        }

        private TestOutcome RunTestInstanceWithContext(ITestCommand testCommand, Context context,
            PatternTestInstanceState testInstanceState)
        {
            try
            {
                if (options.SkipTestExecution)
                {
                    return RunTestChildren(testCommand, context.Sandbox, testInstanceState);
                }
                else
                {
                    TestOutcome outcome = TestOutcome.Passed;                    
                    UpdateInterimOutcome(context, ref outcome, DoInitializeTestInstance(context, testInstanceState));
                    if (outcome.Status == TestStatus.Passed)
                    {
                        UpdateInterimOutcome(context, ref outcome, DoSetUpTestInstance(context, testInstanceState));
                        if (outcome.Status == TestStatus.Passed)
                        {
                            UpdateInterimOutcome(context, ref outcome, DoExecuteTestInstance(context, testInstanceState));

                            if (outcome.Status == TestStatus.Passed)
                            {
                                UpdateInterimOutcome(context, ref outcome, RunTestChildren(testCommand, context.Sandbox, testInstanceState));
                            }
                        }

                        UpdateInterimOutcome(context, ref outcome, DoTearDownTestInstance(context, testInstanceState));
                    }

                    UpdateInterimOutcome(context, ref outcome, DoDisposeTestInstance(context, testInstanceState));
                    return outcome;
                }
            }
            catch (Exception ex)
            {
                Log.Failures.WriteException(ex, "An exception occurred while running test instance '{0}'.", testInstanceState.TestStep.Name);
                return TestOutcome.Error;
            }
        }

        private TestOutcome RunTestChildren(ITestCommand testCommand, Sandbox sandbox,
            PatternTestInstanceState testInstanceState)
        {
            TestOutcome outcome = TestOutcome.Passed;

            foreach (ITestCommand childTestCommand in testCommand.Children)
            {
                if (progressMonitor.IsCanceled)
                    return TestOutcome.Canceled;

                PatternTestHandlerDecorator testHandlerDecorator = delegate(Sandbox childSandbox, ref IPatternTestHandler childHandler)
                {
                    PatternTestActions decoratedChildTestActions = PatternTestActions.CreateDecorator(childHandler);
                    childHandler = decoratedChildTestActions;

                    return DoDecorateChildTest(childSandbox, testInstanceState, decoratedChildTestActions);
                };

                outcome = outcome.CombineWith(RunTest(childTestCommand, testInstanceState.TestStep, sandbox, testHandlerDecorator));
            }

            return GeneralizeInheritedOutcome(outcome);
        }

        private static void UpdateInterimOutcome(Context context, ref TestOutcome outcome, TestOutcome newOutcome)
        {
            outcome = outcome.CombineWith(newOutcome);
            context.SetInterimOutcome(outcome);
        }

        #region Actions
        private static TestOutcome DoBeforeTest(Sandbox sandbox, PatternTestState testState)
        {
            foreach (PatternTestParameter parameter in testState.Test.Parameters)
            {
                IDataBindingAccessor bindingAccessor = parameter.Binder.Register(testState.BindingContext, parameter);
                testState.SlotBindingAccessors.Add(new KeyValuePair<ISlotInfo, IDataBindingAccessor>(parameter.Slot, bindingAccessor));
            }

            return sandbox.Run(delegate
            {
                testState.TestHandler.BeforeTest(testState);
            }, "Before Test");
        }

        private static TestOutcome DoInitializeTest(Context context, PatternTestState testState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Initialize;

                return context.Sandbox.Run(delegate
                {
                    testState.TestHandler.InitializeTest(testState);
                }, "Initialize");
            }
        }

        private static TestOutcome DoDisposeTest(Context context, PatternTestState testState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Dispose;

                return context.Sandbox.Run(delegate
                {
                    testState.TestHandler.DisposeTest(testState);
                }, "Dispose");
            }
        }

        private static TestOutcome DoAfterTest(Sandbox sandbox, PatternTestState testState)
        {
            return sandbox.Run(delegate
            {
                testState.TestHandler.AfterTest(testState);
            }, "After Test");
        }

        private static TestOutcome DoDecorateTestInstance(Sandbox sandbox, PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
        {
            return sandbox.Run(delegate
            {
                testState.TestHandler.DecorateTestInstance(testState, decoratedTestInstanceActions);
            }, "Decorate Child Test");
        }

        private static TestOutcome DoBeforeTestInstance(Sandbox sandbox, PatternTestInstanceState testInstanceState)
        {
            foreach (KeyValuePair<string, string> entry in testInstanceState.BindingItem.GetRow().GetMetadata())
                testInstanceState.TestStep.Metadata.Add(entry.Key, entry.Value);

            if (testInstanceState.TestState.SlotBindingAccessors.Count != 0)
            {
                foreach (KeyValuePair<ISlotInfo, IDataBindingAccessor> entry in testInstanceState.TestState.SlotBindingAccessors)
                    testInstanceState.SlotValues.Add(entry.Key, entry.Value.GetValue(testInstanceState.BindingItem));
            }

            return sandbox.Run(delegate
            {
                testInstanceState.TestInstanceHandler.BeforeTestInstance(testInstanceState);
            }, "Before Test Instance");
        }

        private static TestOutcome DoInitializeTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Initialize;

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.InitializeTestInstance(testInstanceState);
                }, "Initialize");
            }
        }

        private static TestOutcome DoSetUpTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.SetUp;

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.SetUpTestInstance(testInstanceState);
                }, "Set Up");
            }
        }

        private static TestOutcome DoExecuteTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Execute;

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.ExecuteTestInstance(testInstanceState);
                }, null);
            }
        }

        private static TestOutcome DoTearDownTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.TearDown;

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.TearDownTestInstance(testInstanceState);
                }, "Tear Down");
            }
        }

        private static TestOutcome DoDisposeTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Dispose;

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.DisposeTestInstance(testInstanceState);
                }, "Dispose");
            }
        }

        private static TestOutcome DoAfterTestInstance(Sandbox sandbox, PatternTestInstanceState testInstanceState)
        {
            return sandbox.Run(delegate
            {
                testInstanceState.TestInstanceHandler.AfterTestInstance(testInstanceState);
            }, "After Test Instance");
        }

        private static TestOutcome DoDecorateChildTest(Sandbox sandbox, PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildTestActions)
        {
            return sandbox.Run(delegate
            {
                testInstanceState.TestInstanceHandler.DecorateChildTest(testInstanceState, decoratedChildTestActions);
            }, "Decorate Child Test");
        }
        #endregion

        private static void AbortSandboxDueToTimeout(Sandbox sandbox, TimeSpan timeout)
        {
            sandbox.Abort(TestOutcome.Timeout, String.Format("The test timed out after {0} seconds.", timeout.TotalSeconds));
        }

        private static TestOutcome ReportTestError(ITestCommand testCommand, ITestStep parentTestStep, Exception ex, string message)
        {
            ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
            Log.Failures.WriteException(ex, message);
            context.FinishStep(TestOutcome.Error, null);
            return TestOutcome.Error;
        }

        private static TestOutcome GeneralizeInheritedOutcome(TestOutcome combinedInheritedOutcome)
        {
            switch (combinedInheritedOutcome.Status)
            {
                case TestStatus.Skipped:
                case TestStatus.Passed:
                    return TestOutcome.Passed;
                case TestStatus.Failed:
                    return TestOutcome.Failed;
                default:
                    return TestOutcome.Inconclusive;
            }
        }

        private static void DoWithTimeout(Sandbox sandbox, TimeSpan? timeout, Action action)
        {
            using (timeout.HasValue ?
                new Timer(delegate { AbortSandboxDueToTimeout(sandbox, timeout.Value); }, null, (int)timeout.Value.TotalMilliseconds, Timeout.Infinite)
                : null)
            {
                action();
            }
        }

        private static void DoWithApartmentState(ApartmentState apartmentState, Action action)
        {
            if (apartmentState != ApartmentState.Unknown
                && Thread.CurrentThread.GetApartmentState() != apartmentState)
            {
                ThreadTask task = new ThreadTask("Test Runner " + apartmentState, action);
                task.ApartmentState = apartmentState;
                task.Run(null);

                if (task.Result.Exception != null)
                    throw new ModelException(String.Format("Failed to perform action in thread with overridden apartment state {0}.",
                        apartmentState), task.Result.Exception);
            }
            else
            {
                action();
            }
        }
    }
}
