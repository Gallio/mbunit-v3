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
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Model;
using Gallio.Icarus.Utilities;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    public sealed class OptionsController : IOptionsController, INotifyPropertyChanged
    {
        private Settings settings;
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private MRUList recentProjects;

        private readonly List<string> unselectedTreeViewCategoriesList = new List<string>();

        public bool AlwaysReloadAssemblies
        {
            get { return settings.AlwaysReloadAssemblies; }
            set { settings.AlwaysReloadAssemblies = value; }
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

        public string TestRunnerFactory
        {
            get { return settings.TestRunnerFactory; }
            set
            {
                settings.TestRunnerFactory = value;
                OnPropertyChanged("TestRunnerFactory");
            }
        }

        public BindingList<string> PluginDirectories { get; private set; }

        public BindingList<string> SelectedTreeViewCategories { get; private set; }

        public BindingList<string> UnselectedTreeViewCategories { get; private set; }

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

        public MRUList RecentProjects
        {
            get
            {
                if (recentProjects == null)
                {
                    // remove any dead projects
                    var list = new List<string>();
                    foreach(var proj in settings.RecentProjects)
                    {
                        if (fileSystem.FileExists(proj))
                            list.Add(proj);
                    }
                    settings.RecentProjects.Clear();
                    settings.RecentProjects.AddRange(list);

                    recentProjects = new MRUList(settings.RecentProjects, 10);
                }
                return recentProjects;
            }
        }

        public OptionsController(IFileSystem fileSystem, IXmlSerializer xmlSerializer,
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
        }

        public void Load()
        {
            settings = LoadSettings(Paths.SettingsFile) ?? new Settings();

            if (settings.TreeViewCategories.Count == 0)
                settings.TreeViewCategories.AddRange(new[] { "Namespace", MetadataKeys.AuthorName, MetadataKeys.Category, 
                    MetadataKeys.Importance, MetadataKeys.TestsOn });

            unselectedTreeViewCategoriesList.Clear();
            foreach (FieldInfo fi in typeof(MetadataKeys).GetFields())
            {
                if (!settings.TreeViewCategories.Contains(fi.Name))
                    unselectedTreeViewCategoriesList.Add(fi.Name);
            }

            PluginDirectories = new BindingList<string>(settings.PluginDirectories);
            SelectedTreeViewCategories = new BindingList<string>(settings.TreeViewCategories);
            UnselectedTreeViewCategories = new BindingList<string>(unselectedTreeViewCategoriesList);
            AddIns = new BindingList<string>(settings.AddIns);
        }

        private Settings LoadSettings(string fileName)
        {
            try
            {
                if (fileSystem.FileExists(fileName))
                    return xmlSerializer.LoadFromXml<Settings>(fileName);
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report("An exception occurred while loading Icarus settings file.", ex);
            }
            return null;    
        }

        public void Save()
        {
            try
            {
                xmlSerializer.SaveToXml(settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report("An exception occurred while saving Icarus settings file.", ex);
            }
        }

        public BindingList<string> AddIns { get; private set; }
        
        public bool GenerateReportAfterTestRun
        {
            get { return settings.GenerateReportAfterTestRun; }
            set { settings.GenerateReportAfterTestRun = value; }
        }

        public void Cancel()
        {
            Load();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
