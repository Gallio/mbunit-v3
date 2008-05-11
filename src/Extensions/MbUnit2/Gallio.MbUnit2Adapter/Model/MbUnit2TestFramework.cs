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
using System.IO;
using System.Reflection;
using Gallio.Model;
using Gallio.MbUnit2Adapter.Properties;
using Gallio.Runtime.Hosting;

namespace Gallio.MbUnit2Adapter.Model
{
    /// <summary>
    /// Builds a test object model based on reflection against MbUnit v2 framework attributes.
    /// </summary>
    public class MbUnit2TestFramework : BaseTestFramework
    {
        private static readonly string[] frameworkAssemblyFiles = new string[]
        {
            @"MbUnit.Framework.dll",
            @"MbUnit.Framework.2.0.dll",
            @"QuickGraph.Algorithms.dll",
            @"QuickGraph.dll",
            @"Refly.dll",
            @"TestFu.dll"
        };

        /// <inheritdoc />
        public override string Name
        {
            get { return Resources.MbUnit2TestFramework_FrameworkName; }
        }

        /// <inheritdoc />
        public override ITestExplorer CreateTestExplorer(TestModel testModel)
        {
            return new MbUnit2TestExplorer(testModel);
        }

        /// <inheritdoc />
        public override void ConfigureTestDomain(TestDomainSetup testDomainSetup)
        {
            foreach (string assembly in testDomainSetup.TestPackageConfig.AssemblyFiles)
            {
                string dir = Path.GetDirectoryName(assembly);
                if (ConfigureAssemblyRedirects(dir, testDomainSetup.TestPackageConfig.HostSetup.Configuration))
                    return;
            }
        }

        private static bool ConfigureAssemblyRedirects(string dir, HostConfiguration config)
        {
            bool foundOne = false;
            foreach (string file in frameworkAssemblyFiles)
            {
                string path = Path.Combine(dir, file);
                if (File.Exists(path))
                {
                    try
                    {
                        AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);
                        config.AddAssemblyBinding(assemblyName, new Uri(path).ToString(), true);
                        foundOne = true;
                    }
                    catch
                    {
                    }
                }
            }

            return foundOne;
        }
    }
}
