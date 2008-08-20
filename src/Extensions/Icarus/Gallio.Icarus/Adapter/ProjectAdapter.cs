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
using System.IO;
using Gallio.Icarus.Controls.Interfaces;
using Gallio.Icarus.Core.CustomEventArgs;
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
        private readonly IProjectTreeModel projectTreeModel;
        private TestModelData testModelData;
        private readonly AssemblyWatcher assemblyWatcher = new AssemblyWatcher();
        private string mode;

        public TestModelData TestModelData
        {
            get { return testModelData; }
            set { testModelData = value; }
        }

        public Project Project
        {
            get { return projectTreeModel.Project; }
        }

        public string TaskName
        {
            set { projectAdapterView.TaskName = value; }
        }

        public string SubTaskName
        {
            set { projectAdapterView.SubTaskName = value; }
        }

        public double CompletedWorkUnits
        {
            set { projectAdapterView.CompletedWorkUnits = value; }
        }

        public double TotalWorkUnits
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

        public Stream ExecutionLog
        {
            set { projectAdapterView.ExecutionLog = value; }
        }

        public event EventHandler<GetTestTreeEventArgs> GetTestTree;
        public event EventHandler<EventArgs> RunTests;
        public event EventHandler<EventArgs> GenerateReport;
        public event EventHandler<EventArgs> CancelOperation;
        public event EventHandler<SingleEventArgs<Filter<ITest>>> SetFilter;
        public event EventHandler<EventArgs> GetReportTypes;
        public event EventHandler<EventArgs> GetTestFrameworks;
        public event EventHandler<SaveReportAsEventArgs> SaveReportAs;
        public event EventHandler<SingleEventArgs<IList<string>>> GetExecutionLog;
        public event EventHandler<EventArgs> UnloadTestPackage;

        public ProjectAdapter(IProjectAdapterView projectAdapterView, IProjectAdapterModel projectAdapterModel, 
            IProjectTreeModel projectTreeModel)
        {
            this.projectAdapterView = projectAdapterView;
            this.projectAdapterModel = projectAdapterModel;
            this.projectTreeModel = projectTreeModel;

            // set up project
            projectAdapterView.ProjectTreeModel = projectTreeModel;
            projectAdapterView.ShadowCopy = Project.TestPackageConfig.HostSetup.ShadowCopy = true;

            // Wire up event handlers
            projectAdapterView.AddAssemblies += AddAssembliesEventHandler;
            projectAdapterView.RemoveAssemblies += RemoveAssembliesEventHandler;
            projectAdapterView.RemoveAssembly += RemoveAssemblyEventHandler;
            projectAdapterView.GetTestTree += GetTestTreeEventHandler;
            projectAdapterView.RunTests += RunTestsEventHandler;
            projectAdapterView.GenerateReport += OnGenerateReport;
            projectAdapterView.CancelOperation += CancelOperationEventHandler;
            projectAdapterView.SaveFilter += SaveFilterEventHandler;
            projectAdapterView.DeleteFilter += DeleteFilterEventHandler;
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
            projectAdapterView.ResetTestStatus += OnResetTestStatus;
            projectAdapterView.ApplyFilter += OnApplyFilter;
            projectAdapterView.GetExecutionLog += OnGetExecutionLog;
            projectAdapterView.UnloadTestPackage += OnUnloadTestPackage;

            // wire up tree model
            projectAdapterView.TestTreeModel = projectAdapterModel.TreeModel;

            // assembly watcher
            assemblyWatcher.AssemblyChangedEvent += assemblyWatcher_AssemblyChangedEvent;
        }

        private void OnUpdateHintDirectoriesEvent(object sender, SingleEventArgs<IList<string>> e)
        {
            Project.TestPackageConfig.HintDirectories.Clear();
            Project.TestPackageConfig.HintDirectories.AddRange(e.Arg);
        }

        private void OnUpdateApplicationBaseDirectoryEvent(object sender, SingleEventArgs<string> e)
        {
            Project.TestPackageConfig.HostSetup.ApplicationBaseDirectory = e.Arg;
        }

        private void UpdateWorkingDirectoryEventHandler(object sender, SingleEventArgs<string> e)
        {
            Project.TestPackageConfig.HostSetup.WorkingDirectory = e.Arg;
        }

        private void UpdateShadowCopyEventHandler(object sender, SingleEventArgs<bool> e)
        {
            Project.TestPackageConfig.HostSetup.ShadowCopy = e.Arg;
        }

        public void assemblyWatcher_AssemblyChangedEvent(string fullPath)
        {
            projectAdapterView.AssemblyChanged(fullPath);
        }

        private void AddAssembliesEventHandler(object sender, SingleEventArgs<IList<string>> e)
        {
            foreach (string assembly in e.Arg)
            {
                if (!File.Exists(assembly))
                    continue;
                if (Path.GetExtension(assembly) == ".gallio")
                    projectTreeModel.LoadProject(assembly);
                else
                {
                    projectTreeModel.Project.TestPackageConfig.AssemblyFiles.Add(assembly);
                    assemblyWatcher.Add(assembly);
                }
            }
        }

        private void RemoveAssembliesEventHandler(object sender, EventArgs e)
        {
            Project.TestPackageConfig.AssemblyFiles.Clear();
            assemblyWatcher.Clear();
        }

        private void RemoveAssemblyEventHandler(object sender, SingleEventArgs<string> e)
        {
            TestData test = testModelData != null ? testModelData.GetTestById(e.Arg) : null;
            string fileName = test != null ? test.Metadata.GetValue(MetadataKeys.CodeBase) : e.Arg;
            
            // remove assembly
            assemblyWatcher.Remove(fileName);
            Project.TestPackageConfig.AssemblyFiles.Remove(fileName);
        }

        private void GetTestTreeEventHandler(object sender, GetTestTreeEventArgs e)
        {
            mode = e.Mode;
            if (e.ReloadTestModelData)
            {
                if (GetTestTree != null)
                    GetTestTree(this, new GetTestTreeEventArgs(Project.TestPackageConfig.HostSetup.ShadowCopy, 
                        Project.TestPackageConfig));
            }
            else
                DataBind();
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

        private void CancelOperationEventHandler(object sender, EventArgs e)
        {
            if (CancelOperation != null)
                CancelOperation(this, e);
        }

        private void SaveFilterEventHandler(object sender, SingleEventArgs<string> e)
        {
            Filter<ITest> filter = projectAdapterModel.CreateFilter();
            UpdateProjectFilter(e.Arg, filter);
            if (SetFilter != null)
                SetFilter(this, new SingleEventArgs<Filter<ITest>>(filter));
            projectAdapterView.TestFilters = UpdateTestFilters(Project.TestFilters);
        }

        private void DeleteFilterEventHandler(object sender, SingleEventArgs<string> e)
        {
            foreach (FilterInfo filterInfo in Project.TestFilters)
            {
                if (filterInfo.FilterName == e.Arg)
                {
                    Project.TestFilters.Remove(filterInfo);
                    break;
                }
            }
            projectAdapterView.TestFilters = UpdateTestFilters(Project.TestFilters);
        }

        private static List<string> UpdateTestFilters(IEnumerable<FilterInfo> filters)
        {
            List<string> list = new List<string>();
            foreach (FilterInfo filter in filters)
                list.Add(filter.FilterName);
            return list;
        }

        public void UpdateProjectFilter(string filterName, Filter<ITest> filter)
        {
            foreach (FilterInfo filterInfo in Project.TestFilters)
            {
                if (filterInfo.FilterName == filterName)
                {
                    filterInfo.Filter = filter.ToFilterExpr();
                    return;
                }
            }
            Project.TestFilters.Add(new FilterInfo(filterName, filter.ToFilterExpr()));
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
            projectTreeModel.SaveProject(e.Arg);
        }

        private void OpenProjectEventHandler(object sender, OpenProjectEventArgs e)
        {
            projectTreeModel.LoadProject(e.FileName);
            CheckProject();
            UpdateProject();

            // load test model data
            mode = e.Mode;
            if (GetTestTree != null)
                GetTestTree(this, new GetTestTreeEventArgs(Project.TestPackageConfig.HostSetup.ShadowCopy, 
                    Project.TestPackageConfig));

            ApplyFilter("AutoSave");
        }

        private void ApplyFilter(string filterName)
        {
            projectAdapterView.EditEnabled = false;

            // set filter (when available)
            foreach (FilterInfo filterInfo in Project.TestFilters)
            {
                if (filterInfo.FilterName == filterName)
                {
                    Filter<ITest> filter = FilterUtils.ParseTestFilter(filterInfo.Filter);
                    projectAdapterModel.ApplyFilter(filter);
                    if (SetFilter != null)
                        SetFilter(this, new SingleEventArgs<Filter<ITest>>(filter));
                    break;
                }
            }
            
            projectAdapterView.EditEnabled = true;
        }

        private void NewProjectEventHandler(object sender, EventArgs e)
        {
            projectTreeModel.NewProject();
            UpdateProject();
        }

        private void OnGetSourceLocation(object sender, SingleEventArgs<string> e)
        {
            TestData testData = testModelData.GetTestById(e.Arg);
            if (testData != null)
                projectAdapterView.SourceCodeLocation = testData.CodeLocation;
        }

        private void OnApplyFilter(object sender, SingleEventArgs<string> e)
        {
            ApplyFilter(e.Arg);
        }

        public void DataBind()
        {
            projectAdapterModel.BuildTestTree(testModelData, mode);
            projectAdapterView.Annotations = testModelData.Annotations;
            projectAdapterView.LoadComplete();
        }

        public void Update(TestData testData, TestStepRun testStepRun)
        {
            projectAdapterModel.Update(testData, testStepRun);
        }

        public void OnResetTestStatus(object sender, EventArgs e)
        {
            projectAdapterModel.ResetTestStatus();
        }

        public void OnGetExecutionLog(object sender, SingleEventArgs<IList<string>> e)
        {
            if (GetExecutionLog != null)
                GetExecutionLog(this, e);
        }

        public void OnUnloadTestPackage(object sender, EventArgs e)
        {
            if (UnloadTestPackage != null)
                UnloadTestPackage(this, e);
        }

        private void CheckProject()
        {
            List<string> assemblyFiles = Project.TestPackageConfig.AssemblyFiles;
            List<string> existingAndNonDuplicatedAssemblies = new List<string>();
            foreach (string file in assemblyFiles)
            {
                if (File.Exists(file) && !existingAndNonDuplicatedAssemblies.Contains(file))
                    existingAndNonDuplicatedAssemblies.Add(file);
            }
            assemblyFiles.Clear();
            assemblyFiles.AddRange(existingAndNonDuplicatedAssemblies);
        }

        private void UpdateProject()
        {
            // set project options
            projectAdapterView.HintDirectories = Project.TestPackageConfig.HintDirectories;
            projectAdapterView.ApplicationBaseDirectory = Project.TestPackageConfig.HostSetup.ApplicationBaseDirectory;
            projectAdapterView.WorkingDirectory = Project.TestPackageConfig.HostSetup.WorkingDirectory;
            projectAdapterView.ShadowCopy = Project.TestPackageConfig.HostSetup.ShadowCopy;

            // show available filters
            projectAdapterView.TestFilters = UpdateTestFilters(Project.TestFilters);

            // attach assembly watcher
            assemblyWatcher.Add(Project.TestPackageConfig.AssemblyFiles);
        }
    }
}
