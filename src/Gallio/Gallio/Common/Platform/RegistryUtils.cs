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
        /// Opens a registry subkey taking into account the bitness of the platform.
        /// </summary>
        /// <param name="key">The registry key.</param>
        /// <param name="subKeyName">The subkey name.</param>
        /// <param name="alternateSubKeyNameFor64Bit">The alternate subkey name to try on 64bit platforms if the other subkey cannot be found.  (The name should probably include "Wow6432Node".)</param>
        /// <returns>The opened subkey, or null if it does not exist.</returns>
        /// <seealso cref="RegistryKey.OpenSubKey(string)"/>
        public static RegistryKey OpenSubKeyWithBitness(RegistryKey key, string subKeyName, string alternateSubKeyNameFor64Bit)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (subKeyName == null)
                throw new ArgumentNullException("subKeyName");
            if (alternateSubKeyNameFor64Bit == null)
                throw new ArgumentNullException("alternateSubKeyNameFor64Bit");

            RegistryKey subKey = key.OpenSubKey(subKeyName);

            if (subKey == null && ProcessSupport.Is64BitProcess)
                subKey = key.OpenSubKey(alternateSubKeyNameFor64Bit);

            return subKey;
        }

        /// <summary>
        /// Gets a registry value taking into account the bitness of the platform.
        /// </summary>
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
