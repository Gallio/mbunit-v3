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
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Icarus.Controls;
using Gallio.Icarus.TreeBuilders;
using Gallio.Runtime.Logging;
using Gallio.Runner;

namespace Gallio.Icarus
{
    [Serializable]
    [XmlRoot("settings", Namespace = SchemaConstants.XmlNamespace)]
    [XmlType(Namespace = SchemaConstants.XmlNamespace)]
    public class Settings
    {
        private string testRunnerFactory = StandardTestRunnerFactoryNames.IsolatedProcess;
        private bool restorePreviousSettings = true;
        private readonly List<string> recentProjects = new List<string>();
        private bool showProgressDialogs = true;
        private string testStatusBarStyle = TestStatusBarStyles.Integration;
        private int passedColor = Color.Green.ToArgb();
        private int failedColor = Color.Red.ToArgb();
        private int inconclusiveColor = Color.Gold.ToArgb();
        private int skippedColor = Color.SlateGray.ToArgb();
        private bool generateReportAfterTestRun = true;
        private bool recursiveExecutionLog = true;
        private bool _autoSaveProject = false;
        private readonly List<string> extensionSpecifications = new List<string>();

        public static readonly string Extension = ".settings";

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
        public bool AlwaysReloadFiles { get; set; }

        [XmlElement("runTestsAfterReload")]
        public bool RunTestsAfterReload { get; set; }

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
            get; set;
        }

        [XmlElement("size")]
        public Size Size { get; set; }

        [XmlElement("location")]
        public Point Location { get; set; }

        [XmlElement("windowState")]
        public FormWindowState WindowState { get; set; }

        [XmlElement("lastProject")]
        public string LastProject { get; set; }

        [XmlArray("recentProjects", IsNullable = false)]
        [XmlArrayItem("recentProject", typeof(string), IsNullable = false)]
        public List<string> RecentProjects
        {
            get { return recentProjects; }
        }

        [XmlElement("generateReportAfterTestRun")]
        public bool GenerateReportAfterTestRun
        {
            get { return generateReportAfterTestRun;}
            set { generateReportAfterTestRun = value; }
        }

        [XmlElement("minLogSeverity")]
        public LogSeverity MinLogSeverity { get; set; }

        [XmlElement("annotationsShowErrors")]
        public bool AnnotationsShowErrors { get; set; }

        [XmlElement("annotationsShowWarnings")]
        public bool AnnotationsShowWarnings { get; set; }

        [XmlElement("annotationsShowInfos")]
        public bool AnnotationsShowInfos { get; set; }

        [XmlArray("extensionSpecifications", IsNullable = false)]
        [XmlArrayItem("extensionSpecification", typeof(string), IsNullable = false)]
        public List<string> TestRunnerExtensions
        {
            get { return extensionSpecifications; }
        }

        [XmlElement("namespaceHierarchy")]
        public NamespaceHierarchy NamespaceHierarchy { get; set; }

        public Settings()
        {
            TreeViewCategories = new List<string>();
            MinLogSeverity = LogSeverity.Info;

            AnnotationsShowErrors = true;
            AnnotationsShowInfos = true;
            AnnotationsShowWarnings = true;
        }

        [XmlElement("recursiveExecutionLog")]
        public bool RecursiveExecutionLog { 
            get { return recursiveExecutionLog; }
            set { recursiveExecutionLog = value; } 
        }

        [XmlElement("autoSave")]
        public bool AutoSaveProject
        {
          get { return _autoSaveProject; }
          set { _autoSaveProject = value; }
        }
    }
}
