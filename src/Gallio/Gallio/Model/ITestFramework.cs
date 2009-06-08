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
        /// Registers the test explorers of this framework into an aggregate list of explorers
        /// from all frameworks.
        /// </summary>
        /// <param name="explorers">The explorer list, not null.</param>
        void RegisterTestExplorers(IList<ITestExplorer> explorers);
    }
}