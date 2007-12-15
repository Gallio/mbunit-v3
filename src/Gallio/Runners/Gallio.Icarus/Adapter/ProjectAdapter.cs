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
using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {
        #region Variables

        private readonly IProjectAdapterView _View;
        private readonly IProjectAdapterModel _Model;
        
        private TestModelData _testModelData;
        private TestPackageConfig _testPackageConfig;

        private string mode = "";

        #endregion

        #region Properties

        public TestModelData TestModelData
        {
            get { return _testModelData; }
            set { _testModelData = value; }
        }

        public TestPackageConfig TestPackageConfig
        {
            get { return _testPackageConfig; }
        }

        public string StatusText
        {
            set { _View.StatusText = value; }
        }

        public int CompletedWorkUnits
        {
            set { _View.CompletedWorkUnits = value; }
        }

        public int TotalWorkUnits
        {
            set { _View.TotalWorkUnits = value; }
        }

        public string LogBody
        {
            set { _View.LogBody = value; }
        }

        #endregion

        #region Events

        public event EventHandler<ProjectEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<SingleStringEventArgs> GetLogStream;
        public event EventHandler<SingleStringEventArgs> GenerateReport;

        #endregion

        #region Constructor

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model, TestPackageConfig testPackageConfig)
        {
            _View = view;
            _Model = model;
            _testPackageConfig = testPackageConfig;

            // Wire up event handlers
            _View.AddAssemblies += _View_AddAssemblies;
            _View.RemoveAssemblies += _View_RemoveAssemblies;
            _View.RemoveAssembly += _View_RemoveAssembly;
            _View.GetTestTree += _View_GetTestTree;
            _View.RunTests += _View_RunTests;
            _View.StopTests += _View_StopTests;
            _View.SetFilter += _View_SetFilter;
            _View.GetLogStream += _View_GetLogStream;
            _View.GenerateReport += _View_GenerateReport;
        }

        #endregion

        #region Event handlers

        private void _View_AddAssemblies(object sender, AddAssembliesEventArgs e)
        {
            _testPackageConfig.AssemblyFiles.AddRange(e.Assemblies);
        }

        private void _View_RemoveAssemblies(object sender, EventArgs e)
        {
            _testPackageConfig.AssemblyFiles.Clear();
        }

        private void _View_RemoveAssembly(object sender, SingleStringEventArgs e)
        {
            _testPackageConfig.AssemblyFiles.Remove(e.String);
        }

        private void _View_GetTestTree(object sender, SingleStringEventArgs e)
        {
            mode = e.String;
            if (GetTestTree != null)
            {
                GetTestTree(this, new ProjectEventArgs(_testPackageConfig));
            }
        }

        [DebuggerStepThrough]
        private void _View_GetLogStream(object sender, SingleStringEventArgs e)
        {
            if (GetLogStream != null)
            {
                GetLogStream(this, e);
            }
        }

        [DebuggerStepThrough]
        private void _View_RunTests(object sender, EventArgs e)
        {
            if (RunTests != null)
            {
                RunTests(this, e);
            }
        }

        [DebuggerStepThrough]
        private void _View_StopTests(object sender, EventArgs e)
        {
            if (StopTests != null)
            {
                StopTests(this, e);
            }
        }

        private void _View_SetFilter(object sender, SetFilterEventArgs e)
        {
            if (SetFilter != null)
            {
                Filter<ITest> filter = _Model.GetFilter(e.Nodes);
                if (filter == null)
                {
                    filter = new NoneFilter<ITest>();
                }
                SetFilter(this, new SetFilterEventArgs(filter));
            }
        }

        private void _View_GenerateReport(object sender, SingleStringEventArgs e)
        {
            if (GenerateReport != null)
            {
                GenerateReport(this, e);
            }
        }

        #endregion

        #region Public methods

        public void DataBind()
        {
            _View.Assemblies = _Model.BuildAssemblyList(_testPackageConfig.AssemblyFiles);
            _View.TestTreeCollection = _Model.BuildTestTree(_testModelData, mode);
            _View.TotalTests(_Model.CountTests(_testModelData));
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
