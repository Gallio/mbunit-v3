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
using Gallio.Collections;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// <para>
    /// The context provides information about the environment in which
    /// a test is executing.  A new context is created each time a test or
    /// test step begins execution.
    /// </para>
    /// <para>
    /// Contexts are arranged in a hierarchy that corresponds to the order in which
    /// the contexts were entered.  Thus the context for a test likely has as
    /// its parent the context for its containing test fixture.
    /// </para>
    /// <para>
    /// Arbitrary user data can be associated with a context.  Furthermore, client
    /// code may attach <see cref="CleanUp" /> event handlers to perform resource
    /// reclamation just prior to marking the test step as finished.
    /// </para>
    /// <para>
    /// When the context is disposed, its associated test step is automatically
    /// marked as being finished unless <see cref="FinishStep" /> was previously called.
    /// When this occurs the test step is finished with an outcome of
    /// <see cref="TestOutcome.Error" />.
    /// </para>
    /// </summary>
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
        /// <para>
        /// Gets the log writer for this context.
        /// </para>
        /// <para>
        /// Each test step gets its own log writer that is distinct from those
        /// of other steps.  So the log writer returned by this property is
        /// particular to the step represented by this test context.
        /// </para>
        /// </summary>
        LogWriter LogWriter { get; }

        /// <summary>
        /// Gets or sets the lifecycle phase the context is in.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string LifecyclePhase { get; set; }

        /// <summary>
        /// Gets or sets the step's outcome.  Ths value of this property is initially
        /// <see cref="TestOutcome.Passed" /> but may change over the course of execution
        /// depending on how particular lifecycle phases behave.  The step's outcome value
        /// becomes frozen once the step finishes.
        /// </summary>
        /// <remarks>
        /// For example, this property enables code running in a tear down method to
        /// determine whether the test failed and to perform different actions in that case.
        /// </remarks>
        TestOutcome Outcome { get; set; }

        /// <summary>
        /// Gets the user data collection associated with the context.  It may be used
        /// to associate arbitrary key/value pairs with the context.
        /// </summary>
        UserDataCollection Data { get; }

        /// <summary>
        /// Gets the current assertion count.
        /// </summary>
        int AssertCount { get; }

        /// <summary>
        /// Returns true if the step associated with the context has finished execution
        /// and completed all <see cref="CleanUp" /> actions.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// <para>
        /// The <see cref="CleanUp" /> event is raised just before the test step
        /// finishes execution to perform resource reclamation.
        /// </para>
        /// <para>
        /// Clients may attach handlers to this event to perform cleanup
        /// activities and other tasks as needed.  If a new event handler is
        /// added and the step has already finished, the handler is immediately invoked.
        /// </para>
        /// </summary>
        event EventHandler CleanUp;

        /// <summary>
        /// Adds the specified amount to the assert count atomically.
        /// </summary>
        /// <param name="value">The amount to add to the assert count</param>
        void AddAssertCount(int value);

        /// <summary>
        /// Adds metadata to the step that is running in the context.
        /// </summary>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null</exception>
        void AddMetadata(string metadataKey, string metadataValue);

        /// <summary>
        /// Starts a child step of the test and returns its context.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is set to the context of the
        /// test step that is starting.
        /// </remarks>
        /// <param name="childStep">The step to start</param>
        /// <returns>The context of the child step</returns>
        /// <exception cref="InvalidOperationException">Thrown if the step has finished</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="childStep"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="childStep"/> is not a child
        /// of the this step</exception>
        ITestContext StartChildStep(ITestStep childStep);

        /// <summary>
        /// <para>
        /// Starts a child step of a test and returns its context.
        /// </para>
        /// <para>
        /// This method is equivalent to calling <see cref="StartChildStep(ITestStep)" />
        /// using a default implementation of <see cref="ITestStep" />
        /// that is initialized with <paramref name="name"/> and <paramref name="codeElement"/>.
        /// </para>
        /// </summary>
        /// <param name="name">The name of the step</param>
        /// <param name="codeElement">The code element, or null if none</param>
        /// <returns>The context of the child step</returns>
        /// <seealso cref="StartChildStep(ITestStep)"/>
        /// <exception cref="InvalidOperationException">Thrown if the step has finished</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        ITestContext StartChildStep(string name, ICodeElementInfo codeElement);

        /// <summary>
        /// Finishes a step and submits its final result.
        /// </summary>
        /// <remarks>
        /// If any children of the step are still executing their contexts are automatically
        /// disposed.  Then <see cref="CleanUp"/> actions are executed.  Finally, the current
        /// thread's test context is exited.
        /// </remarks>
        /// <param name="outcome">The final test outcome</param>
        /// <param name="actualDuration">The actual duration of the step, if null the step monitor
        /// will record the duration as the total amount of time since the step monitor was started</param>
        /// <seealso cref="CleanUp"/>
        void FinishStep(TestOutcome outcome, TimeSpan? actualDuration);
    }
}