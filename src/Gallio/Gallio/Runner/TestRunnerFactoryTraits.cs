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
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runner
{
    /// <summary>
    /// Describes traits of an <see cref="ITestRunnerFactory"/> component.
    /// </summary>
    public class TestRunnerFactoryTraits : Traits
    {
        private readonly string name;
        private readonly string description;

        /// <summary>
        /// Creates test runner factory traits.
        /// </summary>
        /// <param name="name">The unique name of the kind of test runner created by the factory.</param>
        /// <param name="description">The description of test runner created by the factory.</param>
        public TestRunnerFactoryTraits(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            this.name = name;
            this.description = description;
        }

        /// <summary>
        /// Gets the unique name of the kind of test runner created by the factory.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of test runner created by the factory.
        /// </summary>
        public string Description
        {
            get { return description; }
        }
    }
}
