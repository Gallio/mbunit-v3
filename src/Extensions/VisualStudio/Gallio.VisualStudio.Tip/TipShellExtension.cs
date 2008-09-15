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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EnvDTE;
using Gallio.Runtime;
using Gallio.VisualStudio.Shell;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;

namespace Gallio.VisualStudio.Tip
{
    /// <summary>
    /// Shell extension for Tip.
    /// </summary>
    public class TipShellExtension : BaseShellExtension
    {
        private BuildEvents buildEvents;
        private SolutionEvents solutionEvents;

        private ITestManagement testManagement;
        private GallioTip tip;
        private GallioTuip tuip;

        public ITestManagement TestManagement
        {
            get
            {
                if (testManagement == null)
                    testManagement = Shell.GetVsService<ITestManagement>(typeof(STestManagement));
                return testManagement;
            }
        }

        public ITmi Tmi
        {
            get { return TestManagement.TmiInstance; }
        }

        public GallioTip Tip
        {
            get
            {
                if (tip == null)
                    tip = (GallioTip)Tmi.FindTipForTestType(Guids.GallioTestType);
                return tip;
            }
        }

        public GallioTuip Tuip
        {
            get
            {
                if (tuip == null)
                    tuip = Shell.GetVsService<GallioTuip>(typeof(SGallioTestService));
                return tuip;
            }
        }

        /// <inheritdoc />
        protected override void InitializeImpl()
        {
            Shell.ProfferVsService(typeof(SGallioTestService), () => new GallioTuip(this));

            buildEvents = Shell.DTE.Events.BuildEvents;
            buildEvents.OnBuildProjConfigBegin += OnBuildProjConfigBegin;
            buildEvents.OnBuildProjConfigDone += OnBuildProjConfigDone;

            solutionEvents = Shell.DTE.Events.SolutionEvents;
            solutionEvents.Opened += OnSolutionOpened;
            solutionEvents.ProjectAdded += OnProjectAdded;
        }

        /// <inheritdoc />
        protected override void ShutdownImpl()
        {
            if (buildEvents != null)
            {
                buildEvents.OnBuildProjConfigBegin -= OnBuildProjConfigBegin;
                buildEvents.OnBuildProjConfigDone -= OnBuildProjConfigDone;
                buildEvents = null;
            }

            if (solutionEvents != null)
            {
                solutionEvents.Opened -= OnSolutionOpened;
                solutionEvents.ProjectAdded -= OnProjectAdded;
                solutionEvents = null;
            }
        }

        private void OnBuildProjConfigBegin(string project, string projectConfig, string platform, string solutionConfig)
        {
        }

        private void OnBuildProjConfigDone(string project, string projectConfig, string platform, string solutionConfig, bool success)
        {
            if (success)
                RefreshTests(FindProject(project));
        }

        private void OnSolutionOpened()
        {
            /*
             * This code hangs indefinitely for me.
             * May be some interactions with R# or other plugins.  -- Jeff.
            IEnumerable<Project> projects = FindAllProjects();

            foreach (Project project in projects)
                RefreshTests(project);
             */
        }

        private void OnProjectAdded(Project project)
        {
            /*
            RefreshTests(project);
             */
        }

        private void RefreshTests(Project project)
        {
            try
            {
                RemoveGallioTests(project);
                PopulateGallioTests(project);
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while refreshing Gallio tests.", ex);
            }
        }

        private void RemoveGallioTests(Project project)
        {
            try
            {
                if (Tmi != null)
                {
                    ArrayList testsToRemove = new ArrayList();
                    foreach (ITestElement testElement in Tmi.GetTests())
                        if (testElement is GallioTestElement
                            && (project.UniqueName == null || testElement.ProjectData.ProjectRelativePath == project.UniqueName))
                            testsToRemove.Add(testElement);

                    if (testsToRemove.Count != 0)
                        Tmi.ReleaseTests(testsToRemove);
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while removing Gallio tests.", ex);
            }
        }

        private void PopulateGallioTests(Project project)
        {
            try
            {
                Solution solution = Shell.DTE.Solution;
                if (solution != null && Tmi != null && project != null)
                {
                    try
                    {
                        Guid projectId = GetProjectId(project);
                        string solutionName = GetSolutionName();
                        ProjectData projectData = new ProjectData(projectId, solutionName, project.Name, project.UniqueName);

                        string targetPath = GetProjectTargetPath(project);
                        if (targetPath != null)
                        {
                            string targetExtension = Path.GetExtension(targetPath);
                            if (targetExtension == @".dll" || targetExtension == @".exe")
                                UpdateTests(Tmi, targetPath, projectData);
                        }
                    }
                    catch (Exception ex)
                    {
                        UnhandledExceptionPolicy.Report("An exception occurred while populating Gallio tests.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("An exception occurred while populating Gallio tests.", ex);
            }
        }

        private Project FindProject(string projectUniqueName)
        {
            Solution solution = Shell.DTE.Solution;
            foreach (Project project in solution.Projects)
            {
                if (projectUniqueName == null
                    || project.UniqueName == projectUniqueName)
                    return project;

                Project subProject = FindProject(projectUniqueName, project.ProjectItems);
                if (subProject != null)
                    return subProject;
            }

            return null;
        }

        private Project FindProject(string projectUniqueName, ProjectItems parent)
        {
            foreach (ProjectItem projectItem in parent)
            {
                if (projectItem.SubProject != null)
                {
                    if (projectUniqueName == null
                        || projectItem.SubProject.UniqueName == projectUniqueName)
                        return projectItem.SubProject;

                    Project subProject = FindProject(projectUniqueName, projectItem.SubProject.ProjectItems);
                    if (subProject != null)
                        return subProject;
                }
            }

            return null;
        }

        private IEnumerable<Project> FindAllProjects()
        {
            List<Project> projects = new List<Project>();

            Solution solution = Shell.DTE.Solution;
            foreach (Project project in solution.Projects)
            {
                if (project.Kind != EnvDTE.Constants.vsProjectItemKindSolutionItems)
                    projects.Add(project);

                FindAllProjects(projects, project.ProjectItems);

            }
            return projects;
        }

        private void FindAllProjects(IList<Project> projects, ProjectItems parent)
        {
            foreach (ProjectItem projectItem in parent)
            {
                Project project = projectItem.SubProject;
                if (project != null)
                {
                    if (project.Kind != EnvDTE.Constants.vsProjectItemKindSolutionItems)
                        projects.Add(project);

                    FindAllProjects(projects, project.ProjectItems);
                }
            }
        }

        private Guid GetProjectId(Project project)
        {
            try
            {
                IVsHierarchy projectHierarchy = GetVsHierarchyFromProject(project);

                Guid guid;
                projectHierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out guid);
                return guid;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        private IVsHierarchy GetVsHierarchyFromProject(Project project)
        {
            IVsSolution solution = Shell.GetVsService<IVsSolution>(typeof(SVsSolution));
            IVsHierarchy projectHierarchy;
            solution.GetProjectOfUniqueName(project.UniqueName, out projectHierarchy);
            return projectHierarchy;
        }

        private string GetSolutionName()
        {
            IVsSolution solution = Shell.GetVsService<IVsSolution>(typeof(SVsSolution));
            object solutionNameObj;
            solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionBaseName, out solutionNameObj);
            return (string)solutionNameObj;
        }

        private void UpdateTests(ITmi tmi, string storage, ProjectData projectData)
        {
            IWarningHandler warningHandler = new WarningHandler(tmi);
            ICollection tests = Tip.Load(storage, projectData, warningHandler);
            tmi.AddOrUpdateTests(tests);
        }

        private string GetProjectTargetPath(Project project)
        {
            try
            {
                Configuration configuration = project.ConfigurationManager.ActiveConfiguration;
                if (configuration != null)
                {
                    string fullPath = (string)project.Properties.Item("FullPath").Value;
                    string outputPath = (string)configuration.Properties.Item("OutputPath").Value;
                    string outputFileName = (string)project.Properties.Item("OutputFileName").Value;

                    return Path.Combine(Path.Combine(fullPath, outputPath), outputFileName);
                }
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
