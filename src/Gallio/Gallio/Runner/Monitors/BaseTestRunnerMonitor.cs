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
using Gallio.Runner;

namespace Gallio.Runner.Monitors
{
    /// <summary>
    /// Base implementation of <see cref="ITestRunnerMonitor" />.
    /// </summary>
    public abstract class BaseTestRunnerMonitor : ITestRunnerMonitor
    {
        private ITestRunner runner;

        /// <summary>
        /// Gets the runner to which the monitor has been bound.
        /// </summary>
        public ITestRunner Runner
        {
            get { return runner; }
        }

        /// <inheritdoc />
        public void Attach(ITestRunner runner)
        {
            if (runner == null)
                throw new ArgumentNullException(@"runner");

            if (this.runner != null)
                throw new InvalidOperationException("The monitor is already attached to a different runner.");

            this.runner = runner;
            OnAttach();
        }

        /// <inheritdoc />
        public void Detach()
        {
            if (runner != null)
            {
                OnDetach();
                runner = null;
            }
        }

        /// <summary>
        /// Called when the monitor is attached to allow subclasses to perform any
        /// necessary processing.
        /// </summary>
        protected virtual void OnAttach()
        {
        }

        /// <summary>
        /// Called when the monitor is detached to allow subclasses to perform any
        /// necessary processing.
        /// </summary>
        protected virtual void OnDetach()
        {
        }
    }
}
