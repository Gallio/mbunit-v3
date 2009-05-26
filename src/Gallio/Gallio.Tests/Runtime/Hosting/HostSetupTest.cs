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

using System;
using System.IO;
using System.Reflection;
using Gallio.Common.Policies;
using Gallio.Runtime.Hosting;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Tests.Runtime.Hosting
{
    [TestsOn(typeof(HostSetup))]
    public class HostSetupTest
    {
        [VerifyContract]
        public readonly IContract EqualityTests = new EqualityContract<HostSetup>
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { new HostSetup { }},
                { new HostSetup { ApplicationBaseDirectory = @"C:\AppBase" }},
                { new HostSetup { ApplicationBaseDirectory = @"C:\AppBase-2" }},
                { new HostSetup { Configuration = { ConfigurationXml = "<config/>" }}},
                { new HostSetup { Configuration = { ConfigurationXml = "<config-2/>" }}},
                { new HostSetup { ConfigurationFileLocation = ConfigurationFileLocation.AppBase }},
                { new HostSetup { ConfigurationFileLocation = ConfigurationFileLocation.None }},
                { new HostSetup { ProcessorArchitecture = ProcessorArchitecture.Amd64 }},
                { new HostSetup { ProcessorArchitecture = ProcessorArchitecture.IA64 }},
                { new HostSetup { ShadowCopy = true }},
                { new HostSetup { Debug = true }},
                { new HostSetup { Elevated = true }},
                { new HostSetup { WorkingDirectory = @"C:\WorkingDir" }},
                { new HostSetup { WorkingDirectory = @"C:\WorkingDir-2" }},
                { new HostSetup { Properties = { { "prop", "value" } }}},
                { new HostSetup { Properties = { { "prop", "value2" } }}},
                { new HostSetup { Properties = { { "prop", "value" }, { "other prop", "value" } }}}
            }
        };

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
                Assert.AreEqual(SpecialPathPolicy.For("Hosting").GetTempDirectory().FullName, Path.GetDirectoryName(path));
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

        [Test]
        public void Copy()
        {
            HostSetup setup = new HostSetup
            {
                ApplicationBaseDirectory = @"C:\AppBase",
                Configuration = { ConfigurationXml = "<xml/>" },
                ConfigurationFileLocation = ConfigurationFileLocation.AppBase,
                ProcessorArchitecture = ProcessorArchitecture.Amd64,
                ShadowCopy = true,
                Debug = true,
                RuntimeVersion = "2.0.50727",
                Elevated = true,
                WorkingDirectory = @"C:\WorkingDir",
                Properties = { { "abc", "def" } }
            };

            HostSetup copy = setup.Copy();

            Assert.AreEqual(setup.ApplicationBaseDirectory, copy.ApplicationBaseDirectory);
            Assert.AreEqual(setup.Configuration, copy.Configuration);
            Assert.AreEqual(setup.ConfigurationFileLocation, copy.ConfigurationFileLocation);
            Assert.AreEqual(setup.ProcessorArchitecture, copy.ProcessorArchitecture);
            Assert.AreEqual(setup.ShadowCopy, copy.ShadowCopy);
            Assert.AreEqual(setup.Elevated, copy.Elevated);
            Assert.AreEqual(setup.Debug, copy.Debug);
            Assert.AreEqual(setup.RuntimeVersion, copy.RuntimeVersion);
            Assert.AreEqual(setup.WorkingDirectory, copy.WorkingDirectory);
            Assert.AreEqual(setup.Properties, copy.Properties);
        }
    }
}
