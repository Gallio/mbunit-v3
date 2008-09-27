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

namespace Gallio.Runner.Sessions
{
    /// <summary>
    /// Event arguments describing an event affecting a test run.
    /// </summary>
    public class TestRunEventArgs : EventArgs
    {
        /// <summary>
        /// Creates event arguments for a test run event.
        /// </summary>
        /// <param name="run">The test run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="run"/> is null</exception>
        public TestRunEventArgs(ITestRun run)
        {
            if (run == null)
                throw new ArgumentNullException("run");

            Run = run;
        }

        /// <summary>
        /// Gets the test run.
        /// </summary>
        public ITestRun Run { get; private set; }
    }
}
