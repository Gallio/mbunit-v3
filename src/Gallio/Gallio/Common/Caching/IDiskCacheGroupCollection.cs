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

namespace Gallio.Common.Caching
{
    /// <summary>
    /// Represents a collection of groups in a disk cache indexed by an aritrary key string.
    /// </summary>
    public interface IDiskCacheGroupCollection
    {
        /// <summary>
        /// Gets the group with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The group.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        IDiskCacheGroup this[string key] { get; }
    }
}
