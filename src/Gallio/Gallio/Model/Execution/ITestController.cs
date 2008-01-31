// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Hosting.ProgressMonitoring;

namespace Gallio.Model.Execution
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
        /// <param name="rootTestMonitor">The root test monitor</param>
        /// <param name="parentTestInstance">The parent test instance or null when
        /// running the tests should produce a root test</param>
        void RunTests(IProgressMonitor progressMonitor, ITestMonitor rootTestMonitor,
            ITestInstance parentTestInstance);
    }
}