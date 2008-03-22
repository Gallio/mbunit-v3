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
using Gallio.Framework.Pattern;
using Gallio;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IPatternTestHandler" /> based on
    /// actions that can be sequenced and composed as chains.
    /// </para>
    /// <para>
    /// This class is the backbone of the <see cref="PatternTestFramework" />
    /// extensibility model.  Because new behaviors can be defined and added
    /// to action chains at will by any component that participates in the test
    /// construction process (typically an <see cref="IPattern" />), the framework
    /// itself does not need to hardcode the mechanism by which the behaviors are
    /// selected.
    /// </para>
    /// </summary>
    /// <seealso cref="IPatternTestHandler" /> for documentation about the behaviors themselves.
    public class PatternTestActions : IPatternTestHandler
    {
        private readonly ActionChain<PatternTestState> beforeTestChain;
        private readonly ActionChain<PatternTestState> initializeTestChain;
        private readonly ActionChain<PatternTestState> disposeTestChain;
        private readonly ActionChain<PatternTestState> afterTestChain;
        private readonly ActionChain<PatternTestState, PatternTestInstanceActions> decorateTestInstanceChain;

        private PatternTestInstanceActions testInstanceActions;

        /// <summary>
        /// Creates a test actions object initially configured with empty action chains
        /// that do nothing.
        /// </summary>
        public PatternTestActions()
        {
            beforeTestChain = new ActionChain<PatternTestState>();
            initializeTestChain = new ActionChain<PatternTestState>();
            disposeTestChain = new ActionChain<PatternTestState>();
            afterTestChain = new ActionChain<PatternTestState>();
            decorateTestInstanceChain = new ActionChain<PatternTestState, PatternTestInstanceActions>();
        }

        /// <summary>
        /// <para>
        /// Creates a new <see cref="PatternTestActions" /> object initially configured
        /// to forward calls to the specified handler without change.
        /// The result is that any behaviors added to the action chains of the returned
        /// <see cref="PatternTestActions" /> will be invoked before, after or around
        /// those of the specified handler. 
        /// </para>
        /// <para>
        /// A pattern test decorator applies additional actions around those of
        /// another <see cref="IPatternTestHandler" /> for a <see cref="PatternTest"/>.
        /// </para>
        /// </summary>
        /// <param name="handler">The handler to decorate</param>
        /// <returns>The decorated handler actions</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null</exception>
        public static PatternTestActions CreateDecorator(IPatternTestHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            PatternTestActions decorator = new PatternTestActions();
            decorator.beforeTestChain.Action = handler.BeforeTest;
            decorator.initializeTestChain.Action = handler.InitializeTest;
            decorator.disposeTestChain.Action = handler.DisposeTest;
            decorator.afterTestChain.Action = handler.AfterTest;
            decorator.decorateTestInstanceChain.Action = handler.DecorateTestInstance;

            decorator.testInstanceActions = PatternTestInstanceActions.CreateDecorator(handler.TestInstanceHandler);
            return decorator;
        }

        /// <summary>
        /// Gets the test instance actions that describes the lifecycle of a test instance.
        /// </summary>
        /// <remarks>
        /// These actions may be further decorated on a per-instance basis using
        /// <see cref="DecorateTestInstanceChain" />.
        /// </remarks>
        public PatternTestInstanceActions TestInstanceActions
        {
            get
            {
                if (testInstanceActions == null)
                    testInstanceActions = new PatternTestInstanceActions();
                return testInstanceActions;
            }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.BeforeTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.BeforeTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestState> BeforeTestChain
        {
            get { return beforeTestChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.InitializeTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.InitializeTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestState> InitializeTestChain
        {
            get { return initializeTestChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.DisposeTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.DisposeTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestState> DisposeTestChain
        {
            get { return disposeTestChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.AfterTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.AfterTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestState> AfterTestChain
        {
            get { return afterTestChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.DecorateTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.DecorateTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestState, PatternTestInstanceActions> DecorateTestInstanceChain
        {
            get { return decorateTestInstanceChain; }
        }

        /// <inheritdoc />
        public IPatternTestInstanceHandler TestInstanceHandler
        {
            get { return TestInstanceActions; }
        }

        /// <inheritdoc />
        public void BeforeTest(PatternTestState testState)
        {
            beforeTestChain.Action(testState);
        }

        /// <inheritdoc />
        public void InitializeTest(PatternTestState testState)
        {
            initializeTestChain.Action(testState);
        }

        /// <inheritdoc />
        public void DisposeTest(PatternTestState testState)
        {
            disposeTestChain.Action(testState);
        }

        /// <inheritdoc />
        public void AfterTest(PatternTestState testState)
        {
            afterTestChain.Action(testState);
        }

        /// <inheritdoc />
        public void DecorateTestInstance(PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions)
        {
            decorateTestInstanceChain.Action(testState, decoratedTestInstanceActions);
        }
    }
}
