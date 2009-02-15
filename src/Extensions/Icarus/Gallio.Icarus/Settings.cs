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
using System.Xml.Serialization;
using Gallio.Icarus.Controls;
using Gallio.Utilities;
using Gallio.Runner;
using System.Drawing;

namespace Gallio.Icarus
{
    [Serializable]
    [XmlRoot("settings", Namespace = XmlSerializationUtils.GallioNamespace)]
    [XmlType(Namespace = XmlSerializationUtils.GallioNamespace)]
    public class Settings
    {
        private string testRunnerFactory = StandardTestRunnerFactoryNames.IsolatedProcess;
        private bool restorePreviousSettings = true;
        private readonly List<string> pluginDirectories = new List<string>();
        private bool showProgressDialogs = true;
        private string testStatusBarStyle = TestStatusBarStyles.Integration;
        private int passedColor = Color.Green.ToArgb();
        private int failedColor = Color.Red.ToArgb();
        private int inconclusiveColor = Color.Gold.ToArgb();
        private int skippedColor = Color.SlateGray.ToArgb();
        private readonly List<string> treeViewCategories = new List<string>();

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
        public bool AlwaysReloadAssemblies { get; set; }

        [XmlElement("showProgressDialogs")]
        public bool ShowProgressDialogs
        {
            get { return showProgressDialogs; }
            set { showProgressDialogs = value; }
        }

        [XmlElement("testStatusBarStyle")]
        public string TestStatusBarStyle
        {
            get { return testStatusBarStyle; }
            set { testStatusBarStyle = value; }
        }

        [XmlArray("pluginDirectories", IsNullable = false)]
        [XmlArrayItem("pluginDirectory", typeof(string), IsNullable = false)]
        public List<string> PluginDirectories
        {
            get { return pluginDirectories; }
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

        [XmlArray("treeViewCategories", IsNullable = false)]
        [XmlArrayItem("treeViewCategory", typeof(string), IsNullable = false)]
        public List<string> TreeViewCategories
        {
            get { return treeViewCategories; }
        }

        [XmlElement("size")]
        public Size Size { get; set; }

        [XmlElement("location")]
        public Point Location { get; set; }
    }
}
