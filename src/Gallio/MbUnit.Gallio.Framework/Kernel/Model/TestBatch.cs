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
using MbUnit.Framework.Kernel.Harness;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A test batch groups tests into work units that are executed by a single <see cref="ITestController" />.
    /// Test batches cannot be nested.  Consequently, a test in the test tree is either not associated
    /// with a test batch, or the test and all of its descendents are associated with
    /// the same test batch.
    /// </summary>
    /// <todo author="jeff">
    /// In principle, some test batches actually could be nested but difficulties
    /// arise when integrating 3rd party test frameworks.  If we can capture the moment
    /// when the nested test batch is being entered then we could in fact
    /// suspend the execution of the current test controller and launch a test controller
    /// for the nested test batch.  This is not always possible and does not seem all
    /// that useful.
    /// </todo>
    public class TestBatch
    {
        private readonly string description;
        private readonly TestControllerFactory controllerFactory;

        /// <summary>
        /// Creates a test batch.
        /// </summary>
        /// <param name="description">The human-readable description of the batch</param>
        /// <param name="controllerFactory"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="description"/> or
        /// <paramref name="controllerFactory"/> is null</exception>
        public TestBatch(string description, TestControllerFactory controllerFactory)
        {
            if (description == null)
                throw new ArgumentNullException(@"description");
            if (controllerFactory == null)
                throw new ArgumentNullException(@"controllerFactory");

            this.description = description;
            this.controllerFactory = controllerFactory;
        }

        /// <summary>
        /// Gets the human-readable description of the batch.
        /// </summary>
        /// <remarks>
        /// The description may be used in diagnostic output but generally will not be seen by end users.
        /// </remarks>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets the test controller factory.
        /// </summary>
        public TestControllerFactory ControllerFactory
        {
            get { return controllerFactory; }
        }

        /// <summary>
        /// Creates a test controller for this test batch.
        /// </summary>
        /// <returns>The newly created test controller</returns>
        public ITestController CreateController()
        {
            return controllerFactory();
        }
    }
}
