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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Specifies a test frameworks that has been selected by a <see cref="TestFrameworkSelector"/>.
    /// </summary>
    public class TestFrameworkSelection
    {
        private readonly ComponentHandle<ITestFramework, TestFrameworkTraits> testFrameworkHandle;
        private readonly TestFrameworkOptions testFrameworkOptions;
        private readonly bool isFallback;

        /// <summary>
        /// Creates a test framework selection.
        /// </summary>
        /// <param name="testFrameworkHandle">The selected test framework handle.</param>
        /// <param name="testFrameworkOptions">The test framework options.</param>
        /// <param name="isFallback">True if the selection includes the fallback test
        /// framework because a test file is not supported by any other registered
        /// test framework.  <seealso cref="TestFrameworkSelector.FallbackMode"/></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testFrameworkHandle"/>
        /// or <paramref name="testFrameworkOptions"/> is null.</exception>
        public TestFrameworkSelection(
            ComponentHandle<ITestFramework, TestFrameworkTraits> testFrameworkHandle,
            TestFrameworkOptions testFrameworkOptions,
            bool isFallback)
        {
            if (testFrameworkHandle == null)
                throw new ArgumentNullException("testFrameworkHandle");
            if (testFrameworkOptions == null)
                throw new ArgumentNullException("testFrameworkOptions");

            this.testFrameworkHandle = testFrameworkHandle;
            this.testFrameworkOptions = testFrameworkOptions;
            this.isFallback = isFallback;
        }

        /// <summary>
        /// Gets the selected test framework handle.
        /// </summary>
        public ComponentHandle<ITestFramework, TestFrameworkTraits> TestFrameworkHandle
        {
            get { return testFrameworkHandle; }
        }

        /// <summary>
        /// Gets the test framework options.
        /// </summary>
        public TestFrameworkOptions TestFrameworkOptions
        {
            get { return testFrameworkOptions; }
        }

        /// <summary>
        /// Returns true if the selected test framework is a fallback because no other
        /// registered test framework supports the requested test resources.
        /// </summary>
        /// <seealso cref="TestFrameworkSelector.FallbackMode"/>
        public bool IsFallback
        {
            get { return isFallback; }
        }
    }
}
