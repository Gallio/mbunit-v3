// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Runner.Events;
using Gallio.Runtime.Logging;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Abstract implementation of a test runner extension.
    /// <seealso cref="ITestRunnerExtension"/> for more details.
    /// </summary>
    public abstract class TestRunnerExtension : ITestRunnerExtension
    {
        private ITestRunnerEvents events;
        private ILogger logger;
        private string parameters = string.Empty;

        /// <summary>
        /// Gets the test runner event extension point.
        /// </summary>
        public ITestRunnerEvents Events
        {
            get { return events; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <inheritdoc />
        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <inheritdoc />
        public void Install(ITestRunnerEvents events, ILogger logger)
        {
            if (events == null)
                throw new ArgumentNullException("events");
            if (logger == null)
                throw new ArgumentNullException("logger");

            this.events = events;
            this.logger = logger;

            Initialize();
        }

        /// <summary>
        /// Initializes the extension as part of extension installation.
        /// </summary>
        protected abstract void Initialize();
    }
}
