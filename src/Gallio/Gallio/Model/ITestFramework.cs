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

using Gallio.Model;

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
        /// Gets the name of the test framework.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creates a test explorer for this test framework.
        /// </summary>
        /// <returns>The test explorer,</returns>
        ITestExplorer CreateTestExplorer();

        /// <summary>
        /// Prepares a test package in advance of test exploration.
        /// A test framework can take advantage of this opportunity to configure the
        /// assembly resolve to ensure that all assembly dependencies can be resolved
        /// as needed.
        /// </summary>
        /// <param name="package">The test package</param>
        void PrepareTestPackage(TestPackage package);
    }
}