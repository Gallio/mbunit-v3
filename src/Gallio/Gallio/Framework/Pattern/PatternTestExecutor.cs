using System;
using System.Collections.Generic;
using System.Threading;
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

        public TestOutcome RunTest(ITestCommand testCommand, ITestInstance parentTestInstance,
            Sandbox parentSandbox, PatternTestHandlerDecorator testHandlerDecorator)
        {
            if (progressMonitor.IsCanceled)
                return TestOutcome.Canceled;

            if (!testCommand.AreDependenciesSatisfied())
            {
                ITestContext context = testCommand.StartRootStep(parentTestInstance);
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
                    return RunTestBody(testCommand, parentTestInstance, sandbox, testHandlerDecorator, test);
                }
            }
            catch (Exception ex)
            {
                return ReportTestError(testCommand, parentTestInstance, ex, String.Format("An exception occurred while preparing to run test '{0}'.", test.FullName));
            }
            finally
            {
                progressMonitor.SetStatus("");
                progressMonitor.Worked(1);
            }
        }

        private TestOutcome RunTestBody(ITestCommand testCommand, ITestInstance parentTestInstance,
            Sandbox sandbox, PatternTestHandlerDecorator testHandlerDecorator, PatternTest test)
        {
            using (CreateTimeoutTimer(sandbox, test.Timeout))
            {
                IPatternTestHandler testHandler = test.TestActions;

                TestOutcome outcome;
                if (testHandlerDecorator != null)
                    outcome = testHandlerDecorator(sandbox, ref testHandler);
                else
                    outcome = TestOutcome.Passed;

                if (outcome.Status == TestStatus.Passed)
                {
                    PatternTestState testState = new PatternTestState(test, testHandler, converter, formatter);

                    outcome = outcome.CombineWith(DoBeforeTest(sandbox, testState));
                    if (outcome.Status == TestStatus.Passed)
                        outcome = outcome.CombineWith(RunTestInstances(testCommand, parentTestInstance, sandbox, testState));

                    outcome = outcome.CombineWith(DoAfterTest(sandbox, testState));
                }

                return outcome;
            }
        }

        private TestOutcome RunTestInstances(ITestCommand testCommand, ITestInstance parentTestInstance,
            Sandbox sandbox, PatternTestState testState)
        {
            try
            {
                TestOutcome outcome = TestOutcome.Passed;
                foreach (DataBindingItem item in testState.BindingContext.GetItems(! options.SkipDynamicTestInstances))
                {
                    outcome = outcome.CombineWith(RunTestInstance(testCommand, parentTestInstance, sandbox, testState, item));
                }

                return GeneralizeInheritedOutcome(outcome);
            }
            catch (Exception ex)
            {
                return ReportTestError(testCommand, parentTestInstance, ex, String.Format("An exception occurred while getting data items for test '{0}'.", testState.Test.FullName));
            }
        }

        private TestOutcome RunTestInstance(ITestCommand testCommand, ITestInstance parentTestInstance,
            Sandbox sandbox, PatternTestState testState, DataBindingItem bindingItem)
        {
            try
            {
                using (bindingItem)
                {
                    PatternTestInstanceActions decoratedTestInstanceActions =
                        PatternTestInstanceActions.CreateDecorator(testState.TestHandler.TestInstanceHandler);

                    TestOutcome outcome = DoDecorateTestInstance(sandbox, testState, decoratedTestInstanceActions);
                    if (outcome.Status == TestStatus.Passed)
                    {
                        PatternTestInstance testInstance = new PatternTestInstance(testState.Test, parentTestInstance,
                            testState.Test.Name, bindingItem.GetRow().IsDynamic);

                        PatternTestInstanceState testInstanceState = new PatternTestInstanceState(testInstance, decoratedTestInstanceActions, testState, bindingItem);

                        outcome = outcome.CombineWith(DoBeforeTestInstance(sandbox, testInstanceState));
                        if (outcome.Status == TestStatus.Passed)
                        {
                            progressMonitor.SetStatus(testInstanceState.TestInstance.Name);

                            Context context = Context.PrepareContext(testCommand.StartRootStep(new BaseTestStep(testInstanceState.TestInstance)), sandbox);
                            outcome = outcome.CombineWith(RunTestInstanceWithContext(testCommand, context, testInstanceState));
                            context.FinishStep(outcome);

                            progressMonitor.SetStatus("");
                        }

                        outcome = outcome.CombineWith(DoAfterTestInstance(sandbox, testInstanceState));
                    }

                    return outcome;
                }
            }
            catch (Exception ex)
            {
                return ReportTestError(testCommand, parentTestInstance, ex, String.Format("An exception occurred while preparing an instance of test '{0}'.", testState.Test.FullName));
            }
        }

        private TestOutcome RunTestInstanceWithContext(ITestCommand testCommand, Context context,
            PatternTestInstanceState testInstanceState)
        {
            try
            {
                if (options.SkipTestInstanceExecution)
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
                Log.Failures.WriteException(ex, "An exception occurred while running test instance '{0}'.", testInstanceState.TestInstance.Name);
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

                outcome = outcome.CombineWith(RunTest(childTestCommand, testInstanceState.TestInstance, sandbox, testHandlerDecorator));
            }

            return GeneralizeInheritedOutcome(outcome);
        }

        private void UpdateInterimOutcome(Context context, ref TestOutcome outcome, TestOutcome newOutcome)
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
            }, "Before Test", null);
        }

        private static TestOutcome DoAfterTest(Sandbox sandbox, PatternTestState testState)
        {
            return sandbox.Run(delegate
            {
                testState.TestHandler.AfterTest(testState);
            }, "After Test", null);
        }

        private static TestOutcome DoDecorateTestInstance(Sandbox sandbox, PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
        {
            return sandbox.Run(delegate
            {
                testState.TestHandler.DecorateTestInstance(testState, decoratedTestInstanceActions);
            }, "Decorate Child Test", null);
        }

        private static TestOutcome DoBeforeTestInstance(Sandbox sandbox, PatternTestInstanceState testInstanceState)
        {
            foreach (KeyValuePair<string, string> entry in testInstanceState.BindingItem.GetRow().GetMetadata())
                testInstanceState.TestInstance.Metadata.Add(entry.Key, entry.Value);

            if (testInstanceState.TestState.SlotBindingAccessors.Count != 0)
            {
                foreach (KeyValuePair<ISlotInfo, IDataBindingAccessor> entry in testInstanceState.TestState.SlotBindingAccessors)
                    testInstanceState.SlotValues.Add(entry.Key, entry.Value.GetValue(testInstanceState.BindingItem));
            }

            return sandbox.Run(delegate
            {
                testInstanceState.TestInstanceHandler.BeforeTestInstance(testInstanceState);
            }, "Before Test Instance", null);
        }

        private static TestOutcome DoInitializeTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Initialize;

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.InitializeTestInstance(testInstanceState);
                }, "Initialize", null);
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
                }, "Set Up", null);
            }
        }

        private static TestOutcome DoExecuteTestInstance(Context context, PatternTestInstanceState testInstanceState)
        {
            using (context.Enter())
            {
                context.LifecyclePhase = LifecyclePhases.Execute;

                string expectedExceptionType = testInstanceState.TestInstance.Metadata.GetValue(MetadataKeys.ExpectedException)
                    ?? testInstanceState.Test.Metadata.GetValue(MetadataKeys.ExpectedException);

                return context.Sandbox.Run(delegate
                {
                    testInstanceState.TestInstanceHandler.ExecuteTestInstance(testInstanceState);
                }, null, expectedExceptionType);
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
                }, "Tear Down", null);
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
                }, "Dispose", null);
            }
        }

        private static TestOutcome DoAfterTestInstance(Sandbox sandbox, PatternTestInstanceState testInstanceState)
        {
            return sandbox.Run(delegate
            {
                testInstanceState.TestInstanceHandler.AfterTestInstance(testInstanceState);
            }, "After Test Instance", null);
        }

        private static TestOutcome DoDecorateChildTest(Sandbox sandbox, PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildTestActions)
        {
            return sandbox.Run(delegate
            {
                testInstanceState.TestInstanceHandler.DecorateChildTest(testInstanceState, decoratedChildTestActions);
            }, "Decorate Child Test", null);
        }
        #endregion

        private static Timer CreateTimeoutTimer(Sandbox sandbox, TimeSpan? timeout)
        {
            if (!timeout.HasValue)
                return null;

            return new Timer(delegate { AbortSandboxDueToTimeout(sandbox, timeout.Value); }, null, (int)timeout.Value.TotalMilliseconds, Timeout.Infinite);
        }

        private static void AbortSandboxDueToTimeout(Sandbox sandbox, TimeSpan timeout)
        {
            sandbox.Abort(TestOutcome.Timeout, String.Format("The test timed out after {0} seconds.", timeout.TotalSeconds));
        }

        private static TestOutcome ReportTestError(ITestCommand testCommand, ITestInstance parentTestInstance, Exception ex, string message)
        {
            ITestContext context = testCommand.StartRootStep(parentTestInstance);
            Log.Failures.WriteException(ex, message);
            context.FinishStep(TestOutcome.Error, null);
            return TestOutcome.Error;
        }

        private static TestOutcome GeneralizeInheritedOutcome(TestOutcome combinedInheritedOutcome)
        {
            switch (combinedInheritedOutcome.Status)
            {
                case TestStatus.Passed:
                    return TestOutcome.Passed;
                case TestStatus.Failed:
                    return TestOutcome.Failed;
                default:
                    return TestOutcome.Inconclusive;
            }
        }
    }
}
