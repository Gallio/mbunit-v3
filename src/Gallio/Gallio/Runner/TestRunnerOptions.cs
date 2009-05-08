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
using System.Collections.Specialized;
using Gallio.Common.Collections;
using Gallio.Runtime.Hosting;

namespace Gallio.Runner
{
    /// <summary>
    /// Provides options that control the operation of the test runner.
    /// </summary>
    [Serializable]
    public sealed class TestRunnerOptions
    {
        private readonly PropertySet properties;

        /// <summary>
        /// Creates a default set of options.
        /// </summary>
        public TestRunnerOptions()
        {
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets a mutable collection of key/value pairs that specify configuration properties
        /// for the test runner.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy</returns>
        public TestRunnerOptions Copy()
        {
            TestRunnerOptions copy = new TestRunnerOptions();
            copy.properties.AddAll(properties);

            return copy;
        }
    }
}
