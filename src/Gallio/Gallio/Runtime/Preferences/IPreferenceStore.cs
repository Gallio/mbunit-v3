// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// Stores and retrieves the contents of preference sets.
    /// </summary>
    /// <seealso cref="IPreferenceSet"/>
    /// <seealso cref="IPreferenceManager"/>
    public interface IPreferenceStore
    {
        /// <summary>
        /// Gets the preference set with the specified name.
        /// </summary>
        /// <param name="preferenceSetName">The name of the preference set.</param>
        /// <returns>The preference set.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="preferenceSetName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="preferenceSetName"/> is empty.</exception>
        IPreferenceSet this[string preferenceSetName] { get; }
    }
}
