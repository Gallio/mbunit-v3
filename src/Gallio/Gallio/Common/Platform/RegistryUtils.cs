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
using Microsoft.Win32;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Provides additional utilities for manipulating the Windows registry, particularly
    /// to take care of differences on 32bit and 64bit platforms.
    /// </summary>
    public static class RegistryUtils
    {
        private static readonly object Sentinel = new object();

        /// <summary>
        /// Opens a registry subkey taking into account the bitness of the platform
        /// and attempts to perform an action.  If the action fails and running on x64,
        /// then tries again with the alternate key.
        /// </summary>
        /// <remarks>
        /// The basic idea is that we try to use the contents of the 64bit
        /// registry first then fallback on the alternate key to access the contents of
        /// the 32bit registry when the process is 64bit.  This way 64bit processes
        /// can preferentially use 64bit registry contents.
        /// </remarks>
        /// <param name="key">The registry key.</param>
        /// <param name="subKeyName">The subkey name.</param>
        /// <param name="alternateSubKeyNameFor64Bit">The alternate subkey name to try on 64bit platforms if the other subkey cannot be found.  (The name should probably include "Wow6432Node".)</param>
        /// <param name="action">The action to perform on the sub key.  Should return true on success.</param>
        /// <returns>True if the action succeeded.</returns>
        /// <seealso cref="RegistryKey.OpenSubKey(string)"/>
        public static bool TryActionOnOpenSubKeyWithBitness(RegistryKey key, string subKeyName, string alternateSubKeyNameFor64Bit,
            Func<RegistryKey, bool> action)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (subKeyName == null)
                throw new ArgumentNullException("subKeyName");
            if (alternateSubKeyNameFor64Bit == null)
                throw new ArgumentNullException("alternateSubKeyNameFor64Bit");

            using (RegistryKey subKey = key.OpenSubKey(subKeyName))
            {
                if (subKey != null && action(subKey))
                    return true;
            }

            if (ProcessSupport.Is64BitProcess)
            {
                using (RegistryKey subKey = key.OpenSubKey(alternateSubKeyNameFor64Bit))
                {
                    if (subKey != null && action(subKey))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a registry value taking into account the bitness of the platform.
        /// </summary>
        /// <remarks>
        /// The basic idea is that we try to use the contents of the 64bit
        /// registry first then fallback on the alternate key to access the contents of
        /// the 32bit registry when the process is 64bit.  This way 64bit processes
        /// can preferentially use 64bit registry contents.
        /// </remarks>
        /// <param name="keyName">The key name.</param>
        /// <param name="alternateKeyNameFor64Bit">The alternate key name to try on 64bit platforms if the other key cannot be found.  (The name should probably include "Wow6432Node".)</param>
        /// <param name="valueName">The value name, or null to read the key's value.</param>
        /// <param name="defaultValueIfKeyExistsButValueDoesNot">The default value to return if the key exists but the value does not.</param>
        /// <returns>The value, or null if the key does not exist.</returns>
        /// <seealso cref="Registry.GetValue(string, string, object)"/>
        public static object GetValueWithBitness(string keyName, string alternateKeyNameFor64Bit, string valueName, string defaultValueIfKeyExistsButValueDoesNot)
        {
            if (keyName == null)
                throw new ArgumentNullException("keyName");
            if (alternateKeyNameFor64Bit == null)
                throw new ArgumentNullException("alternateKeyNameFor64Bit");

            object value = Registry.GetValue(keyName, valueName, Sentinel);

            if (value == null || value == Sentinel)
            {
                if (ProcessSupport.Is64BitProcess)
                    value = Registry.GetValue(alternateKeyNameFor64Bit, valueName, defaultValueIfKeyExistsButValueDoesNot);
                else if (value == Sentinel)
                    value = defaultValueIfKeyExistsButValueDoesNot;
            }

            return value;
        }
    }
}
