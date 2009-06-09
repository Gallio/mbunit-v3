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
using System.Threading;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// An abstract base class for implementing new test decorator behaviors.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test decorator applies actions to be performed around the initialization, setup,
    /// execution, teardown and disposal lifecycle of a test (in that order).
    /// </para>
    /// <para>
    /// This abstract class provides a convenient way to implement new test decorators
    /// of your own.  To add new behavior to the decorator, override the appropriate
    /// lifecycle methods, such as <see cref="Execute" />.
    /// </para>
    /// <para>
    /// If you need more control over how the test is decorated, you may
    /// prefer subclassing <see cref="TestDecoratorPatternAttribute" /> directly instead.
    /// </para>
    /// <para>
    /// When multiple test decorators are applied to a test, they are installed
    /// in order according to the <see cref="DecoratorPatternAttribute.Order" /> property.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = true, Inherited = true)]
    public abstract class TestDecoratorAttribute : TestDecoratorPatternAttribute
    {
        private static int nextUniqueId;
        private readonly Key<Action<PatternTestInstanceState>> defaultActionKey = new Key<Action<PatternTestInstanceState>>(@"DefaultAction:" + Interlocked.Increment(ref nextUniqueId));

        /// <summary>
        /// Initializes the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Override this method to add your own behavior around the default actions.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null.</param>
        /// <seealso cref="IPatternTestInstanceHandler.InitializeTestInstance"/>
        protected virtual void Initialize(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Sets up the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Override this method to add your own behavior around the default actions.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null.</param>
        /// <seealso cref="IPatternTestInstanceHandler.SetUpTestInstance"/>
        protected virtual void SetUp(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Executes the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Override this method to add your own behavior around the default actions.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null.</param>
        /// <seealso cref="IPatternTestInstanceHandler.ExecuteTestInstance"/>
        protected virtual void Execute(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Tears down the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Override this method to add your own behavior around the default actions.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null.</param>
        /// <seealso cref="IPatternTestInstanceHandler.TearDownTestInstance"/>
        protected virtual void TearDown(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <summary>
        /// Disposes the test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Override this method to add your own behavior around the default actions.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, not null.</param>
        /// <seealso cref="IPatternTestInstanceHandler.DisposeTestInstance"/>
        protected virtual void Dispose(PatternTestInstanceState testInstanceState)
        {
            InvokeDefaultAction(testInstanceState);
        }

        /// <inheritdoc />
        protected override void DecorateTest(IPatternScope scope, ICodeElementInfo codeElement)
        {
            // Note: We do this as a test instance decorator to ensure that all of
            //       the setup actions registered by the test or its containing fixture
            //       can be wrapped because this is the last possible opportunity for new
            //       actions to be added.
            scope.TestBuilder.TestActions.DecorateTestInstanceChain.After(delegate(PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
            {
                WrapDefaultAction(decoratedTestInstanceActions.InitializeTestInstanceChain, Initialize);
                WrapDefaultAction(decoratedTestInstanceActions.SetUpTestInstanceChain, SetUp);
                WrapDefaultAction(decoratedTestInstanceActions.ExecuteTestInstanceChain, Execute);
                WrapDefaultAction(decoratedTestInstanceActions.TearDownTestInstanceChain, TearDown);
                WrapDefaultAction(decoratedTestInstanceActions.DisposeTestInstanceChain, Dispose);
            });
        }

        private void WrapDefaultAction(ActionChain<PatternTestInstanceState> chain, Action<PatternTestInstanceState> decoratorAction)
        {
            chain.Around((testInstanceState, decoratedAction) =>
            {
                try
                {
                    testInstanceState.Data.SetValue(defaultActionKey, decoratedAction);
                    decoratorAction(testInstanceState);
                }
                finally
                {
                    testInstanceState.Data.RemoveValue(defaultActionKey);
                }
            });
        }

        private void InvokeDefaultAction(PatternTestInstanceState testInstanceState)
        {
            Action<PatternTestInstanceState> action = testInstanceState.Data.GetValue(defaultActionKey);
            action(testInstanceState);
        }
    }
}