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
using Gallio.Common.Collections;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// Provides operations for writing preference settings.
    /// </summary>
    public interface IPreferenceSetWriter : IPreferenceSetReader
    {
        /// <summary>
        /// Sets a preference setting.
        /// </summary>
        /// <param name="preferenceSettingKey">The preference setting key.</param>
        /// <param name="value">The value to set, or null to remove the value.</param>
        /// <typeparam name="T">The setting value type.</typeparam>
        void SetSetting<T>(Key<T> preferenceSettingKey, T value);

        /// <summary>
        /// Removes a preference setting.
        /// </summary>
        /// <param name="preferenceSettingKey">The preference setting key.</param>
        void RemoveSetting<T>(Key<T> preferenceSettingKey);
    }
}
