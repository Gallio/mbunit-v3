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

namespace MbUnit.Framework.Model
{
    /// <summary>
    /// The test framework service provides support for enumerating and executing
    /// tests that belong to some test framework.  A new third party test framework
    /// may be supported by defining and registering a suitable implementation
    /// of this interface.
    /// </summary>
    public interface ITestFramework
    {
        /// <summary>
        /// Gets the name of the test framework.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Populates the test template tree with this framework's contributions.
        /// </summary>
        /// <param name="builder">The template tree builder</param>
        /// <param name="parent">The parent template</param>
        void BuildTemplates(TestTemplateTreeBuilder builder, ITestTemplate parent);

        /// <summary>
        /// Provides the test framework with an opportunity to perform processing
        /// just after a test assembly is loaded.  For example, it might quickly
        /// scan the assembly to configure assembly resolution strategies or
        /// to configure the behavior of built-in services in other ways.
        /// </summary>
        /// <param name="assembly">The loaded test assembly</param>
        void InitializeTestAssembly(Assembly assembly);
    }
}
