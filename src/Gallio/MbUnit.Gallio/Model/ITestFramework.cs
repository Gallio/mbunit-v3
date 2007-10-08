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
using MbUnit.Core.Harness;
using MbUnit.Model;

namespace MbUnit.Model
{
    /// <summary>
    /// The test framework service provides support for enumerating and executing
    /// tests that belong to some test framework.  A new third party test framework
    /// may be supported by defining and registering a suitable implementation
    /// of this interface.
    /// </summary>
    /// <remarks>
    /// A test framework should register this interface rather than <see cref="ITestHarnessContributor" />
    /// directly because it allows the system to see which frameworks are available and
    /// to selectively enable them.
    /// </remarks>
    public interface ITestFramework : ITestHarnessContributor
    {
        /// <summary>
        /// Gets the name of the test framework.
        /// </summary>
        string Name { get; }
    }
}