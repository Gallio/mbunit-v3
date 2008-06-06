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

namespace Gallio.Icarus.Controls
{
    public class ProjectTreeModel : TreeModelBase, IProjectTreeModel
    {
        private Node projectRoot, assemblies, reports;
        private Project project;
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
            project = new Project();

            projectRoot = new Node("Default");
            assemblies = new Node("Assemblies");
            projectRoot.Nodes.Add(assemblies);
            reports = new Node("Reports");
            reports.Image = global::Gallio.Icarus.Properties.Resources.Report.ToBitmap();
            projectRoot.Nodes.Add(reports);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return projectRoot;
            }
            else if ((Node)treePath.LastNode == projectRoot)
            {
                foreach (Node n in projectRoot.Nodes)
                    yield return n;
            }
            else if ((Node)treePath.LastNode == assemblies)
            {
                foreach (string assemblyFile in project.TestPackageConfig.AssemblyFiles)
                {
                    Node n = new Node(Path.GetFileNameWithoutExtension(assemblyFile));
                    n.Image = global::Gallio.Icarus.Properties.Resources.Assembly;
                    n.Tag = assemblyFile;
                    yield return n;
                }
            }
            else if ((Node)treePath.LastNode == reports && fileName != string.Empty)
            {
                string reportDirectory = Path.Combine(Path.GetDirectoryName(fileName), "Reports");
                if (Directory.Exists(reportDirectory))
                {
                    foreach (string file in Directory.GetFiles(reportDirectory, ".xml", SearchOption.AllDirectories))
                    {
                        Node n = new Node(Path.GetFileNameWithoutExtension(file));
                        n.Image = global::Gallio.Icarus.Properties.Resources.XmlFile.ToBitmap();
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

        public void SaveProject(string fileName)
        {
            if (fileName == string.Empty)
            {
                // create folder (if necessary)
                if (!Directory.Exists(Paths.IcarusAppDataFolder))
                    Directory.CreateDirectory(Paths.IcarusAppDataFolder);
                fileName = Paths.DefaultProject;
            }
            XmlSerializationUtils.SaveToXml(project, fileName);
            FileName = fileName;
        }

        public void LoadProject(string fileName)
        {
            // fail fast
            if (!File.Exists(fileName))
                throw new ArgumentException(String.Format("Project file {0} does not exist.", fileName));

            // deserialize project
            project = XmlSerializationUtils.LoadFromXml<Project>(fileName);
            FileName = fileName;
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
