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
using MbUnit.Core.Harness;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Interfaces;
using MbUnit.Model.Serialization;

namespace MbUnit.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {
        private readonly IProjectAdapterView _View;
        private readonly IProjectAdapterModel _Model;
        
        private TestModel _testCollection;
        private TestPackage testpackage;

        public event EventHandler<ProjectEventArgs> GetTestTree;

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model)
        {
            _View = view;
            _Model = model;

            // Wire up event handlers
            _View.AddAssemblies += _View_AddAssemblies;
            _View.GetTestTree += _View_GetTestTree;

            // Create empty new test package
            testpackage = new TestPackage();
        }

        private void _View_AddAssemblies(object sender, AddAssembliesEventArgs e)
        {
            testpackage.AssemblyFiles.AddRange(e.Assemblies);
        }

        void _View_GetTestTree(object sender, EventArgs e)
        {
            if (GetTestTree != null)
            {
                GetTestTree(this, new ProjectEventArgs(testpackage));
            }
        }

        public TestModel TestCollection
        {
            set { _testCollection = value; }
        }

        public void DataBind()
        {
            _View.Assemblies = _Model.BuildAssemblyList(testpackage.AssemblyFiles);
            _View.TestTreeCollection = _Model.BuildTestTree(_testCollection);
            _View.DataBind();
        }
    }
}
