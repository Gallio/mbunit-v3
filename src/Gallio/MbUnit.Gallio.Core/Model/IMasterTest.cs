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
using System.Text;

namespace MbUnit.Core.Model
{
    /// <summary>
    /// <para>
    /// A master test is a test that can construct a <see cref="ITestController" /> to run itself
    /// and all of its children.
    /// </para>
    /// <para>
    /// The test harness scans the list of tests to execute for any
    /// <see cref="IMasterTest" /> it finds and partitions the test tree into discrete units
    /// of work to be carried out by the <see cref="ITestController" /> created by the appropriate
    /// <see cref="IMasterTest" />.
    /// </para>
    /// <para>
    /// For example, the top-level test created by a <see cref="ITestFramework" />
    /// is usually a <see cref="IMasterTest" />.  The <see cref="ITestController" /> created by the
    /// framework's master test will take care of setting up the environment for the entire
    /// subtree beneath it and actually running the tests.  This a <see cref="IMasterTest" /> represents
    /// a division of labor among multiple possible test execution strategies as defined by <see cref="ITestController" />.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// When master tests are nested, only the outermost master test is considered.  There currently
    /// do not exist any mechanisms for master tests to delegate control to one another in the middle
    /// of their execution.
    /// </para>
    /// </remarks>
    public interface IMasterTest
    {
        /// <summary>
        /// Creates a <see cref="ITestController" /> to run this test and all of its children.
        /// </summary>
        /// <returns>The new test controller</returns>
        ITestController CreateTestController();
    }
}
