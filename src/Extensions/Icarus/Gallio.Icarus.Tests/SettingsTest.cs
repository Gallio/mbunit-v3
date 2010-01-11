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

using Gallio.Runtime.Logging;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests
{
    [TestsOn(typeof(Settings))]
    public class SettingsTest
    {
        private Settings settings;

        [SetUp]
        public void SetUp()
        {
            settings = new Settings();
        }

        [Test]
        public void MinLogSeverity_should_default_to_info()
        {
            Assert.AreEqual(LogSeverity.Info, settings.MinLogSeverity);
        }

        [Test]
        public void TreeViewCategories_should_be_an_empty_list()
        {
            Assert.IsNotNull(settings.TreeViewCategories);
        }

        [Test]
        public void AnnotationsShowErrors_should_default_to_true()
        {
            Assert.IsTrue(settings.AnnotationsShowErrors);
        }

        [Test]
        public void AnnotationsShowInfos_should_default_to_true()
        {
            Assert.IsTrue(settings.AnnotationsShowInfos);
        }

        [Test]
        public void AnnotationsShowWarnings_should_default_to_true()
        {
            Assert.IsTrue(settings.AnnotationsShowWarnings);
        }
    }
}
