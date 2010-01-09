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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Gallio.Icarus.TreeBuilders;
using Gallio.Icarus.Utilities;
using Gallio.Runtime.Logging;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IOptionsController : INotifyPropertyChanged
    {
        bool AlwaysReloadFiles { get; set; }
        bool RunTestsAfterReload { get; set; }
        string TestStatusBarStyle { get; set; }
        bool ShowProgressDialogs { get; set; }
        bool RestorePreviousSettings { get; set; }
        Observable<string> TestRunnerFactory { get; set; }
        Observable<IList<string>> SelectedTreeViewCategories { get; }
        Observable<IList<string>> UnselectedTreeViewCategories { get; }
        Color PassedColor { get; set; }
        Color FailedColor { get; set; }
        Color InconclusiveColor { get; set; }
        Color SkippedColor { get; set; }
        double UpdateDelay { get; }
        Size Size { get; set; }
        Point Location { get; set; }
        FormWindowState WindowState { get; set; }
        bool GenerateReportAfterTestRun { get; set; }
        MRUList RecentProjects { get; }
        LogSeverity MinLogSeverity { get; set; }
        bool AnnotationsShowErrors { get; set; }
        bool AnnotationsShowWarnings { get; set; }
        bool AnnotationsShowInfos { get; set; }
        IList<string> TestRunnerExtensions { get; }
        NamespaceHierarchy NamespaceHierarchy { get; set; }

        void Cancel();
        void Load();
        void Save();
    }
}
