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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A context handler offers services for test monitors to interact
    /// with the test context.  A context handler should track its contexts
    /// so as to ensure that they 
    /// </summary>
    public interface IContextHandler
    {
        /// <summary>
        /// Creates a context for a test step.
        /// </summary>
        /// <param name="step">The test step</param>
        /// <returns>The step's context</returns>
        Context CreateContext(ITestStep step);

        /// <summary>
        /// Enters the specified context to start running the step.
        /// </summary>
        /// <param name="context">The step's context</param>
        void StartStep(Context context);

        /// <summary>
        /// Exits the specified context and records the final outcome of the step.
        /// </summary>
        /// <param name="context">The step's context</param>
        /// <param name="status">The status</param>
        /// <param name="outcome">The outcome</param>
        /// <param name="actualDuration">The actual duration of execution, or null to use the
        /// time since <see cref="StartStep" /> was called</param>
        void FinishStep(Context context, TestStatus status, TestOutcome outcome, TimeSpan? actualDuration);

        /// <summary>
        /// Dynamically adds metadata to the context for a step.
        /// </summary>
        /// <param name="context">The step's context</param>
        /// <param name="key">The metadata key</param>
        /// <param name="value">The metadata value</param>
        void AddMetadata(Context context, string key, string value);

        /// <summary>
        /// Sets the current lifecycle phase for a step.
        /// </summary>
        /// <param name="context">The step's context</param>
        /// <param name="phase">The lifecycle phase</param>
        void SetLifecyclePhase(Context context, string phase);

        /// <summary>
        /// Sets the interim outcome of the step.
        /// </summary>
        /// <param name="context">The step's context</param>
        /// <param name="outcome">The interim outcome</param>
        void SetInterimOutcome(Context context, TestOutcome outcome);
    }
}
