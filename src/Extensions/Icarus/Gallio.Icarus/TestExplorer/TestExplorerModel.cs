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
using System.Drawing;
using Aga.Controls.Tree;
using Gallio.Icarus.Models;
using Gallio.UI.DataBinding;

namespace Gallio.Icarus.TestExplorer
{
    public class TestExplorerModel : ITestExplorerModel
    {
        public Observable<bool> FilterPassed { get; private set; }
        public Observable<bool> FilterFailed { get; private set; }
        public Observable<bool> FilterInconclusive { get; private set; }
        
        public ITreeModel TreeModel { get; private set; }

        public Observable<Color> PassedColor { get; private set; }
        public Observable<Color> FailedColor { get; private set; }
        public Observable<Color> InconclusiveColor { get; private set; }
        public Observable<Color> SkippedColor { get; private set; }

        public Observable<IList<string>> TreeViewCategories { get; set; }
        public Observable<string> CurrentTreeViewCategory { get; set; }
        public Observable<IList<string>> CollapsedNodes { get; set; }
        public Observable<bool> CanEditTree { get; private set; }

        public TestExplorerModel(ISortedTreeModel treeModel)
        {
            FilterPassed = new Observable<bool>();
            FilterFailed = new Observable<bool>();
            FilterInconclusive = new Observable<bool>();

            TreeModel = treeModel;

            PassedColor = new Observable<Color>();
            FailedColor = new Observable<Color>();
            InconclusiveColor = new Observable<Color>();
            SkippedColor = new Observable<Color>();

            TreeViewCategories = new Observable<IList<string>>(new List<string>());
            
            CurrentTreeViewCategory = new Observable<string>();

            CollapsedNodes = new Observable<IList<string>>(new List<string>());

            CanEditTree = new Observable<bool>();
        }
    }
}
