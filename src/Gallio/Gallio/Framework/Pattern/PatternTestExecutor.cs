// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Concurrency;
using Gallio.Framework.Data;
using Gallio.Model.Commands;
using Gallio.Model.Contexts;
using Gallio.Model.Tree;
using Gallio.Model.Environments;
using Gallio.Runtime.Conversions;
using Gallio.Runtime.Formatting;
using Gallio.Framework;
using Gallio.Model;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using Gallio.Runtime.ProgressMonitoring;
using System.Diagnostics;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Encapsulates the algorithm for recursively running a <see cref="PatternTest" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Please note that this code has been optimized to minimize the stack depth so as
    /// to be more debugger-friendly.  That means some of these methods (and the methods
    /// they call) are rather a lot longer and harder to read than they used to be.
    /// However the payoff is very significant for end-users.  As a result of this optimization
    /// the effective stack depth for a simple test has been reduced from over 160 to about 30 in many cases.
    /// -- Jeff.
    /// </para>
    /// </remarks>
    [SystemInternal]
    internal class PatternTestExecutor
    {
        public delegate TestOutcome PatternTestActionsDecorator(Sandbox sandbox, ref PatternTestActions testActions);

        private readonly TestExecutionOptions options;
        private readonly IProgressMonitor progressMonitor;
        private readonly IFormatter formatter;
        private readonly IConverter converter;
        private readonly ITestEnvironmentManager environmentManager;
        private readonly WorkScheduler scheduler;

        public PatternTestExecutor(TestExecutionOptions options, IProgressMonitor progressMonitor,
            IFormatter formatter, IConverter converter, ITestEnvironmentManager environmentManager)
        {
            this.options = options;
            this.progressMonitor = progressMonitor;
            this.formatter = formatter;
            this.converter = converter;
            this.environmentManager = environmentManager;

            scheduler = new WorkScheduler(() =>
                options.SingleThreaded ? 1 : TestAssemblyExecutionParameters.DegreeOfParallelism);
        }

        public RunTestAction CreateActionToRunTest(ITestCommand testCommand, Model.Tree.TestStep parentTestStep,
            Sandbox parentSandbox, PatternTestActionsDecorator testActionsDecorator)
        {
            return new RunTestAction(this, testCommand, parentTestStep, parentSandbox, testActionsDecorator);
        }

        private static void UpdateInterimOutcome(TestContext context, ref TestOutcome outcome, TestOutcome newOutcome)
        {
            outcome = outcome.CombineWith(newOutcome);
            context.SetInterimOutcome(outcome);
        }

        private static TestResult PublishOutcomeFromInvisibleTest(ITestCommand testCommand, Model.Tree.TestStep testStep, TestOutcome outcome)
        {
            switch (outcome.Status)
            {
                case TestStatus.Skipped:
                case TestStatus.Passed:
                    // Either nothing interesting happened or the test was silently skipped during Before/After.
                    return new TestResult(TestOutcome.Passed);

                case TestStatus.Failed:
                case TestStatus.Inconclusive:
                default:
                    // Something bad happened during Before/After that prevented the test from running.
                    ITestContext context = testCommand.StartStep(testStep);
                    context.LogWriter.Failures.Write("The test did not run.  Consult the parent test log for more details.");
                    return context.FinishStep(outcome, null);
            }
        }

        #region Actions
        internal sealed class RunTestAction
        {
            private readonly PatternTestExecutor executor;
            private readonly ITestCommand testCommand;
            private readonly Model.Tree.TestStep parentTestStep;
            private readonly Sandbox parentSandbox;
            private readonly PatternTestActionsDecorator testActionsDecorator;
            private readonly TestContext parentContext;

            private TestResult result;
            private bool reentered;

            public RunTestAction(PatternTestExecutor executor, ITestCommand testCommand, Model.Tree.TestStep parentTestStep, Sandbox parentSandbox, PatternTestActionsDecorator testActionsDecorator)
            {
                this.executor = executor;
                this.testCommand = testCommand;
                this.parentTestStep = parentTestStep;
                this.parentSandbox = parentSandbox;
                this.testActionsDecorator = testActionsDecorator;

                result = new TestResult(TestOutcome.Error);
            }

            public RunTestAction(PatternTestExecutor executor, ITestCommand testCommand, TestContext parentContext, PatternTestActionsDecorator testActionsDecorator)
                : this(executor, testCommand, parentContext.TestStep, parentContext.Sandbox, testActionsDecorator)
            {
                this.parentContext = parentContext;
            }

            public TestResult Result
            {
                get { return result; }
            }

            [DebuggerNonUserCode]
            public void Run()
            {
                var test = (PatternTest)testCommand.Test;

                TestContextCookie? parentContextCookie = null;
                try
                {
                    if (parentContext != null)
                        parentContextCookie = parentContext.Enter();

                    // The first time we call Run, we check whether the ApartmentState of the
                    // Thread is correct.  If it is not, then we start a new thread and reenter
                    // with a flag set to skip initial processing.
                    if (!reentered)
                    {
                        if (executor.progressMonitor.IsCanceled)
                        {
                            result = new TestResult(TestOutcome.Canceled);
                            return;
                        }

                        if (!testCommand.AreDependenciesSatisfied())
                        {
                            ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
                            context.LogWriter.Warnings.WriteLine("Skipped due to an unsatisfied test dependency.");
                            result = context.FinishStep(TestOutcome.Skipped, null);
                            return;
                        }

                        executor.progressMonitor.SetStatus(test.Name);

                        if (test.ApartmentState != ApartmentState.Unknown
                            && Thread.CurrentThread.GetApartmentState() != test.ApartmentState)
                        {
                            reentered = true;

                            ThreadTask task = new TestEnvironmentAwareThreadTask("Test Runner " + test.ApartmentState,
                                (Action) Run, executor.environmentManager);
                            task.ApartmentState = test.ApartmentState;
                            task.Run(null);

                            if (!task.Result.HasValue)
                            {
                                throw new ModelException(
                                    String.Format("Failed to perform action in thread with overridden apartment state {0}.",
                                        test.ApartmentState), task.Result.Exception);
                            }

                            return;
                        }
                    }

                    // Actually run the test.
                    // Yes, this is a monstruously long method due to the inlining optimzation to minimize stack depth.
                    using (Sandbox sandbox = parentSandbox.CreateChild())
                    {
                        using (new ProcessIsolation())
                        {
                            using (sandbox.StartTimer(test.Timeout))
                            {
                                TestOutcome outcome;
                                PatternTestActions testActions = test.TestActions;

                                if (testActionsDecorator != null)
                                    outcome = testActionsDecorator(sandbox, ref testActions);
                                else
                                    outcome = TestOutcome.Passed;

                                if (outcome.Status == TestStatus.Passed)
                                {
                                    PatternTestStep primaryTestStep = new PatternTestStep(test, parentTestStep);
                                    PatternTestState testState = new PatternTestState(primaryTestStep, testActions,
                                        executor.converter, executor.formatter, testCommand.IsExplicit);

                                    bool invisibleTest = true;

                                    outcome = outcome.CombineWith(sandbox.Run(TestLog.Writer,
                                        new BeforeTestAction(testState).Run, "Before Test"));

                                    if (outcome.Status == TestStatus.Passed)
                                    {
                                        bool reusePrimaryTestStep = !testState.BindingContext.HasBindings;
                                        if (!reusePrimaryTestStep)
                                            primaryTestStep.IsTestCase = false;

                                        invisibleTest = false;
                                        TestContext primaryContext = TestContext.PrepareContext(
                                            testCommand.StartStep(primaryTestStep), sandbox);
                                        testState.SetInContext(primaryContext);

                                        using (primaryContext.Enter())
                                        {
                                            primaryContext.LifecyclePhase = LifecyclePhases.Initialize;

                                            outcome = outcome.CombineWith(primaryContext.Sandbox.Run(TestLog.Writer,
                                                new InitializeTestAction(testState).Run, "Initialize"));
                                        }

                                        if (outcome.Status == TestStatus.Passed)
                                        {
                                            try
                                            {
                                                bool atLeastOneItem = false;
                                                foreach (IDataItem bindingItem in testState.BindingContext.GetItems(!executor.options.SkipDynamicTests))
                                                {
                                                    atLeastOneItem = true;

                                                    if (executor.progressMonitor.IsCanceled)
                                                    {
                                                        outcome = TestOutcome.Canceled;
                                                        break;
                                                    }

                                                    TestOutcome instanceOutcome;
                                                    try
                                                    {
                                                        PatternTestInstanceActions decoratedTestInstanceActions = testState.TestActions.TestInstanceActions.Copy();

                                                        instanceOutcome = primaryContext.Sandbox.Run(TestLog.Writer,
                                                            new DecorateTestInstanceAction(testState, decoratedTestInstanceActions).Run, "Decorate Child Test");

                                                        if (instanceOutcome.Status == TestStatus.Passed)
                                                        {
                                                            bool invisibleTestInstance = true;

                                                            PatternTestStep testStep;
                                                            if (reusePrimaryTestStep)
                                                            {
                                                                testStep = testState.PrimaryTestStep;
                                                                invisibleTestInstance = false;

                                                                PropertyBag map = DataItemUtils.GetMetadata(bindingItem);
                                                                foreach (KeyValuePair<string, string> entry in map.Pairs)
                                                                    primaryContext.AddMetadata(entry.Key, entry.Value);
                                                            }
                                                            else
                                                            {
                                                                testStep = new PatternTestStep(testState.Test, testState.PrimaryTestStep,
                                                                    testState.Test.Name, testState.Test.CodeElement, false);
                                                                testStep.Kind = testState.Test.Kind;

                                                                testStep.IsDynamic = bindingItem.IsDynamic;
                                                                bindingItem.PopulateMetadata(testStep.Metadata);
                                                            }

                                                            var bodyAction = new RunTestInstanceAction(executor, testCommand);
                                                            var testInstanceState = new PatternTestInstanceState(testStep, decoratedTestInstanceActions, testState, bindingItem, bodyAction.Run);
                                                            bodyAction.TestInstanceState = testInstanceState;

                                                            instanceOutcome = instanceOutcome.CombineWith(primaryContext.Sandbox.Run(
                                                                TestLog.Writer, new BeforeTestInstanceAction(testInstanceState).Run, "Before Test Instance"));

                                                            if (instanceOutcome.Status == TestStatus.Passed)
                                                            {
                                                                executor.progressMonitor.SetStatus(testStep.Name);

                                                                TestContext context = reusePrimaryTestStep
                                                                    ? primaryContext
                                                                    : TestContext.PrepareContext(testCommand.StartStep(testStep), primaryContext.Sandbox.CreateChild());
                                                                testState.SetInContext(context);
                                                                testInstanceState.SetInContext(context);
                                                                invisibleTestInstance = false;

                                                                using (context.Enter())
                                                                {
                                                                    if (RunTestInstanceBodyAction.CanOptimizeCallChainAndInvokeBodyActionDirectly(testInstanceState))
                                                                    {
                                                                        bodyAction.SkipProtect = true;
                                                                        instanceOutcome = instanceOutcome.CombineWith(bodyAction.Run());
                                                                    }
                                                                    else
                                                                    {
                                                                        var runTestInstanceBodyAction = new RunTestInstanceBodyAction(testInstanceState);
                                                                        TestOutcome sandboxOutcome = context.Sandbox.Run(TestLog.Writer, runTestInstanceBodyAction.Run, "Body");

                                                                        instanceOutcome = instanceOutcome.CombineWith(runTestInstanceBodyAction.Outcome.CombineWith(sandboxOutcome));
                                                                    }
                                                                }

                                                                if (!reusePrimaryTestStep)
                                                                    context.FinishStep(instanceOutcome);

                                                                executor.progressMonitor.SetStatus("");
                                                            }

                                                            instanceOutcome = instanceOutcome.CombineWith(primaryContext.Sandbox.Run(
                                                                TestLog.Writer, new AfterTestInstanceAction(testInstanceState).Run, "After Test Instance"));

                                                            if (invisibleTestInstance)
                                                                instanceOutcome = PublishOutcomeFromInvisibleTest(testCommand, testStep, instanceOutcome).Outcome;
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        string message = String.Format("An exception occurred while preparing an instance of test '{0}'.", testState.Test.FullName);

                                                        if (reusePrimaryTestStep)
                                                        {
                                                            TestLog.Failures.WriteException(ex, message);
                                                            instanceOutcome = TestOutcome.Error;
                                                        }
                                                        else
                                                        {
                                                            instanceOutcome = ReportTestError(testCommand, testState.PrimaryTestStep, ex, message).Outcome;
                                                        }
                                                    }

                                                    // If we are reporting a single result (reuse = true) then provide full details, otherwise
                                                    // just keep the general status.
                                                    outcome = outcome.CombineWith(reusePrimaryTestStep ? instanceOutcome : instanceOutcome.Generalize());
                                                }

                                                if (!atLeastOneItem && outcome == TestOutcome.Passed)
                                                {
                                                    TestLog.Warnings.WriteLine("Test skipped because it is parameterized but no data was provided.");
                                                    outcome = TestOutcome.Skipped;
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                TestLog.Failures.WriteException(ex, String.Format("An exception occurred while getting data items for test '{0}'.", testState.Test.FullName));
                                                outcome = TestOutcome.Error;
                                            }
                                        }

                                        using (primaryContext.Enter())
                                        {
                                            primaryContext.LifecyclePhase = LifecyclePhases.Dispose;

                                            outcome = outcome.CombineWith(primaryContext.Sandbox.Run(TestLog.Writer,
                                                new DisposeTestAction(testState).Run, "Dispose"));
                                        }

                                        result = primaryContext.FinishStep(outcome);
                                    }

                                    outcome = outcome.CombineWith(sandbox.Run(TestLog.Writer,
                                        new AfterTestAction(testState).Run, "After Test"));

                                    if (invisibleTest)
                                        result = PublishOutcomeFromInvisibleTest(testCommand, primaryTestStep, outcome);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = ReportTestError(testCommand, parentTestStep, ex, String.Format("An exception occurred while preparing to run test '{0}'.", test.FullName));
                }
                finally
                {
                    if (parentContextCookie.HasValue)
                        parentContextCookie.Value.ExitContext();

                    executor.progressMonitor.SetStatus("");
                    executor.progressMonitor.Worked(1);
                }
            }
        }

        private sealed class RunTestInstanceAction
        {
            private readonly PatternTestExecutor executor;
            private readonly ITestCommand testCommand;
            private PatternTestInstanceState testInstanceState;
            private bool skipProtect;

            public RunTestInstanceAction(PatternTestExecutor executor, ITestCommand testCommand)
            {
                this.executor = executor;
                this.testCommand = testCommand;
            }

            public PatternTestInstanceState TestInstanceState
            {
                set { testInstanceState = value; }
            }

            public bool SkipProtect
            {
                set { skipProtect = value ; }
            }

            [DebuggerNonUserCode]
            public TestOutcome Run()
            {
                TestContext context = TestContext.CurrentContext;

                using (skipProtect ? null : context.Sandbox.Protect())
                {
                    try
                    {
                        if (executor.progressMonitor.IsCanceled)
                            return TestOutcome.Canceled;

                        var outcome = TestOutcome.Passed;

                        if (!executor.options.SkipTestExecution)
                        {
                            context.LifecyclePhase = LifecyclePhases.Initialize;

                            UpdateInterimOutcome(context, ref outcome,
                                context.Sandbox.Run(TestLog.Writer, new InitializeTestInstanceAction(testInstanceState).Run, "Initialize"));
                        }

                        if (outcome.Status == TestStatus.Passed)
                        {
                            if (!executor.options.SkipTestExecution)
                            {
                                context.LifecyclePhase = LifecyclePhases.SetUp;

                                UpdateInterimOutcome(context, ref outcome,
                                    context.Sandbox.Run(TestLog.Writer, new SetUpTestInstanceAction(testInstanceState).Run, "Set Up"));
                            }

                            if (outcome.Status == TestStatus.Passed)
                            {
                                if (!executor.options.SkipTestExecution)
                                {
                                    context.LifecyclePhase = LifecyclePhases.Execute;

                                    UpdateInterimOutcome(context, ref outcome,
                                        context.Sandbox.Run(TestLog.Writer, new ExecuteTestInstanceAction(testInstanceState).Run, "Execute"));
                                }

                                if (outcome.Status == TestStatus.Passed)
                                {
                                    // Run all test children.
                                    PatternTestActionsDecorator testActionsDecorator =
                                        delegate(Sandbox childSandbox, ref PatternTestActions childTestActions)
                                        {
                                            childTestActions = childTestActions.Copy();
                                            return childSandbox.Run(TestLog.Writer, new DecorateChildTestAction(testInstanceState, childTestActions).Run, "Decorate Child Test");
                                        };

                                    foreach (TestBatch batch in GenerateTestBatches(testCommand.Children))
                                    {
                                        if (batch.Commands.Count == 1)
                                        {
                                            // Special case to reduce effective stack depth and cost since there is no
                                            // need to use the scheduler if nothing is running in parallel.
                                            RunTestAction action = new RunTestAction(executor, batch.Commands[0], context, testActionsDecorator);

                                            action.Run();

                                            UpdateInterimOutcome(context, ref outcome, action.Result.Outcome.Generalize());
                                        }
                                        else
                                        {
                                            RunTestAction[] actions = GenericCollectionUtils.ConvertAllToArray(batch.Commands,
                                                childTestCommand => new RunTestAction(executor, childTestCommand, context, testActionsDecorator));

                                            executor.scheduler.Run(Array.ConvertAll<RunTestAction, Action>(actions, action => action.Run));

                                            TestOutcome combinedChildOutcome = TestOutcome.Passed;
                                            foreach (var action in actions)
                                                combinedChildOutcome = combinedChildOutcome.CombineWith(action.Result.Outcome);

                                            UpdateInterimOutcome(context, ref outcome, combinedChildOutcome.Generalize());
                                        }
                                    }
                                }
                            }

                            if (!executor.options.SkipTestExecution)
                            {
                                context.LifecyclePhase = LifecyclePhases.TearDown;

                                UpdateInterimOutcome(context, ref outcome,
                                    context.Sandbox.Run(TestLog.Writer, new TearDownTestInstanceAction(testInstanceState).Run, "Tear Down"));
                            }
                        }

                        if (!executor.options.SkipTestExecution)
                        {
                            context.LifecyclePhase = LifecyclePhases.Dispose;

                            UpdateInterimOutcome(context, ref outcome,
                                context.Sandbox.Run(TestLog.Writer, new DisposeTestInstanceAction(testInstanceState).Run, "Dispose"));
                        }

                        return outcome;
                    }
                    catch (Exception ex)
                    {
                        TestLog.Failures.WriteException(ex,
                            String.Format("An exception occurred while running test instance '{0}'.", testInstanceState.TestStep.Name));
                        return TestOutcome.Error;
                    }
                }
            }
        }

        private sealed class BeforeTestAction
        {
            private readonly PatternTestState testState;

            public BeforeTestAction(PatternTestState testState)
            {
                this.testState = testState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                foreach (PatternTestParameter parameter in testState.Test.Parameters)
                {
                    IDataAccessor accessor = parameter.Binder.Register(testState.BindingContext, parameter.DataContext);
                    testState.TestParameterDataAccessors.Add(parameter, accessor);
                }

                testState.TestActions.BeforeTestChain.Action(testState);
            }
        }

        private sealed class InitializeTestAction
        {
            private readonly PatternTestState testState;

            public InitializeTestAction(PatternTestState testState)
            {
                this.testState = testState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testState.TestActions.InitializeTestChain.Action(testState);
            }
        }

        private sealed class DisposeTestAction
        {
            private readonly PatternTestState testState;

            public DisposeTestAction(PatternTestState testState)
            {
                this.testState = testState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testState.TestActions.DisposeTestChain.Action(testState);
            }
        }

        private sealed class AfterTestAction
        {
            private readonly PatternTestState testState;

            public AfterTestAction(PatternTestState testState)
            {
                this.testState = testState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testState.TestActions.AfterTestChain.Action(testState);

                testState.TestParameterDataAccessors.Clear();
            }
        }

        private sealed class DecorateTestInstanceAction
        {
            private readonly PatternTestState testState;
            private readonly PatternTestInstanceActions decoratedTestInstanceActions;

            public DecorateTestInstanceAction(PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
            {
                this.testState = testState;
                this.decoratedTestInstanceActions = decoratedTestInstanceActions;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testState.TestActions.DecorateTestInstanceChain.Action(testState, decoratedTestInstanceActions);
            }
        }

        private sealed class BeforeTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public BeforeTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                foreach (KeyValuePair<PatternTestParameter, IDataAccessor> accessorPair in testInstanceState.TestState.TestParameterDataAccessors)
                {
                    object value = Bind(accessorPair.Value, testInstanceState.BindingItem, testInstanceState.Formatter);
                    testInstanceState.TestParameterValues.Add(accessorPair.Key, value);
                }

                foreach (KeyValuePair<PatternTestParameter, object> dataPair in testInstanceState.TestParameterValues)
                {
                    dataPair.Key.TestParameterActions.BindTestParameter(testInstanceState, dataPair.Value);
                }

                testInstanceState.TestInstanceActions.BeforeTestInstanceChain.Action(testInstanceState);
            }
        }

        private sealed class InitializeTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public InitializeTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.InitializeTestInstanceChain.Action(testInstanceState);
            }
        }

        private sealed class SetUpTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public SetUpTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.SetUpTestInstanceChain.Action(testInstanceState);
            }
        }

        private sealed class ExecuteTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public ExecuteTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.ExecuteTestInstanceChain.Action(testInstanceState);
            }
        }

        private sealed class RunTestInstanceBodyAction
        {
            private readonly PatternTestInstanceState testInstanceState;
            private TestOutcome outcome;

            public RunTestInstanceBodyAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
                outcome = TestOutcome.Error;
            }

            public TestOutcome Outcome
            {
                get { return outcome; }
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                outcome = testInstanceState.TestInstanceActions.RunTestInstanceBodyChain.Func(testInstanceState);
            }

            public static bool CanOptimizeCallChainAndInvokeBodyActionDirectly(PatternTestInstanceState testInstanceState)
            {
                return testInstanceState.TestInstanceActions.IsDefaultRunTestInstanceBodyChain;
            }
        }

        private sealed class TearDownTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public TearDownTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.TearDownTestInstanceChain.Action(testInstanceState);
            }
        }

        private sealed class DisposeTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public DisposeTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.DisposeTestInstanceChain.Action(testInstanceState);
            }
        }

        private sealed class AfterTestInstanceAction
        {
            private readonly PatternTestInstanceState testInstanceState;

            public AfterTestInstanceAction(PatternTestInstanceState testInstanceState)
            {
                this.testInstanceState = testInstanceState;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.AfterTestInstanceChain.Action(testInstanceState);

                foreach (KeyValuePair<PatternTestParameter, object> dataPair in testInstanceState.TestParameterValues)
                {
                    dataPair.Key.TestParameterActions.UnbindTestParameter(testInstanceState, dataPair.Value);
                }

                testInstanceState.TestParameterValues.Clear();
            }
        }

        private sealed class DecorateChildTestAction
        {
            private readonly PatternTestInstanceState testInstanceState;
            private readonly PatternTestActions decoratedChildTestActions;

            public DecorateChildTestAction(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildTestActions)
            {
                this.testInstanceState = testInstanceState;
                this.decoratedChildTestActions = decoratedChildTestActions;
            }

            [UserCodeEntryPoint, DebuggerNonUserCode]
            public void Run()
            {
                testInstanceState.TestInstanceActions.DecorateChildTestChain.Action(testInstanceState, decoratedChildTestActions);
            }
        }
        #endregion

        private static object Bind(IDataAccessor accessor, IDataItem bindingItem, IFormatter formatter)
        {
            try
            {
                return accessor.GetValue(bindingItem);
            }
            catch (DataBindingException ex)
            {
                using (TestLog.Failures.BeginSection("Data binding failure"))
                {
                    if (!string.IsNullOrEmpty(ex.Message))
                        TestLog.Failures.WriteLine(ex.Message);

                    bool first = true;
                    foreach (DataBinding binding in bindingItem.GetBindingsForInformalDescription())
                    {
                        if (first)
                        {
                            TestLog.Failures.Write("\nAvailable data bindings for this item:\n\n");
                            first = false;
                        }

                        using (TestLog.Failures.BeginMarker(Marker.Label))
                        {
                            TestLog.Failures.Write(binding);
                            TestLog.Failures.Write(": ");
                        }

                        TestLog.Failures.WriteLine(bindingItem.GetValue(binding));
                    }

                    if (first)
                        TestLog.Failures.Write("\nThis item does not appear to provide any data bindings.\n");
                }

                throw new SilentTestException(TestOutcome.Error);
            }
        }

        private static TestResult ReportTestError(ITestCommand testCommand, Model.Tree.TestStep parentTestStep, Exception ex, string message)
        {
            ITestContext context = testCommand.StartPrimaryChildStep(parentTestStep);
            TestLog.Failures.WriteException(ex, message);
            return context.FinishStep(TestOutcome.Error, null);
        }

        private static IEnumerable<TestBatch> GenerateTestBatches(IEnumerable<ITestCommand> commands)
        {
            foreach (TestPartition partition in GenerateTestPartitions(commands))
            {
                if (partition.ParallelCommands.Count > 0)
                    yield return new TestBatch(partition.ParallelCommands);

                foreach (ITestCommand command in partition.SequentialCommands)
                    yield return new TestBatch(new [] { command });
            }
        }

        private static IEnumerable<TestPartition> GenerateTestPartitions(IEnumerable<ITestCommand> commands)
        {
            TestPartition currentPartition = null;
            int currentOrder = int.MinValue;
            foreach (ITestCommand command in commands)
            {
                if (command.Test.Order != currentOrder)
                {
                    if (currentPartition != null)
                    {
                        yield return currentPartition;
                        currentPartition = null;
                    }

                    currentOrder = command.Test.Order;
                }

                if (currentPartition == null)
                    currentPartition = new TestPartition();

                PatternTest test = command.Test as PatternTest;
                if (test != null && test.IsParallelizable && command.Dependencies.Count == 0)
                {
                    currentPartition.ParallelCommands.Add(command);
                }
                else
                {
                    currentPartition.SequentialCommands.Add(command);
                }
            }

            if (currentPartition != null)
                yield return currentPartition;
        }

        private sealed class TestBatch
        {
            public TestBatch(IList<ITestCommand> commands)
            {
                Commands = commands;
            }

            public IList<ITestCommand> Commands { get; private set; }
        }

        private sealed class TestPartition
        {
            public TestPartition()
            {
                ParallelCommands = new List<ITestCommand>();
                SequentialCommands = new List<ITestCommand>();
            }

            public IList<ITestCommand> ParallelCommands { get; private set; }
            public IList<ITestCommand> SequentialCommands { get; private set; }
        }
    }
}
