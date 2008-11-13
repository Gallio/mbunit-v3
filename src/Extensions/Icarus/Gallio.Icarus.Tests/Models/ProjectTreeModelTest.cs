using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
