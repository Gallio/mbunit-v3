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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gallio.Common.IO;
using Gallio.Common.Xml;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Events;
using Gallio.Icarus.TreeBuilders;
using Gallio.Model;
using Gallio.Icarus.Utilities;
using Gallio.Runtime.Logging;
using Gallio.UI.Common.Policies;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Controllers
{
    public class OptionsController : NotifyController, IOptionsController, Handles<ProjectOpened>, Handles<ProjectSaved>
    {
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        private Settings settings = new Settings();
        private MRUList recentProjects;

        public bool AlwaysReloadFiles
        {
            get { return settings.AlwaysReloadFiles; }
            set { settings.AlwaysReloadFiles = value; }
        }

        public bool RunTestsAfterReload
        {
            get { return settings.RunTestsAfterReload; }
            set { settings.RunTestsAfterReload = value; }
        }

        public string TestStatusBarStyle
        {
            get { return settings.TestStatusBarStyle; }
            set { settings.TestStatusBarStyle = value; }
        }

        public bool ShowProgressDialogs
        {
            get { return settings.ShowProgressDialogs; }
            set { settings.ShowProgressDialogs = value; }
        }

        public bool RestorePreviousSettings
        {
            get { return settings.RestorePreviousSettings; }
            set { settings.RestorePreviousSettings = value; }
        }

        public Observable<string> TestRunnerFactory
        {
            get; set;
        }

        public Observable<IList<string>> SelectedTreeViewCategories
        {
            get; private set;
        }

        public Observable<IList<string>> UnselectedTreeViewCategories
        {
            get; private set;
        }

        public Color PassedColor
        {
            get { return Color.FromArgb(settings.PassedColor); }
            set { settings.PassedColor = value.ToArgb(); }
        }

        public Color FailedColor
        {
            get { return Color.FromArgb(settings.FailedColor); }
            set { settings.FailedColor = value.ToArgb(); }
        }

        public Color InconclusiveColor
        {
            get { return Color.FromArgb(settings.InconclusiveColor); }
            set { settings.InconclusiveColor = value.ToArgb(); }
        }

        public Color SkippedColor
        {
            get { return Color.FromArgb(settings.SkippedColor); }
            set { settings.SkippedColor = value.ToArgb(); }
        }

        public double UpdateDelay
        {
            get { return 1000; }
        }

        public Size Size
        {
            get { return settings.Size; }
            set { settings.Size = value; }
        }

        public Point Location
        {
            get { return settings.Location; }
            set { settings.Location = value; }
        }

        public FormWindowState WindowState
        {
            get { return settings.WindowState; }
            set { settings.WindowState = value; }
        }

        public MRUList RecentProjects
        {
            get
            {
                if (recentProjects == null)
                {
                    recentProjects = new MRUList(settings.RecentProjects, 10);
                    recentProjects.PropertyChanged += (sender, e) =>
                        {
                            if (e.PropertyName == "Items")
                                OnPropertyChanged(new PropertyChangedEventArgs("RecentProjects"));
                        };
                }
                return recentProjects;
            }
        }

        public LogSeverity MinLogSeverity
        {
            get { return settings.MinLogSeverity; }
            set { settings.MinLogSeverity = value; }
        }

        public bool AnnotationsShowErrors
        {
            get { return settings.AnnotationsShowErrors; }
            set { settings.AnnotationsShowErrors = value; }
        }

        public bool AnnotationsShowWarnings
        {
            get { return settings.AnnotationsShowWarnings; }
            set { settings.AnnotationsShowWarnings = value; }
        }

        public bool AnnotationsShowInfos
        {
            get { return settings.AnnotationsShowInfos; }
            set { settings.AnnotationsShowInfos = value; }
        }

        public IList<string> TestRunnerExtensions 
        {
            get 
            { 
                return settings.TestRunnerExtensions; 
            }
            set
            {
                settings.TestRunnerExtensions.Clear();
                settings.TestRunnerExtensions.AddRange(value);
            }
        }

        public NamespaceHierarchy NamespaceHierarchy
        {
            get { return settings.NamespaceHierarchy; }
            set { settings.NamespaceHierarchy = value; }
        }

        public OptionsController(IFileSystem fileSystem, IXmlSerializer xmlSerializer, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;

            TestRunnerFactory = new Observable<string>();
            TestRunnerFactory.PropertyChanged += (s, e) => 
                settings.TestRunnerFactory = TestRunnerFactory.Value;

            SelectedTreeViewCategories = new Observable<IList<string>>();
            SelectedTreeViewCategories.PropertyChanged += (s, e) => 
                settings.TreeViewCategories = new List<string>(SelectedTreeViewCategories.Value);
            
            UnselectedTreeViewCategories = new Observable<IList<string>>();

            Load();
        }

        public void Load()
        {
            try
            {
                settings = fileSystem.FileExists(Paths.SettingsFile) ? xmlSerializer.LoadFromXml<Settings>(Paths.SettingsFile) 
                    : new Settings();

                if (settings.TreeViewCategories.Count == 0)
                {
                    // add default categories
                    settings.TreeViewCategories.AddRange(new[] { "Namespace", MetadataKeys.AuthorName, 
                    MetadataKeys.Category, MetadataKeys.Importance, MetadataKeys.TestsOn });
                }
                SelectedTreeViewCategories.Value = new List<string>(settings.TreeViewCategories);

                var unselectedCategories = new List<string>();
                foreach (var fieldInfo in typeof(MetadataKeys).GetFields())
                {
                    if (!settings.TreeViewCategories.Contains(fieldInfo.Name))
                        unselectedCategories.Add(fieldInfo.Name);
                }
                UnselectedTreeViewCategories.Value = unselectedCategories;

                TestRunnerFactory.Value = settings.TestRunnerFactory;
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report("An exception occurred while loading Icarus settings file.", ex);
            }
        }

        public void Save()
        {
            try
            {
                // create folder, if necessary
                if (!fileSystem.DirectoryExists(Paths.IcarusAppDataFolder))
                    fileSystem.CreateDirectory(Paths.IcarusAppDataFolder);

                xmlSerializer.SaveToXml(settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report("An exception occurred while saving Icarus settings file.", ex);
            }
        }

        public bool GenerateReportAfterTestRun
        {
            get { return settings.GenerateReportAfterTestRun; }
            set { settings.GenerateReportAfterTestRun = value; }
        }

        public void Cancel()
        {
            Load();
        }

        public void Handle(ProjectOpened @event)
        {
            RecentProjects.Add(@event.ProjectLocation);
        }

        public void Handle(ProjectSaved @event)
        {
            RecentProjects.Add(@event.ProjectLocation);
        }
    }
}
