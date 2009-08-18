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
using System.Threading;
using Gallio.Common.Collections;
using Gallio.Model.Filters;

namespace Gallio.Model
{
    /// <summary>
    /// Provides options that control how test execution occurs.
    /// </summary>
    [Serializable]
    public sealed class TestExecutionOptions
    {
        private readonly PropertySet properties;
        private FilterSet<ITestDescriptor> filterSet;
        private bool skipDynamicTests;
        private bool skipTestExecution;
        private bool exactFilter;
        private bool singleThreaded;

        /// <summary>
        /// Creates a default set of test execution options.
        /// </summary>
        public TestExecutionOptions()
        {
            filterSet = FilterSet<ITestDescriptor>.Empty;
            properties = new PropertySet();
        }

        /// <summary>
        /// Gets or sets the filter set.
        /// </summary>
        /// <value>Defaults to an empty filter set.</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public FilterSet<ITestDescriptor> FilterSet
        {
            get { return filterSet; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(@"value");

                filterSet = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the filter exactly specifies all tests to select.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <c>false</c>, then children of the selected tests are also included.
        /// </para>
        /// </remarks>
        /// <value>Defaults to false.</value>
        public bool ExactFilter
        {
            get { return exactFilter; }
            set { exactFilter = value; }
        }

        /// <summary>
        /// Gets or sets whether to skip running tests that use dynamic data items.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This flag can be useful in combination with <see cref="SkipTestExecution" />
        /// to enumerate non-dynamic tests only.
        /// </para>
        /// </remarks>
        /// <value>Defaults to <c>false</c></value>
        public bool SkipDynamicTests
        {
            get { return skipDynamicTests; }
            set { skipDynamicTests = value; }
        }

        /// <summary>
        /// Gets or sets whether to skip the execution of tests.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The test runner will go through most of the motions of running tests but will skip
        /// the actual execution phase.  This option can be used to enumerate data-driven test
        /// steps without running them.  It can also be used to verify that the execution
        /// environment is sane without doing most of the work of running the tests.
        /// </para>
        /// </remarks>
        /// <value>Defaults to <c>false</c></value>
        public bool SkipTestExecution
        {
            get { return skipTestExecution; }
            set { skipTestExecution = value; }
        }

        /// <summary>
        /// Gets or sets whether to run tests within a single thread.  This ensures that the
        /// tests will run on the same thread as the initial call to the test harness (unless
        /// the test framework itself spawns any new threads).
        /// </summary>
        /// <remarks>
        /// <para>
        /// When tests are run single-threaded, the test harness will no longer be able to guarantee
        /// that the default apartment state is <see cref="ApartmentState.STA"/>.
        /// </para>
        /// </remarks>
        /// <value>Defaults to <c>false</c>.</value>
        public bool SingleThreaded
        {
            get { return singleThreaded; }
            set { singleThreaded = value; }
        }

        /// <summary>
        /// Gets a read-only collection of configuration properties for test execution.
        /// </summary>
        public PropertySet Properties
        {
            get { return properties.AsReadOnly(); }
        }

        /// <summary>
        /// Clears the collection of properties.
        /// </summary>
        public void ClearProperties()
        {
            properties.Clear();
        }

        /// <summary>
        /// Adds a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if <paramref name="key"/> is already in the property set.</exception>
        public void AddProperty(string key, string value)
        {
            properties.Add(key, value); // note: implicitly checks arguments
        }

        /// <summary>
        /// Removes a property key/value pair.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        public void RemoveProperty(string key)
        {
            properties.Remove(key); // note: implicitly checks arguments
        }

        /// <summary>
        /// Creates a copy of the options.
        /// </summary>
        /// <returns>The copy.</returns>
        public TestExecutionOptions Copy()
        {
            var copy = new TestExecutionOptions();

            copy.filterSet = filterSet;
            copy.exactFilter = exactFilter;
            copy.skipDynamicTests = skipDynamicTests;
            copy.skipTestExecution = skipTestExecution;
            copy.singleThreaded = singleThreaded;
            copy.properties.AddAll(properties);

            return copy;
        }
    }
}