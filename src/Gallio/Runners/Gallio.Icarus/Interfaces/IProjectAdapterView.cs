// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Windows.Forms;
using Gallio.Icarus.Core.CustomEventArgs;

namespace Gallio.Icarus.Interfaces
{
    public interface IProjectAdapterView
    {
        event EventHandler<AddAssembliesEventArgs> AddAssemblies;
        event EventHandler<EventArgs> RemoveAssemblies;
        event EventHandler<SingleStringEventArgs> RemoveAssembly;
        event EventHandler<SingleStringEventArgs> GetTestTree;
        event EventHandler<EventArgs> RunTests;
        event EventHandler<EventArgs> StopTests;
        event EventHandler<SetFilterEventArgs> SetFilter;
        event EventHandler<SingleStringEventArgs> GetLogStream;
        event EventHandler<SingleStringEventArgs> GenerateReport;
        TreeNode[] TestTreeCollection { set; }
        ListViewItem[] Assemblies { set; }
        string StatusText { set; }
        string LogBody { set; }
        int CompletedWorkUnits { set; }
        int TotalWorkUnits { set; }
        void DataBind();
        void Passed(object o);
        void Failed(object o);
        void Ignored(object o);
        void Skipped(object o);
        void TotalTests(object o);
    }
}
