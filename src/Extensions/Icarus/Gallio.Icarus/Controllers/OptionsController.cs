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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Runner;
using Gallio.Utilities;
using Gallio.Model;
using Gallio.Icarus.Utilities;

namespace Gallio.Icarus.Controllers
{
    public sealed class OptionsController : IOptionsController
    {
        private Settings settings;
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerialization xmlSerialization;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;

        private readonly BindingList<string> pluginDirectories;
        private readonly BindingList<string> selectedTreeViewCategories;
        private readonly BindingList<string> unselectedTreeViewCategories;
        private readonly List<string> unselectedTreeViewCategoriesList = new List<string>();

        public bool AlwaysReloadAssemblies
        {
            get { return settings.AlwaysReloadAssemblies; }
            set { settings.AlwaysReloadAssemblies = value; }
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
            set { settings.TestRunnerFactory = value; }
        }

        public string[] TestRunnerFactories
        {
            get
            {
                List<string> items = new List<string>();
                foreach (FieldInfo fi in typeof(StandardTestRunnerFactoryNames).GetFields())
                    items.Add(fi.Name);
                return items.ToArray();
            }
        }

        public BindingList<string> PluginDirectories
        {
            get { return pluginDirectories; }
        }

        public BindingList<string> SelectedTreeViewCategories
        {
            get { return selectedTreeViewCategories; }
        }

        public BindingList<string> UnselectedTreeViewCategories
        {
            get { return unselectedTreeViewCategories; }
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

        private OptionsController() : this(new FileSystem(), new XmlSerialization(), 
            new UnhandledExceptionPolicy())
        { }

        internal OptionsController(IFileSystem fileSystem, IXmlSerialization xmlSerialization, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.fileSystem = fileSystem;
            this.xmlSerialization = xmlSerialization;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;

            Load();

            pluginDirectories = new BindingList<string>(settings.PluginDirectories);
            selectedTreeViewCategories = new BindingList<string>(settings.TreeViewCategories);
            unselectedTreeViewCategories = new BindingList<string>(unselectedTreeViewCategoriesList);
        }

        internal static OptionsController Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            static Nested()
            { }

            internal static readonly OptionsController instance = new OptionsController();
        }

        private void Load()
        {
            settings = LoadSettings(Paths.SettingsFile) ?? new Settings();

            if (settings.TreeViewCategories.Count == 0)
                settings.TreeViewCategories.AddRange(new[] { "Namespace", MetadataKeys.AuthorName, MetadataKeys.CategoryName, 
                    MetadataKeys.Importance, MetadataKeys.TestsOn });

            unselectedTreeViewCategoriesList.Clear();
            foreach (FieldInfo fi in typeof(MetadataKeys).GetFields())
            {
                if (!settings.TreeViewCategories.Contains(fi.Name))
                    unselectedTreeViewCategoriesList.Add(fi.Name);
            }
        }

        private Settings LoadSettings(string fileName)
        {
            try
            {
                if (fileSystem.FileExists(fileName))
                    return xmlSerialization.LoadFromXml<Settings>(fileName);
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
                xmlSerialization.SaveToXml(settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report("An exception occurred while saving Icarus settings file.", ex);
            }
        }

        public void Cancel()
        {
            Load();
        }
    }
}
