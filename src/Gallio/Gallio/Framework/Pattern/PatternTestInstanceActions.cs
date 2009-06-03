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
using Gallio.Common;
using Gallio.Model;
using Gallio.Common.Diagnostics;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="IPatternTestInstanceHandler" /> based on
    /// actions that can be sequenced and composed as chains.
    /// </para>
    /// </summary>
    /// <seealso cref="IPatternTestInstanceHandler" /> for documentation about the behaviors themselves.
    /// <seealso cref="PatternTestActions"/> for actions on tests.
    public class PatternTestInstanceActions : IPatternTestInstanceHandler
    {
        private readonly ActionChain<PatternTestInstanceState> beforeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> initializeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> setUpTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> executeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> tearDownTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> disposeTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState> afterTestInstanceChain;
        private readonly ActionChain<PatternTestInstanceState, PatternTestActions> decorateChildTestChain;
        private readonly FuncChain<PatternTestInstanceState, TestOutcome> runTestInstanceBodyChain;

        /// <summary>
        /// Creates a test instance actions object initially configured with empty action chains
        /// that do nothing.
        /// </summary>
        public PatternTestInstanceActions()
        {
            beforeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            initializeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            setUpTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            executeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            tearDownTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            disposeTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            afterTestInstanceChain = new ActionChain<PatternTestInstanceState>();
            decorateChildTestChain = new ActionChain<PatternTestInstanceState, PatternTestActions>();
            runTestInstanceBodyChain = new FuncChain<PatternTestInstanceState, TestOutcome>(state => state.RunBody());
        }

        /// <summary>
        /// <para>
        /// Creates a new <see cref="PatternTestInstanceActions" /> object initially configured
        /// to forward calls to the specified handler without change.
        /// The result is that any behaviors added to the action chains of the returned
        /// <see cref="PatternTestInstanceActions" /> will be invoked before, after or around
        /// those of the specified handler. 
        /// </para>
        /// <para>
        /// A pattern test decorator applies additional actions around those of
        /// another <see cref="IPatternTestInstanceHandler" /> for a <see cref="PatternTestStep"/>.
        /// </para>
        /// </summary>
        /// <param name="handler">The handler to decorate.</param>
        /// <returns>The decorated handler actions</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="handler"/> is null.</exception>
        public static PatternTestInstanceActions CreateDecorator(IPatternTestInstanceHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            PatternTestInstanceActions decorator = new PatternTestInstanceActions();
            decorator.beforeTestInstanceChain.Action = handler.BeforeTestInstance;
            decorator.initializeTestInstanceChain.Action = handler.InitializeTestInstance;
            decorator.setUpTestInstanceChain.Action = handler.SetUpTestInstance;
            decorator.executeTestInstanceChain.Action = handler.ExecuteTestInstance;
            decorator.tearDownTestInstanceChain.Action = handler.TearDownTestInstance;
            decorator.disposeTestInstanceChain.Action = handler.DisposeTestInstance;
            decorator.afterTestInstanceChain.Action = handler.AfterTestInstance;
            decorator.decorateChildTestChain.Action = handler.DecorateChildTest;
            decorator.runTestInstanceBodyChain.Func = handler.RunTestInstanceBody;
            return decorator;
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.BeforeTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.BeforeTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> BeforeTestInstanceChain
        {
            get { return beforeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.InitializeTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.InitializeTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> InitializeTestInstanceChain
        {
            get { return initializeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.SetUpTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.SetUpTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> SetUpTestInstanceChain
        {
            get { return setUpTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.ExecuteTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.ExecuteTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> ExecuteTestInstanceChain
        {
            get { return executeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.TearDownTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.TearDownTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> TearDownTestInstanceChain
        {
            get { return tearDownTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.DisposeTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.DisposeTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> DisposeTestInstanceChain
        {
            get { return disposeTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.AfterTestInstance" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.AfterTestInstance"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState> AfterTestInstanceChain
        {
            get { return afterTestInstanceChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.DecorateChildTest" /> actions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.DecorateChildTest"/> for details about the semantics of these actions.
        public ActionChain<PatternTestInstanceState, PatternTestActions> DecorateChildTestChain
        {
            get { return decorateChildTestChain; }
        }

        /// <summary>
        /// Gets the chain of <see cref="IPatternTestInstanceHandler.RunTestInstanceBody" /> functions.
        /// </summary>
        /// <seealso cref="IPatternTestInstanceHandler.RunTestInstanceBody"/> for details about the semantics of these functions.
        public FuncChain<PatternTestInstanceState, TestOutcome> RunTestInstanceBodyChain
        {
            get { return runTestInstanceBodyChain; }
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void BeforeTestInstance(PatternTestInstanceState testInstanceState)
        {
            beforeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void InitializeTestInstance(PatternTestInstanceState testInstanceState)
        {
            initializeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void SetUpTestInstance(PatternTestInstanceState testInstanceState)
        {
            setUpTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void ExecuteTestInstance(PatternTestInstanceState testInstanceState)
        {
            executeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void TearDownTestInstance(PatternTestInstanceState testInstanceState)
        {
            tearDownTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void DisposeTestInstance(PatternTestInstanceState testInstanceState)
        {
            disposeTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void AfterTestInstance(PatternTestInstanceState testInstanceState)
        {
            afterTestInstanceChain.Action(testInstanceState);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public void DecorateChildTest(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildTestActions)
        {
            decorateChildTestChain.Action(testInstanceState, decoratedChildTestActions);
        }

        /// <inheritdoc />
        [UserCodeEntryPoint]
        public TestOutcome RunTestInstanceBody(PatternTestInstanceState testInstanceState)
        {
            return runTestInstanceBodyChain.Func(testInstanceState);
        }
    }
}