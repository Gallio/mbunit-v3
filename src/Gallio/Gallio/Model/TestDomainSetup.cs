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

namespace Gallio.Model
{
    /// <summary>
    /// Specifies setup options for a test domain.
    /// A test domain represents an isolated host environment within which a subset of a test package
    /// will be loaded, explored and run.
    /// </summary>
    /// <todo author="jeff">
    /// Provide a mechanism for configuring the host factory.
    /// </todo>
    [Serializable]
    public class TestDomainSetup
    {
        private TestPackageConfig testPackageConfig;

        /// <summary>
        /// Creates setup options a test domain.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration to load in the test domain</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/> is null</exception>
        public TestDomainSetup(TestPackageConfig testPackageConfig)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");

            this.testPackageConfig = testPackageConfig;
        }

        /// <summary>
        /// Gets or sets the test package configuration to load in the test domain.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
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
    }
}
