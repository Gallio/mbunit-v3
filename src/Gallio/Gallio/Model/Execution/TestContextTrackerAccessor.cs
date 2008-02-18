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

using Gallio.Hosting;

namespace Gallio.Model.Execution
{
    /// <summary>
    /// Static service locator class for <see cref="ITestContextTracker" />.
    /// Handles the case where no <see cref="ITestContextTracker" /> is registered
    /// with the <see cref="Runtime" /> by returning a <see cref="StubTestContextTracker" />.
    /// </summary>
    public static class TestContextTrackerAccessor
    {
        private static ITestContextTracker cachedContextTracker;

        static TestContextTrackerAccessor()
        {
            Runtime.InstanceChanged += delegate { cachedContextTracker = null; };
        }

        /// <summary>
        /// Gets the context tracker instance.
        /// </summary>
        public static ITestContextTracker GetInstance()
        {
            if (cachedContextTracker == null)
            {
                if (Runtime.IsInitialized)
                    cachedContextTracker = Runtime.Instance.Resolve<ITestContextTracker>();
                else
                    cachedContextTracker = new StubTestContextTracker();
            }

            return cachedContextTracker;
        }
    }
}
