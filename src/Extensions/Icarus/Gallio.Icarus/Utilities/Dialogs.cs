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

using System.Windows.Forms;
using Gallio.Icarus.Properties;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Utilities
{
    public static class Dialogs
    {
        private static readonly string projectFileFilter = string.Format("Gallio Projects (*{0})|*{0}",
            TestProject.Extension);

        public static OpenFileDialog CreateAddFilesDialog()
        {
            return new OpenFileDialog
            {
                Title = Resources.Dialogs_CreateAddFilesDialog_Add_Files___,
                Filter = "Test Files (*.*)|*.*",
                Multiselect = true
            };
        }

        public static OpenFileDialog CreateOpenProjectDialog()
        {
            return new OpenFileDialog
            {
                Title = Resources.Dialogs_CreateOpenProjectDialog_Open_Project___,
                Filter = projectFileFilter
            };
        }

        public static SaveFileDialog CreateSaveProjectDialog()
        {
            return new SaveFileDialog
            {
                Title = Resources.Dialogs_CreateSaveProjectDialog_Save_Project_As___,
                OverwritePrompt = true,
                AddExtension = true,
                DefaultExt = projectFileFilter,
                Filter = projectFileFilter
            };
        }
    }
}
