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
        /// <param name="preferenceSettingKey">The preference setting key</param>
        /// <param name="value">The value to set, or null to remove the value</param>
        /// <typeparam name="T">The setting value type</typeparam>
        void SetSetting<T>(Key<T> preferenceSettingKey, T value);

        /// <summary>
        /// Removes a preference setting.
        /// </summary>
        /// <param name="preferenceSettingKey">The preference setting key</param>
        void RemoveSetting<T>(Key<T> preferenceSettingKey);
    }
}
