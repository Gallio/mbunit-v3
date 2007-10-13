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
using System.Diagnostics;

using MbUnit.Core.Harness;
using MbUnit.Icarus.Core.CustomEventArgs;
using MbUnit.Icarus.Core.Interfaces;
using MbUnit.Icarus.Interfaces;
using MbUnit.Model.Serialization;

namespace MbUnit.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {
        #region Variables

        private readonly IProjectAdapterView _View;
        private readonly IProjectAdapterModel _Model;
        
        private TestModel _testModel;
        private TestPackage _testPackage;
        
        #endregion

        #region Properties

        public TestModel TestModel
        {
            get { return _testModel; }
            set { _testModel = value; }
        }

        public TestPackage TestPackage
        {
            get { return _testPackage; }
            set { _testPackage = value; }
        }

        public string StatusText
        {
            get { return _View.StatusText; }
            set { _View.StatusText = value; }
        }

        public int CompletedWorkUnits
        {
            get { return _View.CompletedWorkUnits; }
            set { _View.CompletedWorkUnits = value; }
        }

        public int TotalWorkUnits
        {
            get { return _View.TotalWorkUnits; }
            set { _View.TotalWorkUnits = value; }
        }

        #endregion

        #region Events

        public event EventHandler<ProjectEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;

        #endregion

        #region Constructor

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model)
        {
            _View = view;
            _Model = model;

            // Wire up event handlers
            _View.AddAssemblies += _View_AddAssemblies;
            _View.RemoveAssemblies += _View_RemoveAssemblies;
            _View.GetTestTree += _View_GetTestTree;
            _View.RunTests += _View_RunTests;

            // Create empty new test package
            _testPackage = new TestPackage();
        }

        #endregion

        #region Event handlers

        private void _View_AddAssemblies(object sender, AddAssembliesEventArgs e)
        {
            _testPackage.AssemblyFiles.AddRange(e.Assemblies);
        }

        private void _View_RemoveAssemblies(object sender, EventArgs e)
        {
            _testPackage.AssemblyFiles.Clear();
        }

        [DebuggerStepThrough]
        private void _View_GetTestTree(object sender, EventArgs e)
        {
            if (GetTestTree != null)
            {
                GetTestTree(this, new ProjectEventArgs(_testPackage));
            }
        }

        [DebuggerStepThrough]
        private void _View_RunTests(object sender, EventArgs e)
        {
            if (RunTests != null)
            {
                RunTests(this, new EventArgs());
            }
        }

        #endregion

        #region Public methods

        public void DataBind()
        {
            _View.Assemblies = _Model.BuildAssemblyList(_testPackage.AssemblyFiles);
            _View.TestTreeCollection = _Model.BuildTestTree(_testModel);
            _View.TotalTests = _Model.CountTests(_testModel);
            _View.DataBind();
        }

        [DebuggerStepThrough]
        public void Passed(string testId)
        {
            _View.Passed(testId);
        }

        [DebuggerStepThrough]
        public void Failed(string testId)
        {
            _View.Failed(testId);
        }

        [DebuggerStepThrough]
        public void Skipped(string testId)
        {
            _View.Skipped(testId);
        }

        [DebuggerStepThrough]
        public void Ignored(string testId)
        {
            _View.Ignored(testId);
        }

        #endregion
    }
}
