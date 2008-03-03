// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

using System.Drawing;
using System.Windows.Forms;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Controls.Enums;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Gallio.Icarus.Tests.Controls
{
    [TestFixture]
    public class TaskButtonTest
    {
        private TaskButton taskButton;

        [SetUp]
        public void SetUp()
        {
            taskButton = new TaskButton();
        }

        [Test]
        public void Description_Test()
        {
            string desc = "test";
            Assert.AreEqual(string.Empty, taskButton.Description);
            taskButton.Description = desc;
            Assert.AreEqual(desc, taskButton.Description);
        }

        [Test]
        public void Icon_Test()
        {
            Bitmap bitmap = new Bitmap(1, 1);
            Assert.IsNull(taskButton.Icon);
            taskButton.Icon = bitmap;
            Assert.AreEqual(bitmap, taskButton.Icon);
        }

        [Test]
        public void DialogResult_Test()
        {
            taskButton.DialogResultChanged += delegate
            {
                Assert.AreEqual(DialogResult.OK, taskButton.DialogResult);
            };
            Assert.AreEqual(DialogResult.None, taskButton.DialogResult);
            taskButton.DialogResult = DialogResult.OK;
            Assert.AreEqual(DialogResult.OK, taskButton.DialogResult);
        }

        [Test]
        public void NotifyDefault_Test()
        {
            taskButton.IsDefaultChanged += delegate
            {
                Assert.IsTrue(taskButton.IsDefault);
            };
            Assert.IsFalse(taskButton.IsDefault);
            taskButton.NotifyDefault(true);
            Assert.IsTrue(taskButton.IsDefault);
        }

        [Test]
        public void PerformClick_Test()
        {
            bool flag = false;
            taskButton.Click += delegate
            {
                flag = true;
            };
            taskButton.PerformClick();
            Assert.IsTrue(flag);
        }
    }
}
