// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System.Collections;
using System.IO;
using Aga.Controls.Tree;
using Gallio.Icarus.Models.Interfaces;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Models
{
    public class ProjectTreeModel : TreeModelBase, IProjectTreeModel
    {
        private readonly Node projectRoot;
        private readonly Node assemblies;
        private readonly Node reports;
        private readonly Node properties;
        private Project project;
        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                projectRoot.Text = Path.GetFileNameWithoutExtension(fileName);
                OnNodesChanged(new TreeModelEventArgs(TreePath.Empty, new[] { projectRoot } ));
            }
        }

        public Project Project
        {
            get { return project; }
            set
            {
                project = value;
                OnStructureChanged(new TreePathEventArgs(new TreePath(projectRoot)));
            }
        }

        public ProjectTreeModel(string fileName, Project project)
        {
            this.project = project;
            this.fileName = fileName;

            projectRoot = new Node(Path.GetFileNameWithoutExtension(fileName));
            
            properties = new Node("Properties");
            projectRoot.Nodes.Add(properties);
            properties.Image = Properties.Resources.PropertiesHS;
            
            assemblies = new Node("Assemblies");
            projectRoot.Nodes.Add(assemblies);
            
            reports = new Node("Reports");
            reports.Image = Properties.Resources.Report.ToBitmap();
            projectRoot.Nodes.Add(reports);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
                yield return projectRoot;
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
            else if (treePath.LastNode == reports && !string.IsNullOrEmpty(fileName))
            {
                string reportDirectory = Path.Combine(Path.GetDirectoryName(fileName), "Reports");
                if (Directory.Exists(reportDirectory))
                {
                    foreach (string file in Directory.GetFiles(reportDirectory, "*.xml", SearchOption.AllDirectories))
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
            return (n != projectRoot && n != assemblies) && n != reports;
        }

        public void Refresh()
        {
            OnStructureChanged(new TreePathEventArgs());
        }
    }
}
