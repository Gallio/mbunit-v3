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
using System.IO;
using Gallio.Common.Policies;
using Gallio.Common.Xml;

namespace Gallio.Icarus.Options
{
    internal class OptionsManager
    {
        private static Settings settings = new Settings();

        public static Settings Settings
        {
            get { return settings; }
        }

        public static void Load()
        {
            try
            {
                if (File.Exists(Paths.SettingsFile))
                    settings = XmlSerializationUtils.LoadFromXml<Settings>(Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while loading Icarus settings file.", ex);
            }
        }

        public static void Save()
        {
            try
            {
                // create folder, if necessary
                if (!Directory.Exists(Paths.IcarusAppDataFolder))
                    Directory.CreateDirectory(Paths.IcarusAppDataFolder);

                XmlSerializationUtils.SaveToXml(Settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while saving Icarus settings file.", ex);
            }
        }
    }
}
