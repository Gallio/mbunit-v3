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
using Gallio.Runtime.Extensibility;

namespace Gallio.Model
{
    /// <summary>
    /// The test framework service provides support for enumerating and executing
    /// tests that belong to some test framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A new third party test framework may be supported by defining and registering 
    /// a suitable implementation of this interface.
    /// </para>
    /// </remarks>
    [Traits(typeof(TestFrameworkTraits))]
    public interface ITestFramework
    {
        /// <summary>
        /// Gets the test driver factory for the framework.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When multiple test frameworks result the same test driver factory
        /// then only one instance of the test driver will be created and it will
        /// be shared by those frameworks.  Factory identity is based on delegate
        /// equality.
        /// </para>
        /// </remarks>
        /// <returns>The test driver factory.</returns>
        TestDriverFactory GetTestDriverFactory();
    }
}