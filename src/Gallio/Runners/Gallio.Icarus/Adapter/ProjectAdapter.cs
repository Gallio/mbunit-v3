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
using Gallio.Runner.Reports;

namespace Gallio.Icarus.Adapter
{
    public class ProjectAdapter : IProjectAdapter
    {
        private readonly IProjectAdapterView projectAdapterView;
        private readonly IProjectAdapterModel projectAdapterModel;
        
        private TestModelData testModelData;
        private Project project;
        private Filter<ITest> filter;

        public TestModelData TestModelData
        {
            get { return testModelData; }
            set { testModelData = value; }
        }

        public Project Project
        {
            get { return project; }
            set { project = value; }
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

        public IList<string> AvailableLogStreams
        {
            set { projectAdapterView.AvailableLogStreams = value; }
        }

        public Exception Exception
        {
            set { projectAdapterView.Exception = value; }
        }

        public event EventHandler<GetTestTreeEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<GetLogStreamEventArgs> GetLogStream;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        public event EventHandler<SingleStringEventArgs> GetAvailableLogStreams;

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
            projectAdapterView.GetAvailableLogStreams += GetAvailableLogStreamsEventHandler;
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

        private void GetTestTreeEventHandler(object sender, GetTestTreeEventArgs e)
        {
            if (GetTestTree != null)
                GetTestTree(this, new GetTestTreeEventArgs(e.Mode, e.ReloadTestModelData, true, project.TestPackageConfig));
        }

        private void GetLogStreamEventHandler(object sender, GetLogStreamEventArgs e)
        {
            if (GetLogStream != null)
                GetLogStream(this, e);
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
                    filterInfo.Filter = filter.ToFilterExpr();
                    return;
                }
            }
            project.TestFilters.Add(new FilterInfo(filterName, filter.ToFilterExpr()));
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

        private void OpenProjectEventHandler(object sender, OpenProjectEventArgs e)
        {
            try
            {
                project = SerializationUtils.LoadFromXml<Project>(e.FileName);
                if (GetTestTree != null)
                    GetTestTree(this, new GetTestTreeEventArgs(e.Mode, true, false, project.TestPackageConfig));
                foreach (FilterInfo filterInfo in project.TestFilters)
                {
                    if (filterInfo.FilterName == "Latest")
                    {
                        filter = FilterUtils.ParseTestFilter(filterInfo.Filter);
                        projectAdapterView.ApplyFilter(filter);
                        SetFilter(this, new SetFilterEventArgs(filterInfo.FilterName, filter));
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

        private void GetAvailableLogStreamsEventHandler(object sender, SingleStringEventArgs e)
        {
            if (GetAvailableLogStreams != null)
                GetAvailableLogStreams(this, e);
        }

        public void DataBind(string mode, bool initialCheckState)
        {
            projectAdapterView.Assemblies = projectAdapterModel.BuildAssemblyList(project.TestPackageConfig.AssemblyFiles);
            projectAdapterView.TestTreeCollection = projectAdapterModel.BuildTestTree(testModelData, mode, initialCheckState);
            projectAdapterView.TotalTests(projectAdapterModel.CountTests(testModelData));
            projectAdapterView.DataBind();
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            projectAdapterView.Update(testData, testStepRun);
        }
    }
}
