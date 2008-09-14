using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Aga.Controls.Tree;
using Gallio.Icarus.Controllers.EventArgs;
using Gallio.Icarus.Controllers.Interfaces;
using Gallio.Icarus.Models;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Icarus.Remoting;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Projects;
using Gallio.Utilities;
using NDepend.Helpers.FileDirectoryPath;

namespace Gallio.Icarus.Controllers
{
    public class ProjectController : IProjectController, INotifyPropertyChanged
    {
        private readonly IProjectTreeModel projectTreeModel;
        private readonly BindingList<FilterInfo> testFilters;
        private readonly List<FilterInfo> testFiltersList = new List<FilterInfo>();
        private readonly BindingList<string> hintDirectories;
        private readonly List<string> hintDirectoriesList = new List<string>();
        private readonly AssemblyWatcher assemblyWatcher = new AssemblyWatcher();

        public event EventHandler<AssemblyChangedEventArgs> AssemblyChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ITreeModel Model
        {
            get { return projectTreeModel; }
        }

        public TestPackageConfig TestPackageConfig
        {
            get { return projectTreeModel.Project.TestPackageConfig; }
        }

        public BindingList<FilterInfo> TestFilters
        {
            get { return testFilters; }
        }

        public BindingList<string> HintDirectories
        {
            get { return hintDirectories; }
        }

        public string ProjectFileName
        {
            get { return projectTreeModel.FileName; }
        }

        public ProjectController(IProjectTreeModel projectTreeModel)
        {
            this.projectTreeModel = projectTreeModel;

            testFilters = new BindingList<FilterInfo>(testFiltersList);
            testFilters.ListChanged += testFilters_ListChanged;

            hintDirectories = new BindingList<string>(hintDirectoriesList);
            hintDirectories.ListChanged += hintDirectories_ListChanged;

            assemblyWatcher.AssemblyChangedEvent += assemblyWatcher_AssemblyChangedEvent;
        }

        void testFilters_ListChanged(object sender, ListChangedEventArgs e)
        {
            projectTreeModel.Project.TestFilters.Clear();
            projectTreeModel.Project.TestFilters.AddRange(testFiltersList);
        }

        void hintDirectories_ListChanged(object sender, ListChangedEventArgs e)
        {
            projectTreeModel.Project.TestPackageConfig.HintDirectories.Clear();
            projectTreeModel.Project.TestPackageConfig.HintDirectories.AddRange(hintDirectoriesList);
        }

        private void assemblyWatcher_AssemblyChangedEvent(string fullPath)
        {
            string assemblyName = Path.GetFileNameWithoutExtension(fullPath);
            EventHandlerUtils.SafeInvoke(AssemblyChanged, this, new AssemblyChangedEventArgs(assemblyName));
        }

        public void AddAssemblies(IList<string> assemblies)
        {
            IList<string> validAssemblies = new List<string>();
            foreach (string assembly in assemblies)
            {
                if (File.Exists(assembly))
                    validAssemblies.Add(assembly);
            }
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.AddRange(validAssemblies);
            assemblyWatcher.Add(validAssemblies);
        }

        public void DeleteFilter(string filterName)
        {
            foreach (FilterInfo filterInfo in testFilters)
            {
                if (filterInfo.FilterName != filterName)
                    continue;
                testFilters.Remove(filterInfo);
                return;
            }
        }

        public Filter<ITest> GetFilter(string filterName)
        {
            foreach (FilterInfo filterInfo in projectTreeModel.Project.TestFilters)
            {
                if (filterInfo.FilterName == filterName)
                    return FilterUtils.ParseTestFilter(filterInfo.Filter);
            }
            return null;
        }

        public void RemoveAllAssemblies()
        {
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.Clear();
        }

        public void RemoveAssembly(string fileName)
        {
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.Remove(fileName);
        }

        public void SaveFilter(string filterName, Filter<ITest> filter){
            foreach (FilterInfo filterInfo in testFilters)
            {
                if (filterInfo.FilterName != filterName)
                    continue;
                filterInfo.Filter = filter.ToFilterExpr();
                return;
            }
            testFilters.Add(new FilterInfo(filterName, filter.ToFilterExpr()));
        }

        private void ConvertToRelativePaths(string directory)
        {
            IList<string> assemblyList = new List<string>();
            foreach (string assembly in projectTreeModel.Project.TestPackageConfig.AssemblyFiles)
            {
                if (Path.IsPathRooted(assembly))
                {
                    try
                    {
                        FilePathAbsolute filePath = new FilePathAbsolute(assembly);
                        DirectoryPathAbsolute directoryPath = new DirectoryPathAbsolute(directory);
                        assemblyList.Add(filePath.GetPathRelativeFrom(directoryPath).Path);
                    }
                    catch
                    {
                        assemblyList.Add(assembly);
                    }
                }
                else
                    assemblyList.Add(assembly);
            }
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.Clear();
            projectTreeModel.Project.TestPackageConfig.AssemblyFiles.AddRange(assemblyList);
        }

        public void OpenProject(string projectName)
        {
            // fail fast
            if (!File.Exists(projectName))
                throw new ArgumentException(String.Format("Project file {0} does not exist.", projectName));

            // deserialize project
            Environment.CurrentDirectory = Path.GetDirectoryName(projectName);
            Project project = XmlSerializationUtils.LoadFromXml<Project>(projectName);
            projectTreeModel.FileName = projectName;
            projectTreeModel.Project = project;
            
            assemblyWatcher.Clear();
            assemblyWatcher.Add(project.TestPackageConfig.AssemblyFiles);

            PublishUpdates();
        }

        public void NewProject()
        {
            projectTreeModel.FileName = Paths.DefaultProject;
            projectTreeModel.Project = new Project();

            assemblyWatcher.Clear();

            PublishUpdates();
        }

        public void SaveProject(string projectName)
        {
            if (projectName == string.Empty)
            {
                // create folder (if necessary)
                if (!Directory.Exists(Paths.IcarusAppDataFolder))
                    Directory.CreateDirectory(Paths.IcarusAppDataFolder);
                projectName = Paths.DefaultProject;
            }
            ConvertToRelativePaths(Path.GetDirectoryName(projectName));
            XmlSerializationUtils.SaveToXml(projectTreeModel.Project, projectName);
        }

        private void PublishUpdates()
        {
            testFilters.Clear();
            foreach (FilterInfo filterInfo in projectTreeModel.Project.TestFilters)
                testFilters.Add(filterInfo);

            hintDirectories.Clear();
            foreach (string hintDirectory in TestPackageConfig.HintDirectories)
                hintDirectories.Add(hintDirectory);

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("TestPackageConfig"));
        }
    }
}
