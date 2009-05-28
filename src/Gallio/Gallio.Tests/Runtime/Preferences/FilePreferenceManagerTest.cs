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
using System.Linq;
using System.Text;
using Gallio.Common.Policies;
using Gallio.Runtime.Preferences;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Preferences
{
    [TestsOn(typeof(FilePreferenceManager))]
    public class FilePreferenceManagerTest
    {
        [Test]
        public void LocalUserPreferences_ReturnsFilePreferenceStoreInLocalUserAppData()
        {
            var manager = new FilePreferenceManager();

            var preferenceStore = (FilePreferenceStore)manager.LocalUserPreferences;

            Assert.AreEqual(SpecialPathPolicy.For("Preferences").GetLocalUserApplicationDataDirectory(), preferenceStore.PreferenceStoreDir);
        }

        [Test]
        public void LocalUserPreferences_ReturnSameInstanceEachTime()
        {
            var manager = new FilePreferenceManager();

            Assert.AreSame(manager.LocalUserPreferences, manager.LocalUserPreferences);
        }

        [Test]
        public void RoamingUserPreferences_ReturnsFilePreferenceStoreInRoamingUserAppData()
        {
            var manager = new FilePreferenceManager();

            var preferenceStore = (FilePreferenceStore)manager.RoamingUserPreferences;

            Assert.AreEqual(SpecialPathPolicy.For("Preferences").GetRoamingUserApplicationDataDirectory(), preferenceStore.PreferenceStoreDir);
        }

        [Test]
        public void RoamingUserPreferences_ReturnSameInstanceEachTime()
        {
            var manager = new FilePreferenceManager();

            Assert.AreSame(manager.RoamingUserPreferences, manager.RoamingUserPreferences);
        }

        [Test]
        public void CommonPreferences_ReturnsFilePreferenceStoreInCommonAppData()
        {
            var manager = new FilePreferenceManager();

            var preferenceStore = (FilePreferenceStore)manager.CommonPreferences;

            Assert.AreEqual(SpecialPathPolicy.For("Preferences").GetCommonApplicationDataDirectory(), preferenceStore.PreferenceStoreDir);
        }

        [Test]
        public void CommonPreferences_ReturnSameInstanceEachTime()
        {
            var manager = new FilePreferenceManager();

            Assert.AreSame(manager.CommonPreferences, manager.CommonPreferences);
        }
    }
}
