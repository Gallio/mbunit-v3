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
using System.Collections.Generic;
using System.Text;
using Gallio.Common.Collections;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// Provides operations for reading preference settings within preference sets.
    /// </summary>
    public interface IPreferenceSetReader
    {
        // TODO: Add GetSetting variant to subscribe for change notifications.

        /// <summary>
        /// Gets a preference setting.
        /// </summary>
        /// <param name="preferenceSettingKey">The preference setting key.</param>
        /// <returns>The setting value or <c>default(T)</c> if none</returns>
        /// <typeparam name="T">The setting value type.</typeparam>
        T GetSetting<T>(Key<T> preferenceSettingKey);

        /// <summary>
        /// Gets a preference setting.
        /// </summary>
        /// <param name="preferenceSettingKey">The preference setting key.</param>
        /// <param name="defaultValue">The default value to return in case the setting does not exist.</param>
        /// <returns>The setting value or <paramref name="defaultValue"/> if none</returns>
        /// <typeparam name="T">The setting value type.</typeparam>
        T GetSetting<T>(Key<T> preferenceSettingKey, T defaultValue);

        /// <summary>
        /// Returns true if the preference set contains a preference setting.
        /// </summary>
        /// <param name="preferenceSettingKey">The preference setting key.</param>
        /// <returns>True setting exists</returns>
        bool HasSetting<T>(Key<T> preferenceSettingKey);
    }
}
