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

using Gallio.Common.IO;
using Gallio.Common.Xml;
using System;
using Gallio.UI.Common.Policies;

namespace Gallio.Icarus.Options
{
    internal class OptionsManager : IOptionsManager
    {
        private readonly IFileSystem fileSystem;
        private readonly IXmlSerializer xmlSerializer;
        private readonly IUnhandledExceptionPolicy unhandledExceptionPolicy;
        private Settings settings = new Settings();

        public Settings Settings
        {
            get { return settings; }
        }

        public OptionsManager(IFileSystem fileSystem, IXmlSerializer xmlSerializer, 
            IUnhandledExceptionPolicy unhandledExceptionPolicy)
        {
            this.fileSystem = fileSystem;
            this.xmlSerializer = xmlSerializer;
            this.unhandledExceptionPolicy = unhandledExceptionPolicy;
        }

        public void Load()
        {
            try
            {
                if (fileSystem.FileExists(Paths.SettingsFile))
                    settings = xmlSerializer.LoadFromXml<Settings>(Paths.SettingsFile);
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

                xmlSerializer.SaveToXml(Settings, Paths.SettingsFile);
            }
            catch (Exception ex)
            {
                unhandledExceptionPolicy.Report("An exception occurred while saving Icarus settings file.", ex);
            }
        }
    }
}
