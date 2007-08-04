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

namespace MbUnit.Core.Harness
{
    /// <summary>
    /// <para>
    /// A test harness contributor extends the <see cref="ITestHarness" /> by
    /// attaching event handlers to modify its lifecycle.  A test harness contributor
    /// is generally a singleton component whereas a new test harness is created
    /// for each test project that is loaded.
    /// </para>
    /// <para>
    /// A new third party test framework may be supported by registering
    /// a suitable implementation of this interface that extends the test harness
    /// with code to enumerate and execute that framework's tests.
    /// </para>
    /// </summary>
    public interface ITestHarnessContributor
    {
        /// <summary>
        /// Applies the contributions of this test harness contributor to the
        /// specified test harness.
        /// </summary>
        /// <param name="harness">The test harness</param>
        void Apply(ITestHarness harness);
    }
}