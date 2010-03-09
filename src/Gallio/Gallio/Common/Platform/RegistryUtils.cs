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
using System.Reflection;
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
        /// and the requested processor architecture affinity.
        /// </summary>
        /// <remarks>
        /// If <paramref name="architecture" /> requests a 32bit architecture, then only
        /// the 32bit registry is searched.  Likewise if it requests a 64bit architecture,
        /// then only the 64bit registry is searched.  Otherwise searches the 64bit
        /// registry first then falls back on the 32bit registry.
        /// </remarks>
        /// <param name="architecture">The processor architecture affinity.</param>
        /// <param name="hive">The registry hive.</param>
        /// <param name="keyName">The key name.</param>
        /// <param name="action">The action to perform on the sub key.  Should return true on success.</param>
        /// <returns>True if the action succeeded.</returns>
        /// <seealso cref="RegistryKey.OpenSubKey(string)"/>
        public static bool TryActionOnOpenSubKeyWithBitness(ProcessorArchitecture architecture,
            RegistryHive hive, string keyName, Func<RegistryKey, bool> action)
        {
            if (keyName == null)
                throw new ArgumentNullException("keyName");

            if (architecture != ProcessorArchitecture.X86)
            {
                using (RegistryKey subKey = OpenKey(hive, keyName, true))
                {
                    if (subKey != null && action(subKey))
                        return true;
                }
            }

            if (architecture != ProcessorArchitecture.Amd64 && architecture != ProcessorArchitecture.IA64)
            {
                using (RegistryKey subKey = OpenKey(hive, keyName, false))
                {
                    if (subKey != null && action(subKey))
                        return true;
                }
            }

            return false;
        }

        private static RegistryKey OpenKey(RegistryHive hive, string keyName, bool is64Bit)
        {
            Type registryViewType = Type.GetType("Microsoft.Win32.RegistryView, mscorlib", false);
            if (registryViewType != null)
            {
                // On .Net 4.0.
                MethodInfo openBaseKeyMethod = typeof(RegistryKey).GetMethod("OpenBaseKey");
                var registryView = Enum.ToObject(registryViewType, is64Bit ? 0x100 : 0x200);
                var baseKey = (RegistryKey) openBaseKeyMethod.Invoke(null, new object[] { hive, registryView });
                return baseKey.OpenSubKey(keyName);
            }
            else
            {
                // On .Net 2.0.  Fake it.
                RegistryKey baseKey = OpenBaseKey(hive);
                if (ProcessSupport.Is64BitProcess && ! is64Bit)
                    keyName = keyName.ToLower().Replace(@"software\", @"software\wow6432node\");
                return baseKey.OpenSubKey(keyName);
            }
        }

        private static RegistryKey OpenBaseKey(RegistryHive hive)
        {
            switch (hive)
            {
                case RegistryHive.ClassesRoot:
                    return Registry.ClassesRoot;
                case RegistryHive.CurrentUser:
                    return Registry.CurrentUser;
                case RegistryHive.LocalMachine:
                    return Registry.LocalMachine;
                case RegistryHive.Users:
                    return Registry.Users;
                case RegistryHive.PerformanceData:
                case RegistryHive.DynData:
                    return Registry.PerformanceData;
                case RegistryHive.CurrentConfig:
                    return Registry.CurrentConfig;
                default:
                    throw new ArgumentOutOfRangeException("hive");
            }
        }

        /// <summary>
        /// Gets a registry value taking into account the bitness of the platform
        /// and the requested processor architecture affinity.
        /// </summary>
        /// <remarks>
        /// If <paramref name="architecture" /> requests a 32bit architecture, then only
        /// the 32bit registry is searched.  Likewise if it requests a 64bit architecture,
        /// then only the 64bit registry is searched.  Otherwise searches the 64bit
        /// registry first then falls back on the 32bit registry.
        /// </remarks>
        /// <param name="architecture">The processor architecture affinity.</param>
        /// <param name="hive">The registry hive.</param>
        /// <param name="keyName">The key name.</param>
        /// <param name="valueName">The value name, or null to read the key's value.</param>
        /// <param name="defaultValue">The default value to return.</param>
        /// <returns>The value, or null if the key does not exist.</returns>
        /// <seealso cref="Registry.GetValue(string, string, object)"/>
        public static object GetValueWithBitness(ProcessorArchitecture architecture,
            RegistryHive hive, string keyName, string valueName, string defaultValue)
        {
            if (keyName == null)
                throw new ArgumentNullException("keyName");

            object value = defaultValue;
            TryActionOnOpenSubKeyWithBitness(architecture, hive, keyName, key =>
            {
                object maybeValue = key.GetValue(valueName, Sentinel);
                if (maybeValue == Sentinel)
                    return false;

                value = maybeValue;
                return true;
            });
            return value;
        }
    }
}
