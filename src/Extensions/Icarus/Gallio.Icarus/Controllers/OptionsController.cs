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
using Gallio.Icarus.Interfaces;
using Gallio.Runner;
using Gallio.Runtime;
using Gallio.Utilities;

namespace Gallio.Icarus.Controllers
{
    internal class OptionsController : IOptionsController
    {
        private readonly Settings settings;
        private BindingList<string> pluginDirectories;

        public bool AlwaysReloadAssemblies
        {
            get { return settings.AlwaysReloadAssemblies; }
            set { settings.AlwaysReloadAssemblies = value; }
        }

        public string TestProgressBarStyle
        {
            get { return settings.TestProgressBarStyle; }
            set { settings.TestProgressBarStyle = value; }
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
            get
            {
                if (pluginDirectories == null)
                    pluginDirectories = new BindingList<string>(settings.PluginDirectories);
                return pluginDirectories;
            }
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

        private OptionsController()
        {
            settings = Load() ?? new Settings();
        }

        internal static OptionsController Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            static Nested()
            {
            }

            internal static readonly OptionsController instance = new OptionsController();
        }

        private static Settings Load()
        {
            try
            {
                if (File.Exists(Paths.SettingsFile))
                    return XmlSerializationUtils.LoadFromXml<Settings>(Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while saving the report.", ex);
            }
            return null;    
        }

        public void Save()
        {
            try
            {
                XmlSerializationUtils.SaveToXml(settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while saving the report.", ex);
            }
        }

        public void RemovePluginDirectory(string directory)
        {
            pluginDirectories.Remove(directory);
        }

        public void AddPluginDirectory(string directory)
        {
            pluginDirectories.Add(directory);
        }
    }
}
