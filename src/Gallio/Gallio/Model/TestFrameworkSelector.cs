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
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Provides options for selecting test frameworks.
    /// </summary>
    public class TestFrameworkSelector
    {
        private TestFrameworkOptions options;

        /// <summary>
        /// Creates a test framework filter initialized to defaults.
        /// </summary>
        public TestFrameworkSelector()
        {
        }

        /// <summary>
        /// Gets or sets a filter predicate for selecting test frameworks to include, or null if all
        /// frameworks should be considered. 
        /// </summary>
        /// <value>
        /// The predicate.  Default is <c>null</c>.
        /// </value>
        public Predicate<ComponentHandle<ITestFramework, TestFrameworkTraits>> Filter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the fallback mode used to specify when the <see cref="FallbackTestFramework" />
        /// should be used if a test file is not supported by any other test framework.
        /// </summary>
        /// <value>
        /// The fallback mode.  Default is <see cref="TestFrameworkFallbackMode.Default" />.
        /// </value>
        public TestFrameworkFallbackMode FallbackMode { get; set; }

        /// <summary>
        /// Gets or sets the test framework options.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public TestFrameworkOptions Options
        {
            get
            {
                if (options == null)
                    options = new TestFrameworkOptions();
                return options;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                options = value;
            }
        }
    }
}
