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

using System;

namespace Gallio.Utilities
{
    /// <summary>
    /// A structure that memoizes the result of some computation for later reuse.
    /// </summary>
    /// <remarks>
    /// Not thread safe.
    /// </remarks>
    /// <typeparam name="T">The value type</typeparam>
    public struct Memoizer<T>
    {
        private T value;
        private bool isPopulated;

        /// <summary>
        /// Gets the memoized value if available, otherwise populates it
        /// using the specified populator function and stores it for later reuse.
        /// </summary>
        /// <param name="populator">The populator</param>
        /// <returns>The value returned by the populator, possibly memoized</returns>
        public T Memoize(Func<T> populator)
        {
            if (!isPopulated)
            {
                value = populator();
                isPopulated = true;
            }

            return value;
        }
    }
}
