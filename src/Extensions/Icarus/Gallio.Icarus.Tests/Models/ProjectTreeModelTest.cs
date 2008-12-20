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

using MbUnit.Framework;
using Gallio.Icarus.Models;
using Gallio.Runner.Projects;
using System.Collections;
using Aga.Controls.Tree;

namespace Gallio.Icarus.Tests.Models
{
    class ProjectTreeModelTest
    {
        [Test]
        public void FileName_Test()
        {
            const string fileName = "fileName";
            ProjectTreeModel projectTreeModel = new ProjectTreeModel(fileName, new Project());
            Assert.AreEqual(fileName, projectTreeModel.FileName);
            const string fileName2 = "fileName2";
            projectTreeModel.FileName = fileName2;
            Assert.AreEqual(fileName2, projectTreeModel.FileName);
        }

        [Test]
        public void Project_Test()
        {
            Project project1 = new Project();
            ProjectTreeModel projectTreeModel = new ProjectTreeModel("fileName", project1);
            Assert.AreEqual(project1, projectTreeModel.Project);
            Project project2 = new Project();
            projectTreeModel.Project = project2;
            Assert.AreEqual(project2, projectTreeModel.Project);
        }

        [Test]
        public void GetChildren_Test_Empty_Path()
        {
            const string fileName = "fileName";
            ProjectTreeModel projectTreeModel = new ProjectTreeModel(fileName, new Project());
            IEnumerable children = projectTreeModel.GetChildren(TreePath.Empty);
            bool first = true;
            foreach (Node node in children)
            {
                Assert.AreEqual(fileName, node.Text);
                // should only be one node (the root)
                Assert.IsTrue(first);
                first = false;
            }
        }
    }
}
