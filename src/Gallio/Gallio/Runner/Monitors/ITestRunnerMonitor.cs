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
using System.Collections.Generic;
using System.Text;
using Gallio.Runner;

namespace Gallio.Runner.Monitors
{
    /// <summary>
    /// A monitor listens to events on the <see cref="ITestRunner" /> to
    /// which it is attached and provides some service.
    /// </summary>
    public interface ITestRunnerMonitor
    {
        /// <summary>
        /// Attaches the monitor to a test runner.
        /// </summary>
        /// <param name="runner">The test runner</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runner"/> is null</exception>
        void Attach(ITestRunner runner);

        /// <summary>
        /// Detaches the monitor.
        /// </summary>
        void Detach();
    }
}
