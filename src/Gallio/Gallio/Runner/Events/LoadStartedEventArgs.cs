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
using Gallio.Model;

namespace Gallio.Runner.Events
{
    /// <summary>
    /// Arguments for an event raised to indicate that a test package is being loaded.
    /// </summary>
    public sealed class LoadStartedEventArgs : OperationStartedEventArgs
    {
        private readonly TestPackageConfig testPackageConfig;

        /// <summary>
        /// Initializes the event arguments.
        /// </summary>
        /// <param name="testPackageConfig">The test package configuration</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testPackageConfig"/> is null</exception>
        public LoadStartedEventArgs(TestPackageConfig testPackageConfig)
        {
            if (testPackageConfig == null)
                throw new ArgumentNullException("testPackageConfig");

            this.testPackageConfig = testPackageConfig;
        }

        /// <summary>
        /// Gets the test package configuration being loaded.
        /// </summary>
        public TestPackageConfig TestPackageConfig
        {
            get { return testPackageConfig; }
        }
    }
}
