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
