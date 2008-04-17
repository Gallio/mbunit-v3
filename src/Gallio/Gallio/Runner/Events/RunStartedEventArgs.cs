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
using Gallio.Model.Execution;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test execution has started.
    /// </summary>
    public sealed class RunStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestExecutionOptions testExecutionOptions;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testExecutionOptions">The test execution options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testExecutionOptions"/> is null</exception>
        public RunStartedEventArgs(TestExecutionOptions testExecutionOptions)
        {
            if (testExecutionOptions == null)
                throw new ArgumentNullException("testExecutionOptions");

            this.testExecutionOptions = testExecutionOptions;
        }

        /// <summary>
        /// Gets the test execution options.
        /// </summary>
        public TestExecutionOptions TestExecutionOptions
        {
            get { return testExecutionOptions; }
        }
    }
}
