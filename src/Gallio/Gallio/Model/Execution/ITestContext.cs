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
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Common.Markup;
using Gallio.Common.Reflection;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// The context provides information about the environment in which
    /// a test is executing.  A new context is created each time a test or
    /// test step begins execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexts are arranged in a hierarchy that corresponds to the order in which
    /// the contexts were entered.  Thus the context for a test likely has as
    /// its parent the context for its containing test fixture.
    /// </para>
    /// <para>
    /// Arbitrary user data can be associated with a context.  Furthermore, client
    /// code may attach <see cref="Finishing" /> event handlers to perform resource
    /// reclamation or other updates when the test step is finished.
    /// </para>
    /// <para>
    /// When the context is disposed, its associated test step is automatically
    /// marked as being finished unless <see cref="FinishStep" /> was previously called.
    /// When this occurs the test step is finished with an outcome of
    /// <see cref="TestOutcome.Error" />.
    /// </para>
    /// </remarks>
    public interface ITestContext : IDisposable
    {
        /// <summary>
        /// Gets the parent context or null if this context has no parent.
        /// </summary>
        ITestContext Parent { get; }

        /// <summary>
        /// Gets the test step associated with the context.
        /// </summary>
        ITestStep TestStep { get; }

        /// <summary>
        /// Gets the log writer for the test executing in this context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each test step gets its own log writer that is distinct from those
        /// of other steps.  So the log writer returned by this property is
        /// particular to the step represented by this test context.
        /// </para>
        /// </remarks>
        MarkupDocumentWriter LogWriter { get; }

        /// <summary>
        /// Gets or sets the lifecycle phase the context is in.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the phase while the test is not running.</exception>
        /// <seealso cref="LifecyclePhases"/>
        string LifecyclePhase
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the step's outcome or its interim outcome if the test is still running.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The value of this property is initially <see cref="TestOutcome.Passed" /> but may change
        /// over the course of execution to reflect the anticipated outcome of the test.  When
        /// the test finishes, its outcome is frozen.
        /// </para>
        /// <para>
        /// For example, this property enables code running as part of the tear down phase to
        /// determine whether the test is failing and to perform different actions in that case.
        /// </para>
        /// </remarks>
        /// <seealso cref="SetInterimOutcome"/>
        TestOutcome Outcome { get; }

        /// <summary>
        /// Gets the user data collection associated with the context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It may be used to associate arbitrary key/value pairs with the context.
        /// </para>
        /// <para>
        /// Each context has its own distinct user data collection.
        /// </para>
        /// </remarks>
        UserDataCollection Data { get; }

        /// <summary>
        /// Gets the current assertion count.
        /// </summary>
        int AssertCount { get; }

        /// <summary>
        /// Returns true if the step associated with the context has finished execution
        /// and completed all <see cref="Finishing" /> actions.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// The <see cref="Finishing" /> event is raised when the test step
        /// is finishing to perform resource reclamation or other updates.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Clients may attach handlers to this event to perform cleanup
        /// activities and other tasks as needed.  If a new event handler is
        /// added and the step has already finished, the handler is immediately invoked.
        /// </para>
        /// </remarks>
        event EventHandler Finishing;

        /// <summary>
        /// Adds the specified amount to the assert count atomically.
        /// </summary>
        /// <param name="value">The amount to add to the assert count.</param>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running.</exception>
        void AddAssertCount(int value);

        /// <summary>
        /// Adds metadata to the step that is running in the context.
        /// </summary>
        /// <param name="metadataKey">The metadata key.</param>
        /// <param name="metadataValue">The metadata value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the test is not running.</exception>
        void AddMetadata(string metadataKey, string metadataValue);

        /// <summary>
        /// Sets the step's interim <see cref="Outcome" />.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The interim outcome is used to communicate the anticipated outcome of the step to 
        /// later phases of execution.
        /// </para>
        /// <para>
        /// The value set here will be overridden by whatever final outcome the step
        /// returns.  Consequently the actual outcome may still differ from the anticipated 
        /// outcome that was set using this method.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if attempting to set the outcome while the test is not running.</exception>
        /// <seealso cref="Outcome"/>
        void SetInterimOutcome(TestOutcome outcome);

        /// <summary>
        /// Starts a child step of the test and returns its context.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The current thread's test context is set to the context of the
        /// test step that is starting.
        /// </para>
        /// </remarks>
        /// <param name="childStep">The step to start.</param>
        /// <returns>The context of the child step.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the step has finished.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="childStep"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="childStep"/> is not a child
        /// of the this step.</exception>
        ITestContext StartChildStep(ITestStep childStep);

        /// <summary>
        /// Finishes a step and submits its final result.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If any children of the step are still executing their contexts are automatically
        /// disposed.  Then <see cref="Finishing"/> actions are executed.  Finally, the current
        /// thread's test context is exited.
        /// </para>
        /// </remarks>
        /// <param name="outcome">The final test outcome.</param>
        /// <param name="actualDuration">The actual duration of the step, if null the step monitor
        /// will record the duration as the total amount of time since the step monitor was started.</param>
        /// <seealso cref="Finishing"/>
        void FinishStep(TestOutcome outcome, TimeSpan? actualDuration);
    }
}