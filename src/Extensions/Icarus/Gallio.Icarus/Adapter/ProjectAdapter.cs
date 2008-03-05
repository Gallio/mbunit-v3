// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.IO;

using Gallio.Icarus.Core.CustomEventArgs;
using Gallio.Icarus.Core.Interfaces;
using Gallio.Icarus.Core.Remoting;
using Gallio.Icarus.Interfaces;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Model.Serialization;
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

        private AssemblyWatcher assemblyWatcher = new AssemblyWatcher();

        public TestModelData TestModelData
        {
            get { return testModelData; }
            set { testModelData = value; }
        }

        public Project Project
        {
            get { return project; }
            set
            {
                project = value;
                
                CheckProject();

                // set project options
                projectAdapterView.HintDirectories = project.TestPackageConfig.HintDirectories;
                projectAdapterView.ApplicationBaseDirectory = project.TestPackageConfig.HostSetup.ApplicationBaseDirectory;
                projectAdapterView.WorkingDirectory = project.TestPackageConfig.HostSetup.WorkingDirectory;
                projectAdapterView.ShadowCopy = project.TestPackageConfig.HostSetup.ShadowCopy;

                // show available filters
                projectAdapterView.TestFilters = project.TestFilters;
                
                // attach assembly watcher
                assemblyWatcher.Add(value.TestPackageConfig.AssemblyFiles);
            }
        }

        private void CheckProject()
        {
            List<string> assemblyFiles = project.TestPackageConfig.AssemblyFiles;
            List<string> existingAndNonDuplicatedAssemblies = new List<string>();
            foreach (string file in assemblyFiles)
            {
                if (File.Exists(file) && !existingAndNonDuplicatedAssemblies.Contains(file))
                    existingAndNonDuplicatedAssemblies.Add(file);
            }
            assemblyFiles.Clear();
            assemblyFiles.AddRange(existingAndNonDuplicatedAssemblies);
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

        public string ReportPath
        {
            set { projectAdapterView.ReportPath = value; }
        }

        public IList<string> ReportTypes
        {
            set { projectAdapterView.ReportTypes = value; }
        }

        public IList<string> TestFrameworks
        {
            set { projectAdapterView.TestFrameworks = value; }
        }

        public Exception Exception
        {
            set { projectAdapterView.Exception = value; }
        }

        public event EventHandler<GetTestTreeEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> GenerateReport;
        public event EventHandler<EventArgs> StopTests;
        public event EventHandler<SetFilterEventArgs> SetFilter;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<EventArgs> GetTestFrameworks;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;

        public ProjectAdapter(IProjectAdapterView view, IProjectAdapterModel model)
        {
            projectAdapterView = view;
            projectAdapterModel = model;

            project = new Project();

            // Wire up event handlers
            projectAdapterView.AddAssemblies += AddAssembliesEventHandler;
            projectAdapterView.RemoveAssemblies += RemoveAssembliesEventHandler;
            projectAdapterView.RemoveAssembly += RemoveAssemblyEventHandler;
            projectAdapterView.GetTestTree += GetTestTreeEventHandler;
            projectAdapterView.RunTests += RunTestsEventHandler;
            projectAdapterView.GenerateReport += OnGenerateReport;
            projectAdapterView.StopTests += StopTestsEventHandler;
            projectAdapterView.SetFilter += SetFilterEventHandler;
            projectAdapterView.RemoveFilter += RemoveFilterEventHandler;
            projectAdapterView.GetReportTypes += GetReportTypesEventHandler;
            projectAdapterView.SaveReportAs += SaveReportAsEventHandler;
            projectAdapterView.SaveProject += SaveProjectEventHandler;
            projectAdapterView.OpenProject += OpenProjectEventHandler;
            projectAdapterView.NewProject += NewProjectEventHandler;
            projectAdapterView.GetTestFrameworks += OnGetTestFrameworks;
            projectAdapterView.GetSourceLocation += OnGetSourceLocation;
            projectAdapterView.UpdateHintDirectoriesEvent += OnUpdateHintDirectoriesEvent;
            projectAdapterView.UpdateApplicationBaseDirectoryEvent += OnUpdateApplicationBaseDirectoryEvent;
            projectAdapterView.UpdateWorkingDirectoryEvent += UpdateWorkingDirectoryEventHandler;
            projectAdapterView.UpdateShadowCopyEvent += UpdateShadowCopyEventHandler;

            // assembly watcher
            assemblyWatcher.AssemblyChangedEvent += new AssemblyWatcher.AssemblyChangedHandler(assemblyWatcher_AssemblyChangedEvent);
        }

        private void OnUpdateHintDirectoriesEvent(object sender, SingleEventArgs<IList<string>> e)
        {
            project.TestPackageConfig.HintDirectories.Clear();
            project.TestPackageConfig.HintDirectories.AddRange(e.Arg);
        }

        private void OnUpdateApplicationBaseDirectoryEvent(object sender, SingleEventArgs<string> e)
        {
            project.TestPackageConfig.HostSetup.ApplicationBaseDirectory = e.Arg;
        }

        private void UpdateWorkingDirectoryEventHandler(object sender, SingleEventArgs<string> e)
        {
            project.TestPackageConfig.HostSetup.WorkingDirectory = e.Arg;
        }

        private void UpdateShadowCopyEventHandler(object sender, SingleEventArgs<bool> e)
        {
            project.TestPackageConfig.HostSetup.ShadowCopy = e.Arg;
        }

        private void assemblyWatcher_AssemblyChangedEvent(string fullPath)
        {
            projectAdapterView.AssemblyChanged(fullPath);
        }

        private void AddAssembliesEventHandler(object sender, SingleEventArgs<IList<string>> e)
        {
            project.TestPackageConfig.AssemblyFiles.AddRange(e.Arg);
            foreach (string assembly in e.Arg)
                assemblyWatcher.Add(assembly);
        }

        private void RemoveAssembliesEventHandler(object sender, EventArgs e)
        {
            project.TestPackageConfig.AssemblyFiles.Clear();
            assemblyWatcher.Clear();
        }

        private void RemoveAssemblyEventHandler(object sender, SingleEventArgs<string> e)
        {
            string fileName;
            if (testModelData.Tests.ContainsKey(e.Arg))
                fileName = testModelData.Tests[e.Arg].Metadata.GetValue(MetadataKeys.CodeBase);
            else
                fileName = e.Arg;
            
            // remove assembly
            assemblyWatcher.Remove(fileName);
            project.TestPackageConfig.AssemblyFiles.Remove(fileName);
        }

        private void GetTestTreeEventHandler(object sender, GetTestTreeEventArgs e)
        {
            if (GetTestTree != null)
                GetTestTree(this, new GetTestTreeEventArgs(e.Mode, e.ReloadTestModelData, project.TestPackageConfig));
        }

        private void RunTestsEventHandler(object sender, EventArgs e)
        {
            // run tests
            if (RunTests != null)
                RunTests(this, e);
        }

        private void OnGenerateReport(object sender, EventArgs e)
        {
            if (GenerateReport != null)
                GenerateReport(this, e);
        }

        private void StopTestsEventHandler(object sender, EventArgs e)
        {
            if (StopTests != null)
                StopTests(this, e);
        }

        private void SetFilterEventHandler(object sender, SetFilterEventArgs e)
        {
            UpdateProjectFilter(e.FilterName, e.Filter);
            if (SetFilter != null)
                SetFilter(this, e);
        }

        private void RemoveFilterEventHandler(object sender, SingleEventArgs<string> e)
        {
            foreach (FilterInfo filterInfo in project.TestFilters)
            {
                if (filterInfo.FilterName == e.Arg)
                {
                    project.TestFilters.Remove(filterInfo);
                    return;
                }
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

        private void OnGetTestFrameworks(object sender, EventArgs e)
        {
            if (GetTestFrameworks != null)
                GetTestFrameworks(this, e);
        }

        private void SaveReportAsEventHandler(object sender, SaveReportAsEventArgs e)
        {
            if (SaveReportAs != null)
                SaveReportAs(this, e);
        }

        private void SaveProjectEventHandler(object sender, SingleEventArgs<string> e)
        {
            SerializationUtils.SaveToXml(project, e.Arg);
        }

        private void OpenProjectEventHandler(object sender, OpenProjectEventArgs e)
        {
            // deserialize project
            Project = SerializationUtils.LoadFromXml<Project>(e.FileName);

            // load test model data
            if (GetTestTree != null)
                GetTestTree(this, new GetTestTreeEventArgs(e.Mode, true, project.TestPackageConfig));

            // set filter (when available)
            foreach (FilterInfo filterInfo in project.TestFilters)
            {
                if (filterInfo.FilterName == "AutoSave")
                {
                    projectAdapterView.ApplyFilter(filterInfo.Filter);
                    if (SetFilter != null)
                        SetFilter(this, new SetFilterEventArgs(filterInfo.FilterName, FilterUtils.ParseTestFilter(filterInfo.Filter)));
                    break;
                }
            }
        }

        private void NewProjectEventHandler(object sender, EventArgs e)
        {
            project = new Project();
        }

        private void OnGetSourceLocation(object sender, SingleEventArgs<string> e)
        {
            TestData testData = testModelData.Tests[e.Arg];
            if (testData != null)
                projectAdapterView.SourceCodeLocation = testData.CodeLocation;
        }

        public void DataBind(string mode)
        {
            projectAdapterView.Assemblies = projectAdapterModel.BuildAssemblyList(project.TestPackageConfig.AssemblyFiles);
            projectAdapterView.TestTreeCollection = projectAdapterModel.BuildTestTree(testModelData, mode);
            projectAdapterView.TotalTests = projectAdapterModel.CountTests(testModelData);
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            projectAdapterView.Update(testData, testStepRun);
        }

        public void WriteToLog(string logName, string logBody)
        {
            projectAdapterView.WriteToLog(logName, logBody);
        }
    }
}
