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

using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using Gallio.Icarus.AdapterModel;
using Gallio.Icarus.Interfaces;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.Model.Serialization;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class LogWindowTest
    {
        [Test]
        public void AppendText_Test()
        {
            string text = "blah blah blah";
            LogWindow logWindow = new LogWindow();
            Assert.AreEqual("Log", logWindow.Text);
            Assert.AreEqual(string.Empty, logWindow.LogBody);
            logWindow.AppendText(text);
            Assert.AreEqual(text, logWindow.LogBody);
        }

        [Test]
        public void AppendText2_Test()
        {
            string text = "blah blah blah";
            LogWindow logWindow = new LogWindow("test");
            Assert.AreEqual("test", logWindow.Text);
            Assert.AreEqual(string.Empty, logWindow.LogBody);
            logWindow.AppendText(text, Color.Black);
            Assert.AreEqual(text, logWindow.LogBody);
        }

        [Test]
        public void Clear_Test()
        {
            string text = "blah blah blah";
            LogWindow logWindow = new LogWindow();
            Assert.AreEqual(string.Empty, logWindow.LogBody);
            logWindow.AppendText(text);
            Assert.AreEqual(text, logWindow.LogBody);
            logWindow.Clear();
            Assert.AreEqual(string.Empty, logWindow.LogBody);
        }
    }
}