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
using System.Collections.Generic;
using System.Text;
using MbUnit.Core.RuntimeSupport;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// A step monitor tracks the execution of a single <see cref="IStep" /> of a <see cref="ITest" />.
    /// </summary>
    public interface IStepMonitor : ICoreContextServiceProvider
    {
        /// <summary>
        /// Gets the context associated with the step.
        /// </summary>
        Context Context { get; }

        /// <summary>
        /// Starts a child step of the test and returns its step monitor.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is set to a new context for the
        /// test step that is starting.
        /// </remarks>
        /// <param name="name">The name of the step</param>
        /// <param name="codeReference">The code reference of the step</param>
        /// <returns>The monitor for the child step</returns>
        /// <exception cref="InvalidOperationException">Thrown if the step has finished</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> or <paramref name="codeReference"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is the empty string</exception>
        IStepMonitor StartChildStep(string name, CodeReference codeReference);

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
    }
}
