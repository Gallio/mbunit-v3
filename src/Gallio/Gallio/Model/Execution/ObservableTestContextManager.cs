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
using Gallio.Model.Messages;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An observable test context manager creates and tracks test contexts that are
    /// associated with a <see cref="ITestExecutionListener" /> for reporting test events
    /// back to the test runner.
    /// </summary>
    public class ObservableTestContextManager : ITestContextManager
    {
        private readonly ITestContextTracker contextTracker;
        private readonly ITestExecutionListener testExecutionListener;

        /// <summary>
        /// Creates a test context manager.
        /// </summary>
        /// <param name="contextTracker">The test context tracker</param>
        /// <param name="testExecutionListener">The test listener to which notifications are dispatched</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/>
        /// or <paramref name="testExecutionListener"/> is null</exception>
        public ObservableTestContextManager(ITestContextTracker contextTracker,
            ITestExecutionListener testExecutionListener)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");
            if (testExecutionListener == null)
                throw new ArgumentNullException("testExecutionListener");

            this.contextTracker = contextTracker;
            this.testExecutionListener = testExecutionListener;
        }

        /// <inheritdoc />
        public ITestContextTracker ContextTracker
        {
            get { return contextTracker; }
        }

        /// <summary>
        /// Gets the test listener to which test events are dispatched.
        /// </summary>
        public ITestExecutionListener TestExecutionListener
        {
            get { return testExecutionListener; }
        }

        /// <inheritdoc />
        public ITestContext StartStep(ITestStep testStep)
        {
            if (testStep == null)
                throw new ArgumentNullException("testStep");

            ITestContext parentContext = contextTracker.CurrentContext;
            ObservableTestContext context = new ObservableTestContext(this, testStep, parentContext);
            context.InitializeAndStartStep();
            return context;
        }
    }
}
