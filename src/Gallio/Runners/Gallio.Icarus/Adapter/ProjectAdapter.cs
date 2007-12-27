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
using Gallio.Runner;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {
        private readonly IProjectAdapterView projectAdapterView;
        private readonly IProjectAdapterModel projectAdapterModel;
        
        private TestModelData testModelData;
        private Project project;
        private Filter<ITest> filter;

        private string mode = "";

        public TestModelData TestModelData
        {
            get { return testModelData; }
            set { testModelData = value; }
        }

        public TestPackageConfig TestPackageConfig
        {
            get { return project.TestPackageConfig; }
            set { project.TestPackageConfig = value; }
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

        public Exception Exception
        {
            set { projectAdapterView.Exception = value; }
        }

        public event EventHandler<ProjectEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<SingleStringEventArgs> GetLogStream;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model)
        {
            projectAdapterView = view;
            projectAdapterModel = model;

            project = new Project();
            filter = new NoneFilter<ITest>();

            // Wire up event handlers
            projectAdapterView.AddAssemblies += AddAssembliesEventHandler;
            projectAdapterView.RemoveAssemblies += RemoveAssembliesEventHandler;
            projectAdapterView.RemoveAssembly += RemoveAssemblyEventHandler;
            projectAdapterView.GetTestTree += GetTestTreeEventHandler;
            projectAdapterView.RunTests += RunTestsEventHandler;
            projectAdapterView.StopTests += StopTestsEventHandler;
            projectAdapterView.SetFilter += SetFilterEventHandler;
            projectAdapterView.GetLogStream += GetLogStreamEventHandler;
            projectAdapterView.GetReportTypes += GetReportTypesEventHandler;
            projectAdapterView.SaveReportAs += SaveReportAsEventHandler;
            projectAdapterView.SaveProject += SaveProjectEventHandler;
            projectAdapterView.OpenProject += OpenProjectEventHandler;
            projectAdapterView.NewProject += NewProjectEventHandler;
        }

        private void AddAssembliesEventHandler(object sender, AddAssembliesEventArgs e)
        {
            project.TestPackageConfig.AssemblyFiles.AddRange(e.Assemblies);
        }

        private void RemoveAssembliesEventHandler(object sender, EventArgs e)
        {
            project.TestPackageConfig.AssemblyFiles.Clear();
        }

        private void RemoveAssemblyEventHandler(object sender, SingleStringEventArgs e)
        {
            project.TestPackageConfig.AssemblyFiles.Remove(e.String);
        }

        private void GetTestTreeEventHandler(object sender, SingleStringEventArgs e)
        {
            mode = e.String;
            if (GetTestTree != null)
            {
                GetTestTree(this, new ProjectEventArgs(project.TestPackageConfig));
            }
        }

        private void GetLogStreamEventHandler(object sender, SingleStringEventArgs e)
        {
            if (GetLogStream != null)
            {
                GetLogStream(this, e);
            }
        }

        private void RunTestsEventHandler(object sender, EventArgs e)
        {
            // add/update "last run" filter in project
            UpdateProjectFilter("LastRun", filter);

            // run tests
            if (RunTests != null)
                RunTests(this, e);
        }

        private void StopTestsEventHandler(object sender, EventArgs e)
        {
            if (StopTests != null)
                StopTests(this, e);
        }

        private void SetFilterEventHandler(object sender, SetFilterEventArgs e)
        {
            if (SetFilter != null)
            {
                filter = projectAdapterModel.GetFilter(e.Nodes);
                if (filter == null)
                    filter = new NoneFilter<ITest>();
                UpdateProjectFilter(e.FilterName, filter);
                SetFilter(this, new SetFilterEventArgs(e.FilterName, filter));
            }
        }

        public void UpdateProjectFilter(string filterName, Filter<ITest> filter)
        {
            foreach (FilterInfo filterInfo in project.TestFilters)
            {
                if (filterInfo.FilterName == filterName)
                {
                    filterInfo.Filter = filter.ToString();
                    return;
                }
            }
            project.TestFilters.Add(new FilterInfo(filterName, filter.ToString()));
        }

        private void GetReportTypesEventHandler(object sender, EventArgs e)
        {
            if (GetReportTypes != null)
                GetReportTypes(this, e);
        }

        private void SaveReportAsEventHandler(object sender, SaveReportAsEventArgs e)
        {
            if (SaveReportAs != null)
                SaveReportAs(this, e);
        }

        private void SaveProjectEventHandler(object sender, SingleStringEventArgs e)
        {
            try
            {
                SerializationUtils.SaveToXml(project, e.String);
            }
            catch (Exception ex)
            {
                projectAdapterView.Exception = ex;
            }
        }

        private void OpenProjectEventHandler(object sender, SingleStringEventArgs e)
        {
            try
            {
                project = SerializationUtils.LoadFromXml<Project>(e.String);
                foreach (FilterInfo filterInfo in project.TestFilters)
                {
                    if (filterInfo.FilterName == "Latest")
                    {
                        //filter = FilterUtils.ParseTestFilter(filterInfo.Filter);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                projectAdapterView.Exception = ex;
            }
        }

        private void NewProjectEventHandler(object sender, EventArgs e)
        {
            project = new Project();
        }

        public void DataBind()
        {
            projectAdapterView.Assemblies = projectAdapterModel.BuildAssemblyList(project.TestPackageConfig.AssemblyFiles);
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
