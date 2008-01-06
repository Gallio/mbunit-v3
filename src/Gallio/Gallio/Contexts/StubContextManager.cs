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

namespace Gallio.Contexts
{
    /// <summary>
    /// <para>
    /// The stub context manager is a dummy implementation of <see cref="IContextManager" />
    /// that is used in situations where the runtime environment has not been initialized.
    /// </para>
    /// <para>
    /// A typical use of this stub is to support the use of certain framework methods from
    /// clients that are not being executed inside of the Gallio test harness such as when
    /// test code is run by third party tools.
    /// </para>
    /// </summary>
    public sealed class StubContextManager : DefaultContextManager
    {
        /// <summary>
        /// Creates a stub context manager.
        /// </summary>
        public StubContextManager()
        {
            GlobalContext = new StubContext();
        }
    }
}
