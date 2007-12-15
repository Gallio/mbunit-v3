// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Contexts;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Model.Reflection;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A step monitor tracks the execution of a <see cref="ITestStep" />.
    /// </summary>
    public interface ITestStepMonitor
    {
        /// <summary>
        /// Gets the step.
        /// </summary>
        ITestStep Step { get; }

        /// <summary>
        /// Gets the step's context.
        /// </summary>
        Context Context { get; }

        /// <summary>
        /// Gets the step's log writer.
        /// </summary>
        LogWriter LogWriter { get; }

        /// <summary>
        /// Gets or sets the step's lifecycle phase.
        /// </summary>
        /// <seealso cref="LifecyclePhases"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string LifecyclePhase { get; set; }

        /// <summary>
        /// Gets the step's outcome.  Ths value of this property is initially
        /// <see cref="TestOutcome.Passed" /> but may change over the course of execution
        /// depending on how particular lifecycle phases behave.  The step's outcome value
        /// becomes frozen once the step finishes.
        /// </summary>
        /// <remarks>
        /// For example, this property enables code running in a tear down method to
        /// determine whether the test failed and to perform different actions in that case.
        /// </remarks>
        TestOutcome Outcome { get; }

        /// <summary>
        /// Starts a child step of the test and returns its step monitor.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is set to a new context for the
        /// test step that is starting.
        /// </remarks>
        /// <param name="childStep">The step to start</param>
        /// <returns>The monitor for the child step</returns>
        /// <exception cref="InvalidOperationException">Thrown if the step has finished</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="childStep"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="childStep"/> is not a child
        /// of the this step</exception>
        ITestStepMonitor StartChildStep(ITestStep childStep);

        /// <summary>
        /// 
        /// Starts a child step of a test and returns its step monitor
        /// using a default implementation of <see cref="ITestStep" />.
        /// </summary>
        /// <param name="name">The name of the step</param>
        /// <param name="codeElement">The code element, or null if none</param>
        /// <returns>The monitor for the child step</returns>
        /// <seealso cref="StartChildStep(ITestStep)"/>
        /// <exception cref="InvalidOperationException">Thrown if the step has finished</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        ITestStepMonitor StartChildStep(string name, ICodeElementInfo codeElement);

        /// <summary>
        /// Finishes a step and submits its final result.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is exited and disposed for the step
        /// that is finishing.
        /// </remarks>
        /// <param name="status">The final test status</param>
        /// <param name="outcome">The final test outcome</param>
        /// <param name="actualDuration">The actual duration of the step, if null the step monitor
        /// will record the duration as the total amount of time since the step monitor was started</param>
        void FinishStep(TestStatus status, TestOutcome outcome, TimeSpan? actualDuration);

        /// <summary>
        /// Sets the interim value of the step's outcome.  The interim outcome
        /// is exposed to the client by the <see cref="Gallio.Contexts.Context.Outcome" />
        /// property which can use it to perform different actions based on whether
        /// the test is known to be failing, for example.  The initial value of
        /// the outcome is <see cref="TestOutcome.Passed" />.
        /// </summary>
        /// <remarks>
        /// When <see cref="FinishStep" /> is called, the value of
        /// the <see cref="Outcome" /> will be overridden
        /// and frozen.  Consequently there is no need to call this method
        /// to set the final outcome of a step.
        /// </remarks>
        /// <param name="outcome">The new interim outcome for the step</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="FinishStep" />
        /// has already been called.</exception>
        void SetInterimOutcome(TestOutcome outcome);

        /// <summary>
        /// Adds metadata to the step.
        /// </summary>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null</exception>
        void AddMetadata(string metadataKey, string metadataValue);
    }
}
