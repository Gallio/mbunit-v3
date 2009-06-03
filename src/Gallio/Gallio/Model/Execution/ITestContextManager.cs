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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// A test context manager functions as a factory for <see cref="ITestContext" />
    /// objects and tracks them with an <see cref="ITestContextTracker" />.
    /// </summary>
    public interface ITestContextManager
    {
        /// <summary>
        /// Gets the test context tracker.
        /// </summary>
        ITestContextTracker ContextTracker { get; }

        /// <summary>
        /// Starts a test step and returns its associated test context.
        /// </summary>
        /// <remarks>
        /// The current thread's test context is set to a new context for the
        /// test step that is starting.  The new context will be a child of the
        /// current thread's context.
        /// </remarks>
        /// <param name="testStep">The test step.</param>
        /// <returns>The test context associated with the test step</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testStep"/> is null.</exception>
        ITestContext StartStep(ITestStep testStep);
    }
}
