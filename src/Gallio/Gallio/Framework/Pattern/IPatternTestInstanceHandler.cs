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

using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A pattern test instance handler provides the logic that implements the various
    /// phases of the test instance execution lifecycle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each method represents the behavior to be performed during a particular phase.
    /// Different actions are permitted during each phase.  Consult the
    /// documentation the appropriate method of this interface for restrictions.
    /// </para>
    /// <para>
    /// The phases generally run in the following order.  Some phases may be skipped
    /// due to exceptions or if there is no work to be done.
    /// <list type="bullet">
    /// <item><see cref="BeforeTestInstance" /></item>
    /// <item>--- begin <see cref="RunTestInstanceBody" /> ---</item>
    /// <item><see cref="InitializeTestInstance" /></item>
    /// <item><see cref="SetUpTestInstance" /></item>
    /// <item><see cref="ExecuteTestInstance" /></item>
    /// <item><see cref="DecorateChildTest" /> before each child test</item>
    /// <item><see cref="TearDownTestInstance" /></item>
    /// <item><see cref="DisposeTestInstance" /></item>
    /// <item>--- end <see cref="RunTestInstanceBody" /> ---</item>
    /// <item><see cref="AfterTestInstance" /></item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IPatternTestInstanceHandler
    {
        /// <summary>
        /// <para>
        /// Prepares a newly created test instance state before its use.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the <see cref="PatternTestState.PrimaryTestStep" />
        /// because the test step for this instance (if different from the primary step) has not yet started.
        /// </para>
        /// <para>
        /// If <see cref="PatternTestInstanceState.IsReusingPrimaryTestStep" /> is false
        /// then this method has the opportunity to modify the name or add metadata to the
        /// brand new <see cref="PatternTestStep" /> that was created for just this test instance.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Adding or changing slot values.</item>
        /// <item>Configuring the test environment in advance of test initialization.</item>
        /// <item>Modifying the name or metadata of the <see cref="PatternTestStep" />, if 
        /// <see cref="PatternTestInstanceState.IsReusingPrimaryTestStep" /> is false
        /// (since the primary test step has already started execution).</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way UNLESS <see cref="PatternTestInstanceState.IsReusingPrimaryTestStep" />
        /// is false.</item>
        /// <item>Skipping the test instance by throwing an appropriate <see cref="SilentTestException" />.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void BeforeTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Initializes a test instance that has just started running.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the test instance
        /// in the <see cref="LifecyclePhases.Initialize" /> lifecycle phase.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Creating the test fixture instance and setting <see cref="PatternTestInstanceState.FixtureType"/>
        /// and <see cref="PatternTestInstanceState.FixtureInstance"/>.</item>
        /// <item>Configuring the test fixture in advance of test execution.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void InitializeTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Sets up a test instance prior to execution.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the test instance
        /// in the <see cref="LifecyclePhases.SetUp" /> lifecycle phase.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Invoking test setup methods.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void SetUpTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Executes the test instance.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the test instance
        /// in the <see cref="LifecyclePhases.Execute" /> lifecycle phase.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Invoking test methods.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void ExecuteTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Tears down a test instance following execution.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the test instance
        /// in the <see cref="LifecyclePhases.TearDown" /> lifecycle phase.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Invoking test teardown methods.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void TearDownTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Disposes a test instance that is about to terminate.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the test instance
        /// in the <see cref="LifecyclePhases.Dispose" /> lifecycle phase.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Deconfiguring the test fixture following test execution.</item>
        /// <item>Disposing the test fixture instance.</item>
        /// <item>Disposing other resources.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void DisposeTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Cleans up a completed test instance after its use.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the <see cref="PatternTestState.PrimaryTestStep" />
        /// because the test step for this instance (if different from the primary step) has terminated.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Deconfiguring the test environment following the test disposal.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testInstanceState">The test instance state, never null</param>
        void AfterTestInstance(PatternTestInstanceState testInstanceState);

        /// <summary>
        /// <para>
        /// Decorates the <see cref="IPatternTestInstanceHandler" /> of a child test before its
        /// <see cref="IPatternTestHandler.BeforeTest" /> actions have a chance to run.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="TestContext" /> of the test instance
        /// in the <see cref="LifecyclePhases.Execute" /> lifecycle phase.
        /// </para>
        /// <para>
        /// This method may apply any number of decorations to the child test's handler
        /// by adding actions to the supplied <paramref name="decoratedChildTestActions" /> object.
        /// The child test's original handler is unmodified by this operation and the
        /// decorated actions are discarded once the child test is finished.
        /// </para>
        /// <para>
        /// A typical use of this method is to augment the <see cref="SetUpTestInstance" />
        /// and <see cref="TearDownTestInstance" /> behaviors of the child test with
        /// additional contributions provided by the parent.
        /// </para>
        /// <para>
        /// It is also possible to decorate descendants besides direct children.
        /// To do so, decorate the child's <see cref="DecorateChildTest" /> behavior
        /// to perpetuate the decoration down to more deeply nested descendants.  This
        /// process of recursive decoration may be carried along to whatever depth is required.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Adding additional actions for the child test to the <paramref name="decoratedChildTestActions"/>.</item>
        /// <item>Accessing user data via <see cref="PatternTestInstanceState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestState" /> object in any way.</item>
        /// <item>Modifying the <see cref="PatternTestStep" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <paramref name="testInstanceState"/> represents the state of the currently
        /// executing instance of this test; not the child.  The child has not started
        /// running yet.  When the child runs, the decorator actions installed in
        /// <paramref name="decoratedChildTestActions"/> will be invoked with references to the
        /// child's state as usual.
        /// </para>
        /// <para>
        /// For some purposes it may be useful to save the <paramref name="testInstanceState"/>
        /// for later use in the decorated action.  For example, if the decorated action
        /// needs to invoke a method on the parent test fixture, then it will need to
        /// have the parent's <paramref name="testInstanceState"/>.  This is very easy
        /// using anonymous delegates (due to variable capture) but can also be accomplished
        /// with other means as required.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, never null</param>
        /// <param name="decoratedChildTestActions">The child test's actions to decorate, never null</param>
        void DecorateChildTest(PatternTestInstanceState testInstanceState, PatternTestActions decoratedChildTestActions);

        /// <summary>
        /// Runs the body of the test from the Initialize phase through the Dispose phase.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is somewhat special in that it gives the test instance handler a chance to
        /// encapsulate the context in which the test runs.  It can cause the test to run repeatedly,
        /// or in another thread, or with some special execution context.  Of course, if it does any
        /// of these things then it is responsible for properly cleaning up the test and responding
        /// in a timely manner to abort events from the current test context's <see cref="Sandbox" />.
        /// </para>
        /// </remarks>
        /// <param name="testInstanceState">The test instance state, never null</param>
        /// <returns>The test outcome</returns>
        TestOutcome RunTestInstanceBody(PatternTestInstanceState testInstanceState);
    }
}