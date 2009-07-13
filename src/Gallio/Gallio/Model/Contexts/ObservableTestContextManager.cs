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
using Gallio.Model.Tree;
using Gallio.Common.Messaging;

namespace Gallio.Model.Contexts
{
    /// <summary>
    /// An observable test context manager creates and tracks test contexts and
    /// publishes test messages back to a <see cref="IMessageSink"/>.
    /// </summary>
    public class ObservableTestContextManager : ITestContextManager
    {
        private readonly ITestContextTracker contextTracker;
        private readonly IMessageSink messageSink;

        /// <summary>
        /// Creates a test context manager.
        /// </summary>
        /// <param name="contextTracker">The test context tracker.</param>
        /// <param name="messageSink">The message sink to which test message are published.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contextTracker"/>
        /// or <paramref name="messageSink"/> is null.</exception>
        public ObservableTestContextManager(ITestContextTracker contextTracker,
            IMessageSink messageSink)
        {
            if (contextTracker == null)
                throw new ArgumentNullException("contextTracker");
            if (messageSink == null)
                throw new ArgumentNullException("messageSink");

            this.contextTracker = contextTracker;
            this.messageSink = messageSink;
        }

        /// <inheritdoc />
        public ITestContextTracker ContextTracker
        {
            get { return contextTracker; }
        }

        /// <summary>
        /// Gets the message sink to which test messages are published.
        /// </summary>
        public IMessageSink MessageSink
        {
            get { return messageSink; }
        }

        /// <inheritdoc />
        public ITestContext StartStep(TestStep testStep)
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