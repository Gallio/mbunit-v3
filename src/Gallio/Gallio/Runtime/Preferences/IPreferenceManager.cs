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
using Gallio.Runtime.Security;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// Stores and retrieves user and machine preferences.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For the API related to defining new preference panes and control panels
    /// refer to the types in the Gallio.UI assembly.
    /// </para>
    /// </remarks>
    /// <seealso cref="IPreferenceSet"/>
    /// <seealso cref="IPreferenceStore"/>
    public interface IPreferenceManager
    {
        /// <summary>
        /// Gets a preference store for the current local user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// These preferences are stored in the user's local application data directory.
        /// </para>
        /// </remarks>
        IPreferenceStore LocalUserPreferences { get; }

        /// <summary>
        /// Gets a preference store for the current roaming user.
        /// </summary>
        /// <remarks>
        /// <para>
        /// These preferences are stored in the user's roaming application data directory.
        /// </para>
        /// </remarks>
        IPreferenceStore RoamingUserPreferences { get; }

        /// <summary>
        /// Gets a preference store common to all users of this system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// These preferences are stored in the system's common application data directory.
        /// </para>
        /// <para>
        /// All users can read common preferences but only administratators may
        /// edit them.  Moreover privilege elevation may be required on Vista and
        /// more recent Windows OS's.
        /// <seealso cref="IElevationManager"/>
        /// </para>
        /// </remarks>
        IPreferenceStore CommonPreferences { get; }
    }
}
