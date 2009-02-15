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
using Gallio.Model;
using Gallio.Runtime.Hosting;

namespace Gallio.Model
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
        /// Gets the unique id of the test framework.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets the name of the test framework.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creates a test explorer for this test framework.
        /// </summary>
        /// <param name="testModel">The test model to populate incrementally as tests are discovered</param>
        /// <returns>The test explorer</returns>
        ITestExplorer CreateTestExplorer(TestModel testModel);

        /// <summary>
        /// Applies additional contributions to a test domain, if desired.
        /// </summary>
        /// <remarks>
        /// This method may be used by a test framework to set additional binding redirects
        /// or hint directories prior to loading a test package in a test domain.
        /// </remarks>
        /// <param name="testDomainSetup">The test domain setup to modify</param>
        void ConfigureTestDomain(TestDomainSetup testDomainSetup);
    }
}