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

using Gallio.Runtime;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// <para>
    /// Static service locator for <see cref="ITestContextTracker" />.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// Handles the case where the runtime is not initialized by returning a
    /// <see cref="StubTestContextTracker" />.
    /// </para>
    /// </remarks>
    public static class TestContextTrackerAccessor
    {
        private static ITestContextTracker cachedContextTracker;

        static TestContextTrackerAccessor()
        {
            RuntimeAccessor.InstanceChanged += delegate { cachedContextTracker = null; };
        }

        /// <summary>
        /// Gets the global test context tracker singleton.
        /// </summary>
        public static ITestContextTracker Instance
        {
            get
            {
                if (cachedContextTracker == null)
                {
                    if (RuntimeAccessor.IsInitialized)
                        cachedContextTracker = RuntimeAccessor.Instance.Resolve<ITestContextTracker>();
                    else
                        cachedContextTracker = new StubTestContextTracker();
                }

                return cachedContextTracker;
            }
        }
    }
}
