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
using Gallio.Runtime.Preferences;

namespace Gallio.TDNetRunner.Core
{
    public class TDNetPreferenceManager
    {
        private readonly IPreferenceManager preferenceManager;

        public TDNetPreferenceManager(IPreferenceManager preferenceManager)
        {
            this.preferenceManager = preferenceManager;
        }

        private IPreferenceSet PreferenceSet
        {
            get { return preferenceManager.CommonPreferences["Gallio.TDNetRunner"]; }
        }

        private static Key<TDNetRunnerInstallationMode> FrameworkInstallationModeKey(string frameworkId)
        {
            return new Key<TDNetRunnerInstallationMode>("InstallationMode." + frameworkId);
        }

        public TDNetRunnerInstallationMode GetInstallationModeForFramework(string frameworkId)
        {
            return PreferenceSet.Read(reader => reader.GetSetting(
                FrameworkInstallationModeKey(frameworkId), TDNetRunnerInstallationMode.Default));
        }

        public void SetInstallationModeForFramework(string frameworkId, TDNetRunnerInstallationMode mode)
        {
            PreferenceSet.Write(writer => writer.SetSetting(
                FrameworkInstallationModeKey(frameworkId), mode));
        }
    }
}
