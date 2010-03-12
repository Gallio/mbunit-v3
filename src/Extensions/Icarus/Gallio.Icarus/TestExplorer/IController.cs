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

using System;
using System.Collections.Generic;
using Gallio.Icarus.Models;
using Gallio.Model;

namespace Gallio.Icarus.TestExplorer
{
    public interface IController
    {
        event EventHandler SaveState;
        event EventHandler RestoreState;

        void SortTree(SortOrder sortOrder);
        void FilterStatus(TestStatus testStatus);
        void AddFiles(string[] fileNames);
        void RemoveAllFiles();
        void RemoveFile(string fileName);
        void RefreshTree();
        void ShowSourceCode(string testId);
        void ResetTests();
        void SetTreeSelection(IList<TestTreeNode> nodes);
    }
}
