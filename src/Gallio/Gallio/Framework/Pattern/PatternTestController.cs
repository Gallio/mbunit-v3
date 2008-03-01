// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Framework.Data.Binders;
using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using Gallio.Reflection;
using Gallio.Model.Execution;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Logging;
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Controls the execution of <see cref="PatternTest" /> instances.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestController : ITestController
    {
        private readonly IConverter converter;
        private readonly IFormatter formatter;

        /// <summary>
        /// Creates a pattern test controller.
        /// </summary>
        /// <param name="formatter">The formatter for data binding</param>
        /// <param name="converter">The converter for data binding</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="formatter"/>
        /// or <paramref name="converter"/> is null</exception>
        public PatternTestController(IFormatter formatter, IConverter converter)
        {
            if (formatter == null)
                throw new ArgumentNullException("formatter");
            if (converter == null)
                throw new ArgumentNullException("converter");

            this.formatter = formatter;
            this.converter = converter;
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void RunTests(IProgressMonitor progressMonitor, ITestCommand rootTestCommand,
            ITestInstance parentTestInstance)
        {
            using (progressMonitor)
            {
                progressMonitor.BeginTask("Running tests.", rootTestCommand.TestCount);

                RunTest(progressMonitor, rootTestCommand, parentTestInstance, null);
            }
        }

        internal IEnumerable<ITestInstance> GetInstances(PatternTest test, ITestInstance parentTestInstance, bool guessDynamicInstances)
        {
            List<ITestInstance> testInstances = new List<ITestInstance>();

            // TODO: Support child decorators and context from the containing test.
            ProcessTest(test, parentTestInstance, guessDynamicInstances, null, delegate(PatternTestInstanceState testInstanceState)
            {
                testInstances.Add(testInstanceState.TestInstance);
                return true;
            }, null);

            return testInstances;
        }

        private bool RunTest(IProgressMonitor progressMonitor, ITestCommand testCommand, ITestInstance parentTestInstance,
            Func<IPatternTestHandler, IPatternTestHandler> decorator)
        {
            progressMonitor.SetStatus(String.Format("Run test: {0}.", testCommand.Test.Name));

            PatternTest test = (PatternTest)testCommand.Test;
            bool success = ProcessTest(test, parentTestInstance, true, decorator, delegate(PatternTestInstanceState testInstanceState)
            {
                progressMonitor.SetStatus(String.Format("Run test instance: {0}.", testInstanceState.TestInstance.Name));

                ITestContext context = testCommand.StartRootStep(new BaseTestStep(testInstanceState.TestInstance));
                TestOutcome outcome = RunTestInstanceWithContext(progressMonitor, testCommand, testInstanceState, context);
                context.FinishStep(outcome, null);
                return outcome.Status == TestStatus.Passed;
            }, delegate(Exception ex, string message)
            {
                ITestContext context = testCommand.StartRootStep(parentTestInstance);
                Log.Failures.WriteException(ex, message);
                context.FinishStep(TestOutcome.Failed, null);
            });

            progressMonitor.Worked(1);
            return success;
        }

        private bool ProcessTest(PatternTest test, ITestInstance parentTestInstance, bool includeDynamicInstances,
            Func<IPatternTestHandler, IPatternTestHandler> decorator, Func<PatternTestInstanceState, bool> processor,
            Action<Exception, string> errorReporter)
        {
            try
            {
                IPatternTestHandler testHandler = test.TestActions;
                if (decorator != null)
                {
                    testHandler = decorator(testHandler);
                    if (testHandler == null)
                        return false;
                }

                PatternTestState testState = new PatternTestState(test, testHandler, converter, formatter);

                bool success = DoBeforeTest(testState);
                if (success)
                    success = ProcessTestInstances(parentTestInstance, testState, includeDynamicInstances, processor, errorReporter);

                success &= DoAfterTest(testState);
                return success;
            }
            catch (Exception ex)
            {
                if (errorReporter != null)
                    errorReporter(ex, String.Format("An exception occurred while preparing to run test '{0}'.", test.FullName));
                return false;
            }
        }

        private static bool ProcessTestInstances(ITestInstance parentTestInstance, PatternTestState testState, bool includeDynamicInstances,
            Func<PatternTestInstanceState, bool> processor, Action<Exception, string> errorReporter)
        {
            try
            {
                bool success = true;
                foreach (DataBindingItem item in testState.BindingContext.GetItems(includeDynamicInstances))
                {
                    success &= ProcessTestInstance(parentTestInstance, testState, item, processor, errorReporter);
                }

                return success;
            }
            catch (Exception ex)
            {
                if (errorReporter != null)
                    errorReporter(ex, String.Format("An exception occurred while getting data items for test '{0}'.", testState.Test.FullName));
                return false;
            }
        }

        private static bool ProcessTestInstance(ITestInstance parentTestInstance, PatternTestState testState,
            DataBindingItem bindingItem, Func<PatternTestInstanceState, bool> processor, Action<Exception, string> errorReporter)
        {
            try
            {
                using (bindingItem)
                {
                    PatternTestInstanceActions decoratedTestInstanceActions =
                        PatternTestInstanceActions.CreateDecorator(testState.TestHandler.TestInstanceHandler);

                    if (!DoDecorateTestInstance(testState, decoratedTestInstanceActions))
                        return false;

                    PatternTestInstance testInstance = new PatternTestInstance(testState.Test, parentTestInstance,
                        testState.Test.Name, bindingItem.GetRow().IsDynamic);

                    PatternTestInstanceState testInstanceState = new PatternTestInstanceState(testInstance, decoratedTestInstanceActions, testState, bindingItem);

                    bool success = DoBeforeTestInstance(testInstanceState);
                    if (success)
                        success = processor(testInstanceState);

                    success &= DoAfterTestInstance(testInstanceState);
                    return success;
                }
            }
            catch (Exception ex)
            {
                if (errorReporter != null)
                    errorReporter(ex, String.Format("An exception occurred while preparing an instance of test '{0}'.", testState.Test.FullName));
                return false;
            }
        }

        private TestOutcome RunTestInstanceWithContext(IProgressMonitor progressMonitor, ITestCommand testCommand,
            PatternTestInstanceState testInstanceState, ITestContext context)
        {
            try
            {
                TestOutcome outcome = context.Outcome = DoInitializeTestInstance(testInstanceState, context);
                if (outcome.Status == TestStatus.Passed)
                {
                    outcome = context.Outcome = DoSetUpTestInstance(testInstanceState, context);
                    if (outcome.Status == TestStatus.Passed)
                    {
                        outcome = context.Outcome = DoExecuteTestInstance(testInstanceState, context);

                        if (outcome.Status == TestStatus.Passed)
                        {
                            foreach (ITestCommand childTestCommand in testCommand.Children)
                            {
                                bool childSuccess = RunTest(progressMonitor, childTestCommand, testInstanceState.TestInstance,
                                    delegate(IPatternTestHandler childHandler)
                                    {
                                        PatternTestActions decoratedChildTestActions = PatternTestActions.CreateDecorator(childHandler);
                                        if (!DoDecorateChildTest(testInstanceState, decoratedChildTestActions))
                                            return null;
                                        return decoratedChildTestActions;
                                    });

                                outcome = context.Outcome = outcome.CombineWith(childSuccess ? TestOutcome.Passed : TestOutcome.Failed);
                            }
                        }
                    }

                    outcome = context.Outcome = outcome.CombineWith(DoTearDownTestInstance(testInstanceState, context));
                }

                outcome = context.Outcome = outcome.CombineWith(DoDisposeTestInstance(testInstanceState, context));
                return outcome;
            }
            catch (Exception ex)
            {
                Log.Failures.WriteException(ex, "An exception occurred while running test instance '{0}'.", testInstanceState.TestInstance.Name);
                return TestOutcome.Error;
            }
        }

        private static bool DoBeforeTest(PatternTestState testState)
        {
            foreach (PatternTestParameter parameter in testState.Test.Parameters)
            {
                IDataBindingAccessor bindingAccessor = parameter.Binder.Register(testState.BindingContext, parameter);
                testState.SlotBindingAccessors.Add(new KeyValuePair<ISlotInfo, IDataBindingAccessor>(parameter.Slot, bindingAccessor));
            }

            return InvokeActionWithPassFail(delegate
            {
                testState.TestHandler.BeforeTest(testState);
            }, "Before Test", null);
        }

        private static bool DoAfterTest(PatternTestState testState)
        {
            return InvokeActionWithPassFail(delegate
            {
                testState.TestHandler.AfterTest(testState);
            }, "After Test", null);
        }

        private static bool DoDecorateTestInstance(PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
        {
            return InvokeActionWithPassFail(delegate
            {
                testState.TestHandler.DecorateTestInstance(testState, decoratedTestInstanceActions);
            }, "Decorate Child Test", null);
        }

        private static bool DoBeforeTestInstance(PatternTestInstanceState testInstanceState)
        {
            foreach (KeyValuePair<string, string> entry in testInstanceState.BindingItem.GetRow().GetMetadata())
                testInstanceState.TestInstance.Metadata.Add(entry.Key, entry.Value);

            if (testInstanceState.TestState.SlotBindingAccessors.Count != 0)
            {
                foreach (KeyValuePair<ISlotInfo, IDataBindingAccessor> entry in testInstanceState.TestState.SlotBindingAccessors)
                    testInstanceState.SlotValues.Add(entry.Key, entry.Value.GetValue(testInstanceState.BindingItem));
            }

            return InvokeActionWithPassFail(delegate
            {
                testInstanceState.TestInstanceHandler.BeforeTestInstance(testInstanceState);
            }, "Before Test Instance", null);
        }

        private static TestOutcome DoInitializeTestInstance(PatternTestInstanceState testInstanceState, ITestContext context)
        {
            context.LifecyclePhase = LifecyclePhases.Initialize;

            return InvokeActionWithOutcome(delegate
            {
                testInstanceState.TestInstanceHandler.InitializeTestInstance(testInstanceState);
            }, "Initialize", null);
        }

        private static TestOutcome DoSetUpTestInstance(PatternTestInstanceState testInstanceState, ITestContext context)
        {
            context.LifecyclePhase = LifecyclePhases.SetUp;

            return InvokeActionWithOutcome(delegate
            {
                testInstanceState.TestInstanceHandler.SetUpTestInstance(testInstanceState);
            }, "Set Up", null);
        }

        private static TestOutcome DoExecuteTestInstance(PatternTestInstanceState testInstanceState, ITestContext context)
        {
            context.LifecyclePhase = LifecyclePhases.Execute;

            string expectedExceptionType = testInstanceState.TestInstance.Metadata.GetValue(MetadataKeys.ExpectedException)
                ?? testInstanceState.Test.Metadata.GetValue(MetadataKeys.ExpectedException);

            return InvokeActionWithOutcome(delegate
            {
                testInstanceState.TestInstanceHandler.ExecuteTestInstance(testInstanceState);
            }, null, expectedExceptionType);
        }

        private static TestOutcome DoTearDownTestInstance(PatternTestInstanceState testInstanceState, ITestContext context)
        {
            context.LifecyclePhase = LifecyclePhases.TearDown;

            return InvokeActionWithOutcome(delegate
            {
                testInstanceState.TestInstanceHandler.TearDownTestInstance(testInstanceState);
            }, "Tear Down", null);
        }

        private static TestOutcome DoDisposeTestInstance(PatternTestInstanceState testInstanceState, ITestContext context)
        {
            context.LifecyclePhase = LifecyclePhases.Dispose;

            return InvokeActionWithOutcome(delegate
            {
                testInstanceState.TestInstanceHandler.DisposeTestInstance(testInstanceState);
            }, "Dispose", null);
        }

        private static bool DoAfterTestInstance(PatternTestInstanceState testInstanceState)
        {
            return InvokeActionWithPassFail(delegate
            {
                testInstanceState.TestInstanceHandler.AfterTestInstance(testInstanceState);
            }, "After Test Instance", null);
        }

        private static bool DoDecorateChildTest(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildTestActions)
        {
            return InvokeActionWithPassFail(delegate
            {
                testInstanceState.TestInstanceHandler.DecorateChildTest(testInstanceState, decoratedChildTestActions);
            }, "Decorate Child Test", null);
        }

        private static bool InvokeActionWithPassFail(Action action, string description,
            string expectedExceptionType)
        {
            return InvokeActionWithOutcome(action, description, expectedExceptionType).Status == TestStatus.Passed;
        }

        private static TestOutcome InvokeActionWithOutcome(Action action, string description,
            string expectedExceptionType)
        {
            return TestActionInvoker.Run(action, description, expectedExceptionType);
        }
    }
}