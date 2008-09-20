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
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Controllers.Interfaces
{
    public interface IProjectController
    {
        ITreeModel Model { get; }
        TestPackageConfig TestPackageConfig { get; }
        BindingList<FilterInfo> TestFilters { get; }
        BindingList<string> HintDirectories { get; }
        string ProjectFileName { get; }

        event EventHandler<AssemblyChangedEventArgs> AssemblyChanged;

        void AddAssemblies(IList<string> assemblies);
        void DeleteFilter(FilterInfo filterInfo);
        Filter<ITest> GetFilter(string filterName);
        void NewProject();
        void OpenProject(string projectName);
        void RemoveAllAssemblies();
        void RemoveAssembly(string fileName);
        void SaveFilter(string filterName, Filter<ITest> filter);
        void SaveProject(string projectName);
    }
}
