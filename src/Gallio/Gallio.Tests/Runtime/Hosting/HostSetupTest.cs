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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestsOn(typeof(HostSetup))]
    public class HostSetupTest
    {
        [Test]
        public void WriteTemporaryConfigurationFile_ReturnsNullWhenNone()
        {
            HostSetup setup = new HostSetup();
            setup.ConfigurationFileLocation = ConfigurationFileLocation.None;

            Assert.IsNull(setup.WriteTemporaryConfigurationFile());
        }

        [Test]
        public void WriteTemporaryConfigurationFile_UsesApplicationBaseDirectoryWhenRequested()
        {
            HostSetup setup = new HostSetup();
            setup.ApplicationBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            setup.ConfigurationFileLocation = ConfigurationFileLocation.AppBase;

            string path = setup.WriteTemporaryConfigurationFile();
            try
            {
                Assert.AreEqual(Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory), Path.GetDirectoryName(path));
                Assert.EndsWith(path, ".tmp.config");
                Assert.Contains(File.ReadAllText(path), "<configuration>");
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Test]
        public void WriteTemporaryConfigurationFile_ThrowsIfApplicationBaseDirectoryMissingButRequested()
        {
            HostSetup setup = new HostSetup();
            setup.ConfigurationFileLocation = ConfigurationFileLocation.AppBase;

            Assert.Throws<InvalidOperationException>(() => setup.WriteTemporaryConfigurationFile());
        }

        [Test]
        public void WriteTemporaryConfigurationFile_UsesTempFolderByDefault()
        {
            HostSetup setup = new HostSetup();
            setup.ConfigurationFileLocation = ConfigurationFileLocation.Temp;

            string path = setup.WriteTemporaryConfigurationFile();
            try
            {
                Assert.AreEqual(Path.GetTempPath(), Path.GetDirectoryName(path) + @"\");
                Assert.Contains(File.ReadAllText(path), "<configuration>");
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Test]
        public void ProcessorArchitecture_CanGetSet()
        {
            HostSetup setup = new HostSetup();
            Assert.AreEqual(ProcessorArchitecture.MSIL, setup.ProcessorArchitecture);
            setup.ProcessorArchitecture = ProcessorArchitecture.X86;
            Assert.AreEqual(ProcessorArchitecture.X86, setup.ProcessorArchitecture);
        }
    }
}
