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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// Describes traits of an <see cref="ITestFramework"/> component.
    /// </summary>
    public class TestFrameworkTraits : Traits
    {
        private readonly Guid id;
        private readonly string name;
        private Icon icon;
        private string[] frameworkAssemblyNames;
        private string[] testFileExtensions;

        /// <summary>
        /// Creates test framework traits.
        /// </summary>
        /// <param name="id">The framework's unique id</param>
        /// <param name="name">The framework's display name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        public TestFrameworkTraits(Guid id, string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Gets the framework's unique id.
        /// </summary>
        public Guid Id
        {
            get { return id; }
        }

        /// <summary>
        /// Gets the framework's display name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets the framework's icon, or null if none.
        /// </summary>
        public Icon Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        /// <summary>
        /// Gets or sets a flag that configures whether the <see cref="ITestExplorer.ConfigureTestDomain" />
        /// method must be called.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The framework manager consults this list to determine whether the framework component
        /// should participate in test domain setup.  If the flag is false, the default, then the
        /// framework component will not be instantiated during test domain setup in order to
        /// improve performance.
        /// </para>
        /// </remarks>
        public bool RequiresConfigureTestDomain { get; set; }

        /// <summary>
        /// Gets or sets the list of framework assembly names that are recognized and supported
        /// by this framework component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The framework manager consults this list to determine whether the framework component
        /// should participate in test exploration.  If none of the named framework assemblies are found
        /// among the test assembly references then the framework component will not be instantiated
        /// during test exploration in order to improve performance.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string[] FrameworkAssemblyNames
        {
            get { return frameworkAssemblyNames ?? EmptyArray<string>.Instance; }
            set
            {
                if (value == null || Array.IndexOf(value, null) >= 0)
                    throw new ArgumentNullException("value");
                frameworkAssemblyNames = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of file extensions recognized by the framework as test files.
        /// Extensions should be prefixed with a period, eg. '.xml'.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The framework manager consults this list to determine whether the framework component
        /// should participate in test exploration.  If none of the test extensions match the files
        /// specified by the test source then the framework component will not be instantiated
        /// during test exploration in order to improve performance.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string[] TestFileExtensions
        {
            get { return testFileExtensions ?? EmptyArray<string>.Instance; }
            set
            {
                if (value == null || Array.IndexOf(value, null) >= 0)
                    throw new ArgumentNullException("value");
                testFileExtensions = value;
            }
        }
    }
}
