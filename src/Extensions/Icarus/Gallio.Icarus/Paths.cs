// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;
using System.IO;

namespace Gallio.Icarus
{
    public static class Paths
    {
        public static string IcarusAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Gallio\Icarus");
        public static string DefaultProject = Path.Combine(IcarusAppDataFolder, @"Icarus.gallio");
        public static string SettingsFile = Path.Combine(IcarusAppDataFolder, @"Icarus.settings");
        public static string DockConfigFile = Path.Combine(IcarusAppDataFolder, @"DockPanel.config");
    }
}
