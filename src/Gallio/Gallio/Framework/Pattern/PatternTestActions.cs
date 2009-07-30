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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Pattern test action provide the logic that implements the various
    /// phases of the test execution lifecycle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each chain of actions represents the behavior to be performed during a particular phase.
    /// Different actions are permitted during each phase.  Consult the
    /// documentation of the chains for restrictions.
    /// </para>
    /// <para>
    /// The phases generally run in the following order.  Some phases may be skipped
    /// due to exceptions or if there is no work to be done.
    /// <list type="bullet">
    /// <item><see cref="BeforeTestChain" /></item>
    /// <item><see cref="InitializeTestChain" /></item>
    /// <item>-- for each test instance --</item>
    /// <item><see cref="DecorateTestInstanceChain" /></item>
    /// <item>Run the actions in the decorated <see cref="PatternTestInstanceActions" /></item>
    /// <item>-- end --</item>
    /// <item><see cref="DisposeTestChain" /></item>
    /// <item><see cref="AfterTestChain" /></item>
    /// </list>
    /// </para>
    /// <para>
    /// This class is the backbone of the <see cref="PatternTestFramework" />
    /// extensibility model.  Because new behaviors can be defined and added
    /// to action chains at will by any component that participates in the test
    /// construction process (typically an <see cref="IPattern" />), the framework
    /// itself does not need to hardcode the mechanism by which the behaviors are
    /// selected.
    /// </para>
    /// </remarks>
    public class PatternTestActions
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
        /// Creates a copy of the test actions.
        /// </summary>
        /// <returns>The new copy.</returns>
        public PatternTestActions Copy()
        {
            var copy = new PatternTestActions();
            copy.beforeTestChain.Action = beforeTestChain.Action;
            copy.initializeTestChain.Action = initializeTestChain.Action;
            copy.disposeTestChain.Action = disposeTestChain.Action;
            copy.afterTestChain.Action = afterTestChain.Action;
            copy.decorateTestInstanceChain.Action = decorateTestInstanceChain.Action;
            copy.testInstanceActions = testInstanceActions.Copy();
            return copy;
        }

        /// <summary>
        /// Gets the test instance actions that describes the lifecycle of a test instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// These actions may be further decorated on a per-instance basis using <see cref="DecorateTestInstanceChain" />.
        /// </para>
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
        /// Prepares a newly created test state before its use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of its containing test
        /// step because the test has not yet been started.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Configuring the <see cref="PatternTestState.PrimaryTestStep"/> in anticipation of test execution.</item>
        /// <item>Accessing user data via <see cref="PatternTestState.Data" />.</item>
        /// <item>Skipping the test by throwing an appropriate <see cref="SilentTestException" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public ActionChain<PatternTestState> BeforeTestChain
        {
            get { return beforeTestChain; }
        }

        /// <summary>
        /// Initializes a test prior to the execution of its instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the <see cref="PatternTestState.PrimaryTestStep" />.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Aborting the test run if any preconditions have not been satisfied or if the test is to be skipped.</item>
        /// <item>Configuring the test environment in advance of the enumeration and execution of all test instances.</item>
        /// <item>Accessing user data via <see cref="PatternTestState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public ActionChain<PatternTestState> InitializeTestChain
        {
            get { return initializeTestChain; }
        }

        /// <summary>
        /// Cleans up a test following the execution of its instances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the <see cref="PatternTestState.PrimaryTestStep" />.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Deconfiguring the test environment following the enumeration and execution of all test instances.</item>
        /// <item>Accessing user data via <see cref="PatternTestState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public ActionChain<PatternTestState> DisposeTestChain
        {
            get { return disposeTestChain; }
        }

        /// <summary>
        /// Cleans up a completed test state after its use.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of its containing test
        /// step because the test has terminated.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Accessing user data via <see cref="PatternTestState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public ActionChain<PatternTestState> AfterTestChain
        {
            get { return afterTestChain; }
        }

        /// <summary>
        /// Decorates the <see cref="PatternTestInstanceActions" /> of a test instance before its
        /// <see cref="PatternTestInstanceActions.BeforeTestInstanceChain" /> actions have a chance to run.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of its containing test
        /// instance because the test has not yet been started.
        /// </para>
        /// <para>
        /// This method may apply any number of decorations to the test instance's actions
        /// to the supplied <see cref="PatternTestInstanceActions" /> object.
        /// The test instance's original actions are unmodified by this operation and the
        /// decorated actions are discarded once the child test is finished.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Adding additional actions for the test instance to the <see cref="PatternTestInstanceActions"/>.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </remarks>
        public ActionChain<PatternTestState, PatternTestInstanceActions> DecorateTestInstanceChain
        {
            get { return decorateTestInstanceChain; }
        }
    }
}
