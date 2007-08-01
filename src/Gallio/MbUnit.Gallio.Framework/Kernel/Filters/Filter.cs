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

namespace MbUnit.Framework.Kernel.Filters
{
    /// <summary>
    /// A filter is a serializable predicate.  The framework uses filters to select
    /// among templates and tests discovered through the process of test enumeration.
    /// </summary>
    /// <remarks>
    /// Filters must be serializable objects defined in the framework assembly
    /// or in plugins.  The reason is that filter definitions may need to be
    /// transferred across AppDomains so the filter's concrete type must be
    /// known to the recipient.  Filters do not implement <see cref="MarshalByRefObject" />
    /// because there is no guarantee that a valid remoting context will exist
    /// in which to carry out the operation.  So, if you need a new filter type
    /// put it in a plugin.
    /// </remarks>
    [Serializable]
    public abstract class Filter<T>
    {
        /// <summary>
        /// Determines whether the filter matches the value.
        /// </summary>
        /// <param name="value">The value to consider, never null</param>
        /// <returns>True if the filter matches the value</returns>
        public abstract bool IsMatch(T value);
    }
}
