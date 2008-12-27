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

using System.ComponentModel;
using System.Drawing;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IOptionsController
    {
        bool AlwaysReloadAssemblies { get; set; }
        string TestStatusBarStyle { get; set; }
        bool ShowProgressDialogs { get; set; }
        bool RestorePreviousSettings { get; set; }
        string TestRunnerFactory { get; set; }
        BindingList<string> PluginDirectories { get; }
        BindingList<string> SelectedTreeViewCategories { get; }
        BindingList<string> UnselectedTreeViewCategories { get; }
        Color PassedColor { get; set; }
        Color FailedColor { get; set; }
        Color InconclusiveColor { get; set; }
        Color SkippedColor { get; set; }
        double UpdateDelay { get; }

        void Cancel();
        void Load();
        void Save();
    }
}
