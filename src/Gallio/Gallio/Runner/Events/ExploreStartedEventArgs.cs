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
using Gallio.Model;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that test exploration has started.
    /// </summary>
    public sealed class ExploreStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestExplorationOptions testExplorationOptions;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testExplorationOptions">The test exploration options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testExplorationOptions"/> is null</exception>
        public ExploreStartedEventArgs(TestExplorationOptions testExplorationOptions)
        {
            if (testExplorationOptions == null)
                throw new ArgumentNullException("testExplorationOptions");

            this.testExplorationOptions = testExplorationOptions;
        }

        /// <summary>
        /// Gets the test exploration options.
        /// </summary>
        public TestExplorationOptions TestExplorationOptions
        {
            get { return testExplorationOptions; }
        }
    }
}
