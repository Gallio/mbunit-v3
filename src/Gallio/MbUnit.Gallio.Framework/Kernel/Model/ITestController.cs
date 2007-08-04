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
using MbUnit.Framework.Kernel.Events;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// A test controller runs a batch of tests.
    /// </summary>
    public interface ITestController : IDisposable
    {
        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <remarks>
        /// This method can be called at most once during the lifetime of a test controller.
        /// </remarks>
        /// <param name="progressMonitor">The progress monitor</param>
        /// <param name="options">The test execution options</param>
        /// <param name="listener">The test listener</param>
        /// <param name="tests">The tests to run (these have been pre-filtered by the test harness)</param>
        void Run(IProgressMonitor progressMonitor, TestExecutionOptions options, IEventListener listener, IList<ITest> tests);
    }
}
