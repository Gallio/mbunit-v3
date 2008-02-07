// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using Gallio.Model.Execution;
using Gallio.Properties;

namespace Gallio.Model
{
    /// <summary>
    /// The root test in the test tree.
    /// </summary>
    public class RootTest : BaseTest
    {
        /// <summary>
        /// Creates the root test.
        /// </summary>
        public RootTest()
            : base(Resources.RootTest_RootTestName, null)
        {
            Kind = TestKinds.Root;
        }

        /// <inheritdoc />
        public override Func<ITestController> TestControllerFactory
        {
            get { return CreateTestController; }
        }

        private static ITestController CreateTestController()
        {
            return new RecursiveTestController();
        }
    }
}
