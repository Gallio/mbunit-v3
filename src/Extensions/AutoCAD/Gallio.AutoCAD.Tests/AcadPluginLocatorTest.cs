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

using System;
using System.IO;
using Gallio.Common.IO;
using Gallio.Runtime.Logging;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.AutoCAD.Tests
{
    [TestsOn(typeof (AcadPluginLocator))]
    public class AcadPluginLocatorTest
    {
        private IFileSystem fileSystem;
        private AcadPluginLocator pluginLocator;

        [SetUp]
        public void SetUp()
        {
            var logger = MockRepository.GenerateStub<ILogger>();
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            pluginLocator = new AcadPluginLocator(logger, fileSystem);
        }

        [Test]
        [Row("19.0s (LMS Tech)")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetPluginPath_WhenNoneAvailable_ThrowsFileNotFoundException(string version)
        {
            StubAvailablePlugins();
            pluginLocator.GetPluginPath(version);
        }

        [Test]
        [Row("18.0s (LMS Tech)")]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetPluginPath_WhenOnlyHigherVersionsAvailable_ThrowsFileNotFoundException(string version)
        {
            StubAvailablePlugins(181, 190);
            pluginLocator.GetPluginPath(version);
        }

        [Test]
        [Row("17.0s (LMS Tech)", "Gallio.AutoCAD.Plugin170.dll")]
        public void GetPluginPath_WhenExactVersionAvailable_ReturnsExactVersion(string version, string exactPlugin)
        {
            StubAvailablePlugins(160, 161, 162, 170, 171, 172, 180, 181, 182, 190);
            Assert.AreEqual(exactPlugin, pluginLocator.GetPluginPath(version));
        }

        [Test]
        [Row("18.0s (LMS Tech)", "Gallio.AutoCAD.Plugin172.dll")]
        public void GetPluginPath_WhenOnlyLowerVersionsAvailable_ReturnsHighestLowerVersion(string version, string highestLowerPlugin)
        {
            StubAvailablePlugins(160, 161, 162, 170, 171, 172);
            Assert.AreEqual(highestLowerPlugin, pluginLocator.GetPluginPath(version));
        }

        [Test]
        [Row(null, "Gallio.AutoCAD.Plugin190.dll")]
        public void GetPluginPath_WhenSpecifiedVersionIsNull_ReturnsHighestAvailableVersion(string version, string highestPlugin)
        {
            StubAvailablePlugins(180, 181, 190);
            Assert.AreEqual(highestPlugin, pluginLocator.GetPluginPath(null));
        }

        private void StubAvailablePlugins(params int[] availableVersions)
        {
            fileSystem.Stub(fs => fs.GetFilesInDirectory(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<SearchOption>.Is.Anything))
					 .Return(Array.ConvertAll(availableVersions, version => GetPluginFileName(version)));
        }

        private static string GetPluginFileName(int version)
        {
            return string.Format("Gallio.AutoCAD.Plugin{0}.dll", version);
        }
    }
}
