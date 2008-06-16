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

using Gallio.NUnitAdapter.TestResources.Properties;
using NUnit.Framework;

namespace Gallio.NUnitAdapter.TestResources
{
    [TestFixture]
    public class SettingsTest
    {
        [Test]
        public void ReadUserSetting()
        {
            Assert.AreEqual("This is a user-level setting",
                Settings.Default["SampleUserSetting"]);
        }

        [Test]
        public void ReadUserSetting2()
        {
            Assert.AreEqual("This is another user-level setting",
                Settings.Default["SampleUserSetting2"]);
        }

        [Test]
        public void ReadApplicationSetting()
        {
            Assert.AreEqual("This is an application-level setting",
                Settings.Default["SampleApplicationSetting"]);
        }

        [Test]
        public void ReadApplicationSetting2()
        {
            Assert.AreEqual("This is another application-level setting",
                Settings.Default["SampleApplicationSetting2"]);
        }
    }
}
