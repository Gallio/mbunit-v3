// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Xml.Serialization;

using Gallio.Utilities;
using Gallio.Runner;
using System.Drawing;

namespace Gallio.Icarus
{
    [Serializable]
    [XmlRoot("settings", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public class Settings : System.ICloneable
    {
        private string testRunnerFactory = StandardTestRunnerFactoryNames.IsolatedProcess;
        private bool restorePreviousSettings = true;
        private readonly List<string> pluginDirectories = new List<string>();
        private bool alwaysReloadAssemblies;
        private bool showProgressDialogs = true;
        private string testProgressBarStyle = "Integration";
        private int passedColor = Color.Green.ToArgb();
        private int failedColor = Color.Red.ToArgb();
        private int inconclusiveColor = Color.Gold.ToArgb();
        private int skippedColor = Color.SlateGray.ToArgb();

        [XmlElement("testRunnerFactory")]
        public string TestRunnerFactory
        {
            get { return testRunnerFactory; }
            set { testRunnerFactory = value; }
        }

        [XmlElement("restorePreviousSettings")]
        public bool RestorePreviousSettings
        {
            get { return restorePreviousSettings; }
            set { restorePreviousSettings = value; }
        }

        [XmlElement("alwaysReloadTests")]
        public bool AlwaysReloadAssemblies
        {
            get { return alwaysReloadAssemblies; }
            set { alwaysReloadAssemblies = value; }
        }

        [XmlElement("showProgressDialogs")]
        public bool ShowProgressDialogs
        {
            get { return showProgressDialogs; }
            set { showProgressDialogs = value; }
        }

        [XmlElement("testProgressBarStyle")]
        public string TestProgressBarStyle
        {
            get { return testProgressBarStyle; }
            set { testProgressBarStyle = value; }
        }

        [XmlArray("pluginDirectories", IsNullable = false)]
        [XmlArrayItem("pluginDirectory", typeof(string), IsNullable = false)]
        public List<string> PluginDirectories
        {
            get { return pluginDirectories; }
            set
            {
                pluginDirectories.Clear();
                foreach (string dir in value)
                    pluginDirectories.Add(dir);
            }
        }

        [XmlElement("passedColor")]
        public int PassedColor
        {
            get { return passedColor; }
            set { passedColor = value; }
        }

        [XmlElement("failedColor")]
        public int FailedColor
        {
            get { return failedColor; }
            set { failedColor = value; }
        }

        [XmlElement("inconclusiveColor")]
        public int InconclusiveColor
        {
            get { return inconclusiveColor; }
            set { inconclusiveColor = value; }
        }

        [XmlElement("skippedColor")]
        public int SkippedColor
        {
            get { return skippedColor; }
            set { skippedColor = value; }
        }

        object System.ICloneable.Clone()
        {
            return Clone();
        }

        public Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }
    }
}