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
        private readonly ActionChain<PatternTestState> afterTestChain;

        private readonly ActionChain<PatternTestInstanceState> beforeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> initializeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> setUpTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> executeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> tearDownTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> disposeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> afterTestInstanceChain;

        private readonly ActionChain<PatternTestInstanceState, PatternTestActions> decorateChildTestChain;

        /// <summary>
        /// Creates a test actions object initially configured with empty action chains
        /// that do nothing.
        /// </summary>
        public PatternTestActions()
        {
            beforeTestChain = new ActionChain<PatternTestState>();
            afterTestChain = new ActionChain<PatternTestState>();

            beforeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            initializeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            setUpTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            executeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            tearDownTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            disposeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            afterTestInstanceChain = new ActionChain<PatternTestInstanceState>();

            decorateChildTestChain = new ActionChain<PatternTestInstanceState, PatternTestActions>();
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
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null</exception>
        public static PatternTestActions CreateDecorator(IPatternTestHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            PatternTestActions decorator = new PatternTestActions();
            decorator.BeforeTestChain.Action = handler.BeforeTest;
            decorator.AfterTestChain.Action = handler.AfterTest;

            decorator.BeforeTestInstanceChain.Action = handler.BeforeTestInstance;
            decorator.InitializeTestInstanceChain.Action = handler.InitializeTestInstance;
            decorator.SetUpTestInstanceChain.Action = handler.SetUpTestInstance;
            decorator.ExecuteTestInstanceChain.Action = handler.ExecuteTestInstance;
            decorator.TearDownTestInstanceChain.Action = handler.TearDownTestInstance;
            decorator.DisposeTestInstanceChain.Action = handler.DisposeTestInstance;
            decorator.AfterTestInstanceChain.Action = handler.AfterTestInstance;

            decorator.DecorateChildTestChain.Action = handler.DecorateChildTest;
            return decorator;
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
        /// Gets the chain of <see cref="IPatternTestHandler.AfterTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.AfterTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestState> AfterTestChain
        {
            get { return afterTestChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.BeforeTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.BeforeTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> BeforeTestInstanceChain
        {
            get { return beforeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.InitializeTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.InitializeTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> InitializeTestInstanceChain
        {
            get { return initializeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.SetUpTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.SetUpTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> SetUpTestInstanceChain
        {
            get { return setUpTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.ExecuteTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.ExecuteTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> ExecuteTestInstanceChain
        {
            get { return executeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.TearDownTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.TearDownTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> TearDownTestInstanceChain
        {
            get { return tearDownTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.DisposeTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.DisposeTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> DisposeTestInstanceChain
        {
            get { return disposeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.AfterTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.AfterTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> AfterTestInstanceChain
        {
            get { return afterTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestHandler.DecorateChildTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestHandler.DecorateChildTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState, PatternTestActions> DecorateChildTestChain
        {
            get { return decorateChildTestChain; }
        }

        /// <inheritdoc />
        public void BeforeTest(PatternTestState testState)
        {
            beforeTestChain.Action(testState);
        }

        /// <inheritdoc />
        public void AfterTest(PatternTestState testState)
        {
            afterTestChain.Action(testState);
        }

        /// <inheritdoc />
        public void BeforeTestInstance(PatternTestInstanceState testInstanceState)
        {
            beforeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void InitializeTestInstance(PatternTestInstanceState testInstanceState)
        {
            initializeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void SetUpTestInstance(PatternTestInstanceState testInstanceState)
        {
            setUpTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void ExecuteTestInstance(PatternTestInstanceState testInstanceState)
        {
            executeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void TearDownTestInstance(PatternTestInstanceState testInstanceState)
        {
            tearDownTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void DisposeTestInstance(PatternTestInstanceState testInstanceState)
        {
            disposeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void AfterTestInstance(PatternTestInstanceState testInstanceState)
        {
            afterTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        public void DecorateChildTest(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildActions)
        {
            decorateChildTestChain.Action(testInstanceState, decoratedChildActions);
        }
    }
}
