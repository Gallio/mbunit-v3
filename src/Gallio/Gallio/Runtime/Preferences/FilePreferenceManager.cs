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
using System.IO;
using System.Text;
using Gallio.Common.Policies;

namespace Gallio.Runtime.Preferences
{
    /// <summary>
    /// An implementation of <see cref="IPreferenceManager" /> based on files.
    /// </summary>
    public class FilePreferenceManager : IPreferenceManager
    {
        private readonly Dictionary<string, IPreferenceStore> cachedPreferenceStores;
        private readonly SpecialPathPolicy specialPathPolicy;

        /// <summary>
        /// Initializes the preference manager.
        /// </summary>
        public FilePreferenceManager()
        {
            cachedPreferenceStores = new Dictionary<string, IPreferenceStore>();
            specialPathPolicy = SpecialPathPolicy.For("Preferences");
        }

        /// <inheritdoc />
        public IPreferenceStore LocalUserPreferences
        {
            get { return GetPreferenceStore(specialPathPolicy.GetLocalUserApplicationDataDirectory()); }
        }

        /// <inheritdoc />
        public IPreferenceStore RoamingUserPreferences
        {
            get { return GetPreferenceStore(specialPathPolicy.GetRoamingUserApplicationDataDirectory()); }
        }

        /// <inheritdoc />
        public IPreferenceStore CommonPreferences
        {
            get { return GetPreferenceStore(specialPathPolicy.GetCommonApplicationDataDirectory()); }
        }

        private IPreferenceStore GetPreferenceStore(DirectoryInfo preferenceStoreDir)
        {
            lock (cachedPreferenceStores)
            {
                string preferenceStoreDirPath = preferenceStoreDir.FullName;
                IPreferenceStore result;
                if (!cachedPreferenceStores.TryGetValue(preferenceStoreDirPath, out result))
                {
                    result = new FilePreferenceStore(preferenceStoreDir);
                    cachedPreferenceStores.Add(preferenceStoreDirPath, result);
                }

                return result;
            }
        }
    }
}
