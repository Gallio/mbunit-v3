// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Reflection;
using System.Text;

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// A test project provides parameters for test enumeration such as the list
    /// of test assemblies.
    /// </summary>
    public class TestProject
    {
        private List<Assembly> assemblies;

        /// <summary>
        /// Creates an empty test project.
        /// </summary>
        public TestProject()
        {
            assemblies = new List<Assembly>();
        }

        /// <summary>
        /// Gets the list of test assemblies.
        /// </summary>
        public IList<Assembly> Assemblies
        {
            get { return assemblies; }
        }

        // TODO: Filters, file-based test descriptions, framework-specific options etc...
    }
}
