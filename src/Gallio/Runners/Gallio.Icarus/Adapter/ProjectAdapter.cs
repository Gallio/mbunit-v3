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
using System.Collections.Generic;
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
        private readonly IProjectAdapterView projectAdapterView;
        private readonly IProjectAdapterModel projectAdapterModel;
        
        private TestModelData testModelData;
        private TestPackageConfig testPackageConfig;

        private string mode = "";

        public TestModelData TestModelData
        {
            get { return testModelData; }
            set { testModelData = value; }
        }

        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
        }

        public string StatusText
        {
            set { projectAdapterView.StatusText = value; }
        }

        public int CompletedWorkUnits
        {
            set { projectAdapterView.CompletedWorkUnits = value; }
        }

        public int TotalWorkUnits
        {
            set { projectAdapterView.TotalWorkUnits = value; }
        }

        public string LogBody
        {
            set { projectAdapterView.LogBody = value; }
        }

        public string ReportPath
        {
            set { projectAdapterView.ReportPath = value; }
        }

        public IList<string> ReportTypes
        {
            set { projectAdapterView.ReportTypes = value; }
        }

        public event EventHandler<ProjectEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<SingleStringEventArgs> GetLogStream;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        public event EventHandler<SingleStringEventArgs> SaveProject;

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model, TestPackageConfig config)
        {
            projectAdapterView = view;
            projectAdapterModel = model;
            testPackageConfig = config;

            // Wire up event handlers
            projectAdapterView.AddAssemblies += _View_AddAssemblies;
            projectAdapterView.RemoveAssemblies += _View_RemoveAssemblies;
            projectAdapterView.RemoveAssembly += _View_RemoveAssembly;
            projectAdapterView.GetTestTree += _View_GetTestTree;
            projectAdapterView.RunTests += _View_RunTests;
            projectAdapterView.StopTests += _View_StopTests;
            projectAdapterView.SetFilter += _View_SetFilter;
            projectAdapterView.GetLogStream += _View_GetLogStream;
            projectAdapterView.GetReportTypes += _View_GetReportTypes;
            projectAdapterView.SaveReportAs += _View_SaveReportAs;
            projectAdapterView.SaveProject += _View_SaveProject;
        }

        private void _View_AddAssemblies(object sender, AddAssembliesEventArgs e)
        {
            testPackageConfig.AssemblyFiles.AddRange(e.Assemblies);
        }

        private void _View_RemoveAssemblies(object sender, EventArgs e)
        {
            testPackageConfig.AssemblyFiles.Clear();
        }

        private void _View_RemoveAssembly(object sender, SingleStringEventArgs e)
        {
            testPackageConfig.AssemblyFiles.Remove(e.String);
        }

        private void _View_GetTestTree(object sender, SingleStringEventArgs e)
        {
            mode = e.String;
            if (GetTestTree != null)
            {
                GetTestTree(this, new ProjectEventArgs(testPackageConfig));
            }
        }

        private void _View_GetLogStream(object sender, SingleStringEventArgs e)
        {
            if (GetLogStream != null)
            {
                GetLogStream(this, e);
            }
        }

        private void _View_RunTests(object sender, EventArgs e)
        {
            if (RunTests != null)
            {
                RunTests(this, e);
            }
        }

        private void _View_StopTests(object sender, EventArgs e)
        {
            if (StopTests != null)
                StopTests(this, e);
        }

        private void _View_SetFilter(object sender, SetFilterEventArgs e)
        {
            if (SetFilter != null)
            {
                Filter<ITest> filter = projectAdapterModel.GetFilter(e.Nodes);
                if (filter == null)
                {
                    filter = new NoneFilter<ITest>();
                }
                SetFilter(this, new SetFilterEventArgs(e.FilterName, filter));
            }
        }

        private void _View_GetReportTypes(object sender, EventArgs e)
        {
            if (GetReportTypes != null)
                GetReportTypes(this, e);
        }

        private void _View_SaveReportAs(object sender, SaveReportAsEventArgs e)
        {
            if (SaveReportAs != null)
                SaveReportAs(this, e);
        }

        private void _View_SaveProject(object sender, SingleStringEventArgs e)
        {
            if (SaveProject != null)
                SaveProject(this, e);
        }

        public void DataBind()
        {
            projectAdapterView.Assemblies = projectAdapterModel.BuildAssemblyList(testPackageConfig.AssemblyFiles);
            projectAdapterView.TestTreeCollection = projectAdapterModel.BuildTestTree(testModelData, mode);
            projectAdapterView.TotalTests(projectAdapterModel.CountTests(testModelData));
            projectAdapterView.DataBind();
        }

        public void Passed(string testId)
        {
            projectAdapterView.Passed(testId);
        }

        public void Failed(string testId)
        {
            projectAdapterView.Failed(testId);
        }

        public void Skipped(string testId)
        {
            projectAdapterView.Skipped(testId);
        }

        public void Ignored(string testId)
        {
            projectAdapterView.Ignored(testId);
        }
    }
}
