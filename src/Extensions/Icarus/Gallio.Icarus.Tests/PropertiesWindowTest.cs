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

using Gallio.Icarus.Interfaces;

using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestFixture]
    public class PropertiesWindowTest : MockTest
    {
        private IProjectAdapterView projectAdapterView;
        private PropertiesWindow propertiesWindow;

        [SetUp]
        public void SetUp()
        {
            projectAdapterView = mocks.CreateMock<IProjectAdapterView>();
            propertiesWindow = new PropertiesWindow(projectAdapterView);
        }

        [Test]
        public void HintDirectories_Test()
        {
            List<string> list = new List<string>();
            list.Add("test");
            mocks.ReplayAll();
            propertiesWindow.HintDirectories = list;
        }

        [Test]
        public void ApplicationBaseDirectory_Test()
        {
            string dir = "test";
            projectAdapterView.UpdateApplicationBaseDirectory(dir);
            mocks.ReplayAll();
            propertiesWindow.ApplicationBaseDirectory = "test";
        }

        [Test]
        public void WorkingDirectory_Test()
        {
            string dir = "test";
            projectAdapterView.UpdateWorkingDirectory(dir);
            mocks.ReplayAll();
            propertiesWindow.WorkingDirectory = "test";
        }

        [Test]
        public void ShadowCopy_Test()
        {
            projectAdapterView.UpdateShadowCopy(true);
            mocks.ReplayAll();
            propertiesWindow.ShadowCopy = true;
        }
    }
}