// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using Gallio.Runner.Projects;
using Gallio.Runner.Projects.Schema;
using MbUnit.Framework;
using Gallio.Icarus.Models;
using System.Collections;
using Aga.Controls.Tree;
using Rhino.Mocks;
using Gallio.Common.IO;

namespace Gallio.Icarus.Tests.Models
{
    [Category("Models"), Author("Graham Hay"), TestsOn(typeof(ProjectTreeModel))]
    internal class ProjectTreeModelTest
    {
        [Test]
        public void FileName_Test()
        {
            const string fileName = "fileName";
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var projectTreeModel = new ProjectTreeModel(fileSystem);
            
            projectTreeModel.FileName = fileName;
            
            Assert.AreEqual(fileName, projectTreeModel.FileName);
            const string fileName2 = "fileName2";
            projectTreeModel.FileName = fileName2;
            Assert.AreEqual(fileName2, projectTreeModel.FileName);
        }

        [Test]
        public void Project_Test()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var projectTreeModel = new ProjectTreeModel(fileSystem);

            var project = new TestProject();
            projectTreeModel.TestProject = project;
            Assert.AreEqual(project, projectTreeModel.TestProject);
        }

        [Test]
        public void GetChildren_Test_Empty_Path()
        {
            var fileSystem = MockRepository.GenerateStub<IFileSystem>();
            var projectTreeModel = new ProjectTreeModel(fileSystem);

            var children = projectTreeModel.GetChildren(TreePath.Empty);
            bool first = true;
            foreach (Node node in children)
            {
                Assert.AreEqual(string.Empty, node.Text);
                // should only be one node (the root)
                Assert.IsTrue(first);
                first = false;
            }
        }
    }
}
