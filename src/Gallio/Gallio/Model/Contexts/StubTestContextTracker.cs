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

namespace Gallio.Model.Contexts
{
    /// <summary>
    /// The stub context tracker is a dummy implementation of <see cref="ITestContextTracker" />
    /// that is used in situations where the runtime environment has not been initialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A typical use of this stub is to support the use of certain framework methods from
    /// clients that are not being executed inside of the Gallio test harness such as when
    /// test code is run by third party tools.
    /// </para>
    /// </remarks>
    public sealed class StubTestContextTracker : DefaultTestContextTracker
    {
        /// <summary>
        /// Creates a stub context tracker.
        /// </summary>
        public StubTestContextTracker()
        {
            GlobalContext = new StubTestContext();
        }
    }
}