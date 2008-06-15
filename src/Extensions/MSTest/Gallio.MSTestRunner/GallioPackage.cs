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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Gallio.MSTestRunner;
using Gallio.MSTestRunner.Resources;
using Gallio.MSTestRunner.Runtime;
using Gallio.Runtime;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.Common;
using Microsoft.VisualStudio.TestTools.Vsip;
using Microsoft.VisualStudio;
using System.Collections.Generic;

namespace Gallio.MSTestRunner
{
    [PackageRegistration(UseManagedResourcesOnly = true, RegisterUsing = RegistrationMethod.Assembly)]
    // Note: can't register by CodeBase because the Tip loader assumes the assembly can be resolved by name.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    [InstalledProductRegistration(true, null, null, null)]
    [ProvideLoadKey("Standard", "3.0", "Gallio.MSTestRunner", "Gallio Project", VSPackageResourceIds.ProductLoadKeyId)]
    [ProvideTip(typeof(GallioTip), typeof(SGallioTestService))]
    [ProvideServiceForTestType(typeof(GallioTestElement), typeof(SGallioTestService))]
    [RegisterTestTypeNoEditor(typeof(GallioTestElement), typeof(GallioTip), new string[] { "dll", "exe" },
      new int[] { VSPackageResourceIds.TestTypeIconId, VSPackageResourceIds.TestTypeIconId }, VSPackageResourceIds.TestTypeNameId)]
    [Guid(Guids.MSTestRunnerPkgGuidString)]
    [ProvideAutoLoad("{f1536ef8-92ec-443c-9ed7-fdadf150da82}")]
    [ComVisible(true)]
    internal sealed class GallioPackage : Package, IVsInstalledProduct
    {
        private static GallioPackage instance;
        private ServiceProvider services;
        private BuildEvents buildEvents;
        private SolutionEvents solutionEvents;

        static GallioPackage()
        {
            GallioInitializer.Initialize();
        }

        public GallioPackage()
        {
            instance = this;
        }

        public static GallioPackage Instance
        {
            get { return instance; }
        }

        public ServiceProvider Services
        {
            get { return services; }
        }

        protected override void Initialize()
        {
            services = new ServiceProvider(this);

            base.Initialize();

            DTE2 dte = Services.DTE;
            if (dte != null)
            {
                buildEvents = dte.Events.BuildEvents;
                buildEvents.OnBuildProjConfigBegin += OnBuildProjConfigBegin;
                buildEvents.OnBuildProjConfigDone += OnBuildProjConfigDone;

                solutionEvents = dte.Events.SolutionEvents;
                solutionEvents.Opened += OnSolutionOpened;
                solutionEvents.ProjectAdded += OnProjectAdded;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (buildEvents != null)
                    {
                        buildEvents.OnBuildProjConfigBegin -= OnBuildProjConfigBegin;
                        buildEvents.OnBuildProjConfigDone -= OnBuildProjConfigDone;
                    }

                    if (solutionEvents != null)
                    {
                        solutionEvents.Opened -= OnSolutionOpened;
                        solutionEvents.ProjectAdded -= OnProjectAdded;
                    }
                }
            }
            finally
            {
                instance = null;
                services = null;
                buildEvents = null;
            }
        }

        int IVsInstalledProduct.IdBmpSplash(out uint pIdBmp)
        {
            pIdBmp = 0;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.OfficialName(out string pbstrName)
        {
            pbstrName = "Gallio Visual Studio Team System Extension";
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductID(out string pbstrPID)
        {
            pbstrPID = "Version " + GetType().Assembly.GetName().Version;
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.ProductDetails(out string pbstrProductDetails)
        {
            pbstrProductDetails = "Gallio integration for Visual Studio Team System";
            return VSConstants.S_OK;
        }

        int IVsInstalledProduct.IdIcoLogoForAboutbox(out uint pIdIco)
        {
            pIdIco = VSPackageResourceIds.ProductIconId;
            return VSConstants.S_OK;
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
            RefreshTests(project);
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
                ITmi tmi = Services.Tmi;
                if (tmi != null)
                {
                    ArrayList testsToRemove = new ArrayList();
                    foreach (ITestElement testElement in tmi.GetTests())
                        if (testElement is GallioTestElement
                            && (project.UniqueName == null || testElement.ProjectData.ProjectRelativePath == project.UniqueName))
                            testsToRemove.Add(testElement);

                    if (testsToRemove.Count != 0)
                        tmi.ReleaseTests(testsToRemove);
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
                Solution solution = Services.DTE.Solution;
                ITmi tmi = Services.Tmi;
                if (solution != null && tmi != null && project != null)
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
                                UpdateTests(tmi, targetPath, projectData);
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
            Solution solution = Services.DTE.Solution;
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

            Solution solution = Services.DTE.Solution;
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
            IVsSolution solution = Services.GetService<IVsSolution>(typeof(SVsSolution));
            IVsHierarchy projectHierarchy;
            solution.GetProjectOfUniqueName(project.UniqueName, out projectHierarchy);
            return projectHierarchy;
        }

        private string GetSolutionName()
        {
            IVsSolution solution = Services.GetService<IVsSolution>(typeof(SVsSolution));
            object solutionNameObj;
            solution.GetProperty((int)__VSPROPID.VSPROPID_SolutionBaseName, out solutionNameObj);
            return (string)solutionNameObj;
        }

        private void UpdateTests(ITmi tmi, string storage, ProjectData projectData)
        {
            IWarningHandler warningHandler = new WarningHandler(tmi);
            ICollection tests = Services.Tip.Load(storage, projectData, warningHandler);
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

        private class WarningHandler : IWarningHandler
        {
            private readonly ITmi tmi;

            public WarningHandler(ITmi tmi)
            {
                this.tmi = tmi;
            }

            public void Write(object sender, WarningEventArgs ea)
            {
                MethodInfo method = tmi.GetType().GetMethod("WriteWarning");
                method.Invoke(tmi, new object[] { sender, ea.Warning });
            }
        }
    }
}
