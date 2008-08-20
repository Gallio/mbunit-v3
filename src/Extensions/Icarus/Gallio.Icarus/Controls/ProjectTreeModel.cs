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
using Aga.Controls.Tree;
using Gallio.Icarus.Controls.Interfaces;
using Gallio.Runner.Projects;
using Gallio.Utilities;
using NDepend.Helpers.FileDirectoryPath;
using System.Collections.Generic;

namespace Gallio.Icarus.Controls
{
    public class ProjectTreeModel : TreeModelBase, IProjectTreeModel
    {
        private readonly Node projectRoot;
        private readonly Node assemblies;
        private readonly Node reports;
        private Project project = new Project();
        private string fileName = string.Empty;

        private string FileName
        {
            set
            {
                fileName = value;
                projectRoot.Text = Path.GetFileNameWithoutExtension(value);
                OnStructureChanged(new TreePathEventArgs());
            }
        }

        public Project Project
        {
            get { return project; }
        }

        public ProjectTreeModel()
        {
            projectRoot = new Node("Default");
            assemblies = new Node("Assemblies");
            projectRoot.Nodes.Add(assemblies);
            reports = new Node("Reports");
            reports.Image = Properties.Resources.Report.ToBitmap();
            projectRoot.Nodes.Add(reports);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return projectRoot;
            }
            else if (treePath.LastNode == projectRoot)
            {
                foreach (Node n in projectRoot.Nodes)
                    yield return n;
            }
            else if (treePath.LastNode == assemblies)
            {
                foreach (string assemblyFile in project.TestPackageConfig.AssemblyFiles)
                {
                    Node n = new Node(Path.GetFileNameWithoutExtension(assemblyFile));
                    n.Image = Properties.Resources.Assembly;
                    n.Tag = assemblyFile;
                    yield return n;
                }
            }
            else if (treePath.LastNode == reports && fileName != string.Empty)
            {
                string reportDirectory = Path.Combine(Path.GetDirectoryName(fileName), "Reports");
                if (Directory.Exists(reportDirectory))
                {
                    foreach (string file in Directory.GetFiles(reportDirectory, ".xml", SearchOption.AllDirectories))
                    {
                        Node n = new Node(Path.GetFileNameWithoutExtension(file));
                        n.Image = Properties.Resources.XmlFile.ToBitmap();
                        n.Tag = file;
                        yield return n;
                    }
                }
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            Node n = treePath.LastNode as Node;
            if (n == projectRoot || n == assemblies || n == reports)
                return false;
            return true;
        }

        public void SaveProject(string file)
        {
            if (file == string.Empty)
            {
                // create folder (if necessary)
                if (!Directory.Exists(Paths.IcarusAppDataFolder))
                    Directory.CreateDirectory(Paths.IcarusAppDataFolder);
                file = Paths.DefaultProject;
            }
            ConvertToRelativePaths(Path.GetDirectoryName(file));
            XmlSerializationUtils.SaveToXml(project, file);
            FileName = file;
        }

        private void ConvertToRelativePaths(string directory)
        {
            IList<string> assemblyList = new List<string>();
            foreach (string assembly in project.TestPackageConfig.AssemblyFiles)
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
            project.TestPackageConfig.AssemblyFiles.Clear();
            project.TestPackageConfig.AssemblyFiles.AddRange(assemblyList);
        }

        public void LoadProject(string file)
        {
            // fail fast
            if (!File.Exists(file))
                throw new ArgumentException(String.Format("Project file {0} does not exist.", file));

            // deserialize project
            Environment.CurrentDirectory = Path.GetDirectoryName(file);
            project = XmlSerializationUtils.LoadFromXml<Project>(file);
            FileName = file;
        }

        public void NewProject()
        {
            project = new Project();
            fileName = string.Empty;
            projectRoot.Text = "Default";
            OnStructureChanged(new TreePathEventArgs());
        }
    }
}
