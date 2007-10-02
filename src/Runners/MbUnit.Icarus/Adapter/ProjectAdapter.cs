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
using MbUnit.Core.Model;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Interfaces;

namespace MbUnit.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {

        private readonly IProjectAdapterView _View;
        private readonly IProjectAdapterModel _Model;
        private TestModel _testCollection;
        public event EventHandler<ProjectEventArgs> GetTestTree;
      

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model)
        {
            _View = view;
            _Model = model;

            _View.GetTestTree += _View_GetTestTree;
        }

        void _View_GetTestTree(object sender, ProjectLoadEventArgs e)
        {
            TestPackage testpackage = new TestPackage();
            //testpackage.AssemblyFiles.Add("C:\\Source\\MbUnitGoogle\\mb-unit\\v3\\src\\TestResources\\MbUnit.TestResources.MbUnit2\\bin\\MbUnit.TestResources.MbUnit2.dll");
            testpackage.AssemblyFiles.Add(e.assembly);

            ProjectEventArgs pa = new ProjectEventArgs(testpackage);

            EventHandler<ProjectEventArgs> getTestTree = GetTestTree;
            if (getTestTree != null)
            {
                getTestTree(this, pa);
            }
        }

        public TestModel TestCollection
        {
            set { _testCollection = value; }
        }

        public void DataBind()
        {
            _View.TestTreeCollection = _Model.BuildTestTree(_testCollection);
            _View.DataBind();
        }
    }
}
