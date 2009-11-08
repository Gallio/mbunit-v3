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
using System.Text.RegularExpressions;
using Aga.Controls.Tree;
using Gallio.Common.IO;
using Gallio.Icarus.Models.ProjectTreeNodes;
using Gallio.Icarus.Reports;
using Gallio.Runner.Projects;

namespace Gallio.Icarus.Models
{
    public class ProjectTreeModel : TreeModelBase, IProjectTreeModel
    {
        private readonly IFileSystem fileSystem;
        private readonly Node projectRoot = new Node();
        private readonly ReportsNode reportsNode;
        private TestProject testProject = new TestProject();
        private string fileName;
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

        public TestProject TestProject
        {
            get { return testProject; }
            set
            {
                testProject = value;

                NotifyTestProjectChanged();

                reportMonitor = new ReportMonitor(testProject.ReportDirectory, 
                    testProject.ReportNameFormat);

                reportMonitor.ReportDirectoryCreated += (s, e) => RefreshReportFolder();
                reportMonitor.ReportDirectoryDeleted += (s, e) => RefreshReportFolder();

                reportMonitor.ReportCreated += (s, e) => ReportCreated(e.FileName);
                reportMonitor.ReportDeleted += (s, e) => ReportDeleted(e.FileName);
                reportMonitor.ReportRenamed += (s, e) => ReportRenamed(e.OldFileName, 
                    e.NewFileName);
            }
        }

        private void RefreshReportFolder()
        {
            OnStructureChanged(new TreePathEventArgs(new TreePath(new[] { projectRoot, reportsNode })));
        }

        private void ReportRenamed(string oldReportName, string newReportName)
        {
            var treePath = new TreePath(new[] { projectRoot, reportsNode });
            OnNodesRemoved(new TreeModelEventArgs(treePath, new[] { new ReportNode(oldReportName) }));
            OnNodesInserted(new TreeModelEventArgs(treePath, new[] { 0 }, 
                new[] { new ReportNode(newReportName) }));
        }

        private void ReportCreated(string reportName)
        {
            var treePath = new TreePath(new[] { projectRoot, reportsNode });
            OnNodesInserted(new TreeModelEventArgs(treePath, new[] { 0 }, 
                new[] { new ReportNode(reportName) }));
        }

        private void ReportDeleted(string reportName)
        {
            var treePath = new TreePath(new[] { projectRoot, reportsNode });
            OnNodesRemoved(new TreeModelEventArgs(treePath, new[] { new ReportNode(reportName) }));
        }

        public void NotifyTestProjectChanged()
        {
            OnStructureChanged(new TreePathEventArgs(new TreePath(projectRoot)));
        }

        public void NotifyReportsChanged()
        {
            var treePath = new TreePath(new[] { projectRoot });
            OnStructureChanged(new TreePathEventArgs(treePath));
        }

        public ProjectTreeModel(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;

            projectRoot.Nodes.Add(new PropertiesNode());
            
            projectRoot.Nodes.Add(new FilesNode());
            
            reportsNode = new ReportsNode(this, fileSystem);
            projectRoot.Nodes.Add(reportsNode);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return projectRoot;
            }
            else if (treePath.LastNode == projectRoot)
            {
                foreach (var n in projectRoot.Nodes)
                    yield return n;
            }
            else if (treePath.LastNode is FilesNode)
            {
                foreach (var file in testProject.TestPackage.Files)
                    yield return new FileNode(file.FullName);
            }
            else if (treePath.LastNode is ReportsNode && !string.IsNullOrEmpty(fileName))
            {
                var reportDirectory = Path.Combine(Path.GetDirectoryName(fileName), "Reports");

                if (!fileSystem.DirectoryExists(reportDirectory))
                    yield break;
             
                var regex = new Regex("{.}");
                var searchPattern = regex.Replace(testProject.ReportNameFormat, "*") + "*.xml";
                foreach (var file in fileSystem.GetFilesInDirectory(reportDirectory, searchPattern, 
                    SearchOption.TopDirectoryOnly))
                {
                    yield return new ReportNode(file);
                }
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            Node n = treePath.LastNode as Node;
            return n != projectRoot && !(n is FilesNode) && !(n is ReportsNode);
        }
    }
}
