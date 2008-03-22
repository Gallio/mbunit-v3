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

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// A pattern test handler provides the logic that implements the various
    /// phases of the test execution lifecycle.  Each method represents the
    /// behavior to be performed during a particular phase.
    /// </para>
    /// <para>
    /// Different actions are permitted during each phase.  Consult the
    /// documentation the appropriate method if this interface for restrictions.
    /// </para>
    /// <para>
    /// The phases generally run in the following order.  Some phases may be skipped
    /// due to exceptions or if there is no work to be done.
    /// <list type="bullet">
    /// <item><see cref="BeforeTest" /></item>
    /// <item><see cref="InitializeTest" /></item>
    /// <item>-- for each test instance --</item>
    /// <item><see cref="DecorateTestInstance" /></item>
    /// <item>Run the actions in the decorated <see cref="IPatternTestInstanceHandler" /></item>
    /// <item>-- end --</item>
    /// <item><see cref="DisposeTest" /></item>
    /// <item><see cref="AfterTest" /></item>
    /// </list>
    /// </para>
    /// </summary>
    public interface IPatternTestHandler
    {
        /// <summary>
        /// Gets the test instance handler that describes the lifecycle of a test instance.
        /// </summary>
        /// <remarks>
        /// These actions may be further decorated on a per-instance basis using
        /// <see cref="DecorateTestInstance" />.
        /// </remarks>
        IPatternTestInstanceHandler TestInstanceHandler { get; }

        /// <summary>
        /// <para>
        /// Prepares a newly created test state before its use.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="Context" /> of its containing test
        /// step because the test has not yet been started.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Adding or changing slot binding accessors via <see cref="PatternTestState.SlotBindingAccessors"/>.</item>
        /// <item>Configuring the <see cref="PatternTestState.PrimaryTestStep"/> in anticipation of test execution.</item>
        /// <item>Accessing user data via <see cref="PatternTestState.Data" />.</item>
        /// </list>
        /// </para>
        /// <para>
        /// The following actions are forbidden during this phase because they would
        /// either go unnoticed or have undesirable side-effects upon test execution:
        /// <list type="bullet">
        /// <item>Modifying the <see cref="PatternTest" /> object in any way.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="testState">The test state, never null</param>
        void BeforeTest(PatternTestState testState);

        /// <summary>
        /// <para>
        /// Initializes a test prior to the execution of its instances.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="Context" /> of the <see cref="PatternTestState.PrimaryTestStep" />.
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
        /// </summary>
        /// <param name="testState">The test state, never null</param>
        void InitializeTest(PatternTestState testState);

        /// <summary>
        /// <para>
        /// Cleans up a test following the execution of its instances.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="Context" /> of the <see cref="PatternTestState.PrimaryTestStep" />.
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
        /// </summary>
        /// <param name="testState">The test state, never null</param>
        void DisposeTest(PatternTestState testState);

        /// <summary>
        /// <para>
        /// Cleans up a completed test state after its use.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="Context" /> of its containing test
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
        /// </summary>
        /// <param name="testState">The test state, never null</param>
        void AfterTest(PatternTestState testState);

        /// <summary>
        /// <para>
        /// Decorates the <see cref="IPatternTestHandler" /> of a test instance before its
        /// <see cref="IPatternTestInstanceHandler.BeforeTestInstance" /> actions have a chance to run.
        /// </para>
        /// <para>
        /// This method runs in the <see cref="Context" /> of its containing test
        /// instance because the test has not yet been started.
        /// </para>
        /// <para>
        /// This method may apply any number of decorations to the test instance's handler
        /// by adding actions to the supplied <paramref name="decoratedTestInstanceActions" /> object.
        /// The test instance's original handler is unmodified by this operation and the
        /// decorated actions are discarded once the child test is finished.
        /// </para>
        /// <para>
        /// The following actions are typically performed during this phase:
        /// <list type="bullet">
        /// <item>Adding additional actions for the test instance to the <paramref name="decoratedTestInstanceActions"/>.</item>
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
        /// </summary>
        /// <param name="testState">The test state, never null</param>
        /// <param name="decoratedTestInstanceActions">The test instance's actions to decorate, never null</param>
        void DecorateTestInstance(PatternTestState testState, PatternTestInstanceActions decoratedTestInstanceActions);
    }
}
