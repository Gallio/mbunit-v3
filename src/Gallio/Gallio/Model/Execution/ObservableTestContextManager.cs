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

namespace Gallio.Model.Execution
{
    /// <summary>
    /// An observable test context manager creates and tracks test contexts that are
    /// associated with a <see cref="ITestListener" /> for reporting test events
    /// back to the test runner.
    /// </summary>
    public class ObservableTestContextManager : ITestContextManager
    {
        private readonly ITestContextTracker contextTracker;
        private readonly ITestListener listener;

        /// <summary>
        /// Creates a test context manager.
        /// </summary>
        /// <param name="contextTracker">The test context tracker</param>
        /// <param name="listener">The test listener to which test events are dispatched</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/>
        /// or <paramref name="listener"/> is null</exception>
        public ObservableTestContextManager(ITestContextTracker contextTracker,
            ITestListener listener)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");
            if (listener == null)
                throw new ArgumentNullException("listener");

            this.contextTracker = contextTracker;
            this.listener = listener;
        }

        /// <inheritdoc />
        public ITestContextTracker ContextTracker
        {
            get { return contextTracker; }
        }

        /// <summary>
        /// Gets the test listener to which test events are dispatched.
        /// </summary>
        public ITestListener Listener
        {
            get { return listener; }
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
