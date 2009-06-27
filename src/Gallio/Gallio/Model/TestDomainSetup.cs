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
using Gallio.Runtime.Hosting;

namespace Gallio.Model
{
    /// <summary>
    /// Specifies setup options for a test domain.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A test domain represents an isolated host environment within which a subset of a test package
    /// will be loaded, explored and run.
    /// </para>
    /// </remarks>
    [Serializable]
    public class TestDomainSetup
    {
        // TODO: Provide a mechanism for configuring the host factory.

        private TestPackageConfig testPackageConfig;
        private HostSetup hostSetup;

        /// <summary>
        /// Creates setup options a test domain.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration to load in the test domain.</param>
        /// <param name="hostSetup">The host setup.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/> is null.</exception>
        public TestDomainSetup(TestPackageConfig testPackageConfig, HostSetup hostSetup)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");
            if (hostSetup == null)
                throw new ArgumentNullException("hostSetup");

            this.testPackageConfig = testPackageConfig;
            this.hostSetup = hostSetup;
        }

        /// <summary>
        /// Gets or sets the test package configuration to load in the test domain.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                testPackageConfig = value;
            }
        }

        /// <summary>
        /// Gets or sets the host setup for the test domain.
        /// </summary>
        public HostSetup HostSetup
        {
            get { return hostSetup; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                hostSetup = value;
            }
        }

        /// <summary>
        /// Merges the contents of another test domain setup into this one.
        /// </summary>
        /// <param name="source">The source setup.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public void MergeFrom(TestDomainSetup source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            foreach (string assemblyFile in source.TestPackageConfig.Files)
                if (!TestPackageConfig.Files.Contains(assemblyFile))
                    TestPackageConfig.Files.Add(assemblyFile);

            foreach (string hintDir in source.TestPackageConfig.HintDirectories)
                if (!TestPackageConfig.HintDirectories.Contains(hintDir))
                    TestPackageConfig.HintDirectories.Add(hintDir);
        }
    }
}
