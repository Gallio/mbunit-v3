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

using Aga.Controls.Tree.NodeControls;
using Gallio.Icarus.Controls;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Controls
{
    class ProjectTreeViewTest
    {
        [Test]
        public void Constructor_Test()
        {
            ProjectTreeView projectTreeView = new ProjectTreeView();
            Assert.AreEqual(2, projectTreeView.NodeControls.Count);
            
            NodeIcon nodeIcon = (NodeIcon)projectTreeView.NodeControls[0];
            Assert.AreEqual("Image", nodeIcon.DataPropertyName);
            Assert.AreEqual(1, nodeIcon.LeftMargin);
            Assert.IsNull(nodeIcon.ParentColumn);

            NodeTextBox nodeTextBox = (NodeTextBox) projectTreeView.NodeControls[1];
            Assert.AreEqual("Text", nodeTextBox.DataPropertyName);
            Assert.IsTrue(nodeTextBox.IncrementalSearchEnabled);
            Assert.IsFalse(nodeTextBox.EditEnabled);
            Assert.AreEqual(3, nodeTextBox.LeftMargin);
            Assert.IsNull(nodeTextBox.ParentColumn);
        }
    }
}
