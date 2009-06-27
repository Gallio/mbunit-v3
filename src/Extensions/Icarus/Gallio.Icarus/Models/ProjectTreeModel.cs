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
using Gallio.Common.IO;
using Gallio.Icarus.Models.ProjectTreeNodes;
using Gallio.Runner.Projects;
using Gallio.Icarus.Reports;

namespace Gallio.Icarus.Models
{
    public class ProjectTreeModel : TreeModelBase, IProjectTreeModel
    {
        private readonly IFileSystem fileSystem;
        private readonly Node projectRoot;
        private Project project = new Project();
        private string fileName = string.Empty;
        private ReportMonitor reportMonitor;

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value;
                projectRoot.Text = Path.GetFileNameWithoutExtension(fileName);
            }
        }

        public Project Project
        {
            get { return project; }
            set
            {
                project = value;
                OnStructureChanged(new TreePathEventArgs(new TreePath(projectRoot)));
                reportMonitor = new ReportMonitor(project);
                reportMonitor.ReportDirectoryChanged += (sender, e) => OnStructureChanged(new TreePathEventArgs());
            }
        }

        public ProjectTreeModel(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            projectRoot = new Node();
            
            projectRoot.Nodes.Add(new PropertiesNode());
            projectRoot.Nodes.Add(new AssembliesNode());
            projectRoot.Nodes.Add(new ReportsNode());
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
            else if (treePath.LastNode is AssembliesNode)
            {
                foreach (string assemblyFile in project.TestPackageConfig.Files)
                    yield return new AssemblyNode(assemblyFile);
            }
            else if (treePath.LastNode is ReportsNode && !string.IsNullOrEmpty(fileName))
            {
                string reportDirectory = Path.Combine(Path.GetDirectoryName(fileName), "Reports");
                if (fileSystem.DirectoryExists(reportDirectory))
                    foreach (string file in fileSystem.GetFilesInDirectory(reportDirectory, "*.xml", SearchOption.AllDirectories))
                        yield return new ReportNode(file);
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            Node n = treePath.LastNode as Node;
            return n != projectRoot && !(n is AssembliesNode) && !(n is ReportsNode);
        }
    }
}
